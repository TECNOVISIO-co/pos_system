using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SalesSwift.Etl;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            var configuration = BuildConfiguration();
            var settings = configuration.GetRequiredSection("Etl").Get<EtlSettings>();
            if (settings is null)
            {
                Console.Error.WriteLine("No se pudo cargar la sección Etl del archivo de configuración.");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(settings.AccessConnectionString) || string.IsNullOrWhiteSpace(settings.PostgresConnectionString))
            {
                Console.Error.WriteLine("Las cadenas de conexión de Access y PostgreSQL son obligatorias.");
                return 1;
            }

            var mode = args.FirstOrDefault()?.ToLowerInvariant() switch
            {
                "delta" => LoadMode.Delta,
                "verify" => LoadMode.Verify,
                _ => LoadMode.Full
            };

            using var access = new OleDbConnection(settings.AccessConnectionString);
            using var postgres = new NpgsqlConnection(settings.PostgresConnectionString);

            await access.OpenAsync();
            await postgres.OpenAsync();
            await using var transaction = await postgres.BeginTransactionAsync();

            var context = new EtlContext(settings, access, postgres, transaction);

            await LoadTaxesAsync(context);
            await LoadProductsAsync(context);
            await LoadPriceListsAsync(context);
            await LoadCustomersAsync(context);
            await LoadWarehousesAsync(context);

            if (mode == LoadMode.Verify)
            {
                await VerifyTotalsAsync(context);
            }
            else
            {
                await LoadInvoicesAsync(context, mode);
                await LoadInvoiceItemsAsync(context, mode);
                await LoadPaymentsAsync(context, mode);
                await LoadReceivablesAsync(context, mode);
            }

            await transaction.CommitAsync();
            Console.WriteLine($"ETL completado en modo {mode}.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error crítico durante la ejecución del ETL.");
            Console.Error.WriteLine(ex);
            return 2;
        }
    }

    private static IConfigurationRoot BuildConfiguration() => new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true)
        .AddEnvironmentVariables(prefix: "ETL_")
        .Build();

    private static async Task LoadTaxesAsync(EtlContext context)
    {
        Console.WriteLine("Sincronizando tabla de impuestos...");
        var rows = (await context.Access.QueryAsync<TaxRow>("SELECT codigo, impuesto, porcentaje FROM impuestos")).ToList();
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.codigo))
            {
                continue;
            }

            var code = row.codigo.Trim();
            var name = string.IsNullOrWhiteSpace(row.impuesto) ? code : row.impuesto.Trim();
            var rate = Convert.ToDecimal(row.porcentaje ?? 0);

            const string sql = @"
                INSERT INTO catalog_taxes (code, name, rate, scope)
                VALUES (@code, @name, @rate, 'sales')
                ON CONFLICT (code) DO UPDATE SET
                    name = EXCLUDED.name,
                    rate = EXCLUDED.rate,
                    updated_at = now();";

            await context.Postgres.ExecuteAsync(sql, new { code, name, rate }, context.Transaction);
        }

        await context.WriteSyncLogAsync("impuestos", rows.Count());
    }

    private static async Task LoadProductsAsync(EtlContext context)
    {
        Console.WriteLine("Sincronizando productos...");
        const string query = @"
            SELECT codigo, descripcion, marca, modelo, precio_compra, precio_venta, iva, impuesto2, impuesto3, cantidad_minima, codigo_referencia
            FROM productos";

        var rows = (await context.Access.QueryAsync<ProductRow>(query)).ToList();
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.codigo))
            {
                continue;
            }

            var sku = row.codigo.Trim();
            var name = string.IsNullOrWhiteSpace(row.descripcion) ? sku : row.descripcion.Trim();
            var brand = row.marca?.Trim();
            var model = row.modelo?.Trim();
            var cost = SafeDecimal(row.precio_compra);
            var price = SafeDecimal(row.precio_venta);
            var minStock = SafeDecimal(row.cantidad_minima);
            var barcode = row.codigo_referencia?.Trim();

            const string sql = @"
                INSERT INTO catalog_products (sku, name, description, brand, model, unit_of_measure, cost, price, barcode, min_stock)
                VALUES (@sku, @name, @name, @brand, @model, 'und', @cost, @price, @barcode, @minStock)
                ON CONFLICT (sku) DO UPDATE SET
                    name = EXCLUDED.name,
                    brand = EXCLUDED.brand,
                    model = EXCLUDED.model,
                    cost = EXCLUDED.cost,
                    price = EXCLUDED.price,
                    barcode = EXCLUDED.barcode,
                    min_stock = EXCLUDED.min_stock,
                    updated_at = now()
                RETURNING id;";

            var productId = await context.Postgres.ExecuteScalarAsync<Guid>(sql, new { sku, name, brand, model, cost, price, barcode, minStock }, context.Transaction);

            await context.Postgres.ExecuteAsync("DELETE FROM catalog_product_taxes WHERE product_id = @productId", new { productId }, context.Transaction);
            var rates = new[] { row.iva, row.impuesto2, row.impuesto3 }
                .Where(v => v.HasValue && v.Value > 0)
                .Select(v => Convert.ToDecimal(v.Value))
                .Distinct()
                .ToArray();

            short priority = 1;
            foreach (var rate in rates)
            {
                var taxId = await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM catalog_taxes WHERE rate = @rate LIMIT 1", new { rate }, context.Transaction);
                if (taxId is null)
                {
                    continue;
                }

                const string taxSql = @"
                    INSERT INTO catalog_product_taxes (product_id, tax_id, priority)
                    VALUES (@productId, @taxId, @priority)
                    ON CONFLICT (product_id, tax_id) DO UPDATE SET priority = EXCLUDED.priority;";

                await context.Postgres.ExecuteAsync(taxSql, new { productId, taxId, priority }, context.Transaction);
                priority++;
            }
        }

        await context.WriteSyncLogAsync("productos", rows.Count());
    }

    private static async Task LoadPriceListsAsync(EtlContext context)
    {
        Console.WriteLine("Sincronizando listas de precios...");
        var rows = (await context.Access.QueryAsync<PriceListRow>("SELECT nombre_lista_precios, codigo_producto, precio_venta FROM listas_precios_productos")).ToList();
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.nombre_lista_precios) || string.IsNullOrWhiteSpace(row.codigo_producto))
            {
                continue;
            }

            var listCode = row.nombre_lista_precios.Trim();
            var productCode = row.codigo_producto.Trim();
            var price = SafeDecimal(row.precio_venta);

            var listId = await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM catalog_price_lists WHERE code = @code", new { code = listCode }, context.Transaction);
            if (listId is null)
            {
                listId = await context.Postgres.ExecuteScalarAsync<Guid>(
                    "INSERT INTO catalog_price_lists (code, name) VALUES (@code, @code) ON CONFLICT (code) DO UPDATE SET updated_at = now() RETURNING id",
                    new { code = listCode }, context.Transaction);
            }

            var productId = await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM catalog_products WHERE sku = @sku", new { sku = productCode }, context.Transaction);
            if (productId is null)
            {
                continue;
            }

            const string sql = @"
                INSERT INTO catalog_price_list_items (price_list_id, product_id, price)
                VALUES (@listId, @productId, @price)
                ON CONFLICT (price_list_id, product_id) DO UPDATE SET
                    price = EXCLUDED.price,
                    updated_at = now();";

            await context.Postgres.ExecuteAsync(sql, new { listId, productId, price }, context.Transaction);
        }

        await context.WriteSyncLogAsync("listas_precios_productos", rows.Count());
    }

    private static async Task LoadCustomersAsync(EtlContext context)
    {
        Console.WriteLine("Sincronizando clientes...");
        const string query = @"
            SELECT codigo, nit, nombre, razon_social, e_mail, telefono, celular, direccion, ciudad, departamento, saldo_puntos_acumulados, cupo_maximo_credito, lista_precios_productos, tipo_documento
            FROM clientes";

        var rows = (await context.Access.QueryAsync<CustomerRow>(query)).ToList();
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.codigo))
            {
                continue;
            }

            var code = row.codigo.Trim();
            var documentType = string.IsNullOrWhiteSpace(row.tipo_documento) ? "NIT" : row.tipo_documento.Trim();
            var documentNumber = string.IsNullOrWhiteSpace(row.nit) ? code : row.nit.Trim();
            var name = string.IsNullOrWhiteSpace(row.nombre) ? code : row.nombre.Trim();
            var tradeName = row.razon_social?.Trim();
            var email = row.e_mail?.Trim();
            var phone = row.telefono?.Trim();
            var mobile = row.celular?.Trim();
            var address = row.direccion?.Trim();
            var city = row.ciudad?.Trim();
            var state = row.departamento?.Trim();
            var loyalty = SafeDecimal(row.saldo_puntos_acumulados);
            var creditLimit = SafeDecimal(row.cupo_maximo_credito);
            Guid? priceListId = null;
            if (!string.IsNullOrWhiteSpace(row.lista_precios_productos))
            {
                priceListId = await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM catalog_price_lists WHERE code = @code", new { code = row.lista_precios_productos.Trim() }, context.Transaction);
            }

            const string sql = @"
                INSERT INTO crm_customers (code, document_type, document_number, name, trade_name, email, phone, mobile, address, city, state, loyalty_points, credit_limit, price_list_id)
                VALUES (@code, @documentType, @documentNumber, @name, @tradeName, @email, @phone, @mobile, @address, @city, @state, @loyalty, @creditLimit, @priceListId)
                ON CONFLICT (code) DO UPDATE SET
                    document_type = EXCLUDED.document_type,
                    document_number = EXCLUDED.document_number,
                    name = EXCLUDED.name,
                    trade_name = EXCLUDED.trade_name,
                    email = EXCLUDED.email,
                    phone = EXCLUDED.phone,
                    mobile = EXCLUDED.mobile,
                    address = EXCLUDED.address,
                    city = EXCLUDED.city,
                    state = EXCLUDED.state,
                    loyalty_points = EXCLUDED.loyalty_points,
                    credit_limit = EXCLUDED.credit_limit,
                    price_list_id = EXCLUDED.price_list_id,
                    updated_at = now();";

            await context.Postgres.ExecuteAsync(sql, new
            {
                code,
                documentType,
                documentNumber,
                name,
                tradeName,
                email,
                phone,
                mobile,
                address,
                city,
                state,
                loyalty,
                creditLimit,
                priceListId
            }, context.Transaction);
        }

        await context.WriteSyncLogAsync("clientes", rows.Count());
    }

    private static async Task LoadWarehousesAsync(EtlContext context)
    {
        Console.WriteLine("Sincronizando bodegas...");
        var rows = (await context.Access.QueryAsync<WarehouseRow>("SELECT id_bodega AS codigo, descripcion FROM bodegas_inventarios")).ToList();
        if (!rows.Any())
        {
            rows = (await context.Access.QueryAsync<WarehouseRow>("SELECT Id AS codigo, descripcion FROM Bodegas")).ToList();
        }

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.codigo))
            {
                continue;
            }

            var code = row.codigo.Trim();
            var name = string.IsNullOrWhiteSpace(row.descripcion) ? $"Bodega {code}" : row.descripcion.Trim();

            const string sql = @"
                INSERT INTO inventory_warehouses (code, name)
                VALUES (@code, @name)
                ON CONFLICT (code) DO UPDATE SET
                    name = EXCLUDED.name,
                    updated_at = now();";

            await context.Postgres.ExecuteAsync(sql, new { code, name }, context.Transaction);
        }

        await context.WriteSyncLogAsync("bodegas", rows.Count());
    }

    private static async Task LoadInvoicesAsync(EtlContext context, LoadMode mode)
    {
        Console.WriteLine("Sincronizando facturas...");
        var invoices = (await context.Access.QueryAsync<InvoiceRow>("SELECT numero, codigo_cliente, fecha, anulada FROM facturas")).ToList();
        var totals = (await context.Access.QueryAsync<InvoiceTotalRow>(@"
            SELECT num_factura AS numero,
                   SUM(cantidad * valor_unitario) AS subtotal,
                   SUM(descuento) AS descuento,
                   SUM(IIF(impuesto IS NULL, 0, impuesto) + IIF(impuesto2 IS NULL, 0, impuesto2) + IIF(impuesto3 IS NULL, 0, impuesto3)) AS impuestos
            FROM detalle_factura
            GROUP BY num_factura")).ToList();
        var totalsMap = totals.ToDictionary(t => t.numero, t => t);

        foreach (var invoice in invoices)
        {
            if (invoice.numero is null)
            {
                continue;
            }

            var number = invoice.numero.Value.ToString();
            var customerCode = invoice.codigo_cliente?.Trim();
            var customerId = string.IsNullOrWhiteSpace(customerCode)
                ? null
                : await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM crm_customers WHERE code = @code", new { code = customerCode }, context.Transaction);

            if (customerId is null)
            {
                continue;
            }

            var status = string.Equals(invoice.anulada?.Trim(), "S", StringComparison.OrdinalIgnoreCase) ? "cancelled" : "posted";
            var issuedAt = invoice.fecha ?? DateTime.UtcNow;
        totalsMap.TryGetValue(invoice.numero.Value, out var detailTotals);
        var subtotal = SafeDecimal(detailTotals?.subtotal);
        var discount = SafeDecimal(detailTotals?.descuento);
        var taxes = SafeDecimal(detailTotals?.impuestos);
            var total = subtotal - discount + taxes;

            const string sql = @"
                INSERT INTO sales_invoices (invoice_number, customer_id, status, issued_at, subtotal, discount_total, tax_total, total)
                VALUES (@number, @customerId, @status, @issuedAt, @subtotal, @discount, @taxes, @total)
                ON CONFLICT (invoice_number) DO UPDATE SET
                    customer_id = EXCLUDED.customer_id,
                    status = EXCLUDED.status,
                    issued_at = EXCLUDED.issued_at,
                    subtotal = EXCLUDED.subtotal,
                    discount_total = EXCLUDED.discount_total,
                    tax_total = EXCLUDED.tax_total,
                    total = EXCLUDED.total,
                    updated_at = now();";

            await context.Postgres.ExecuteAsync(sql, new
            {
                number,
                customerId,
                status,
                issuedAt,
                subtotal,
                discount,
                taxes,
                total
            }, context.Transaction);
        }

        await context.WriteSyncLogAsync("facturas", invoices.Count());
    }

    private static async Task LoadInvoiceItemsAsync(EtlContext context, LoadMode mode)
    {
        Console.WriteLine("Sincronizando detalles de factura...");
        const string query = @"
            SELECT item, num_factura, cod_producto, cantidad, valor_unitario, descuento, impuesto, impuesto2, impuesto3
            FROM detalle_factura";

        var rows = (await context.Access.QueryAsync<InvoiceItemRow>(query)).ToList();
        var lineCounters = new Dictionary<long, int>();
        foreach (var row in rows)
        {
            if (row.num_factura is null || string.IsNullOrWhiteSpace(row.cod_producto))
            {
                continue;
            }

            var invoiceId = await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM sales_invoices WHERE invoice_number = @number", new { number = row.num_factura.Value.ToString() }, context.Transaction);
            var productId = await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM catalog_products WHERE sku = @sku", new { sku = row.cod_producto.Trim() }, context.Transaction);
            if (invoiceId is null || productId is null)
            {
                continue;
            }

            var quantity = SafeDecimal(row.cantidad);
            if (quantity <= 0)
            {
                continue;
            }

            var unitPrice = SafeDecimal(row.valor_unitario);
            var discountAmount = SafeDecimal(row.descuento);
            var taxTotal = SafeDecimal(row.impuesto) + SafeDecimal(row.impuesto2) + SafeDecimal(row.impuesto3);
            var lineTotal = quantity * unitPrice - discountAmount + taxTotal;
            int lineNumber;
            if (row.item.HasValue && row.item.Value > 0)
            {
                lineNumber = (int)row.item.Value;
            }
            else
            {
                lineCounters.TryGetValue(row.num_factura.Value, out var counter);
                counter++;
                lineCounters[row.num_factura.Value] = counter;
                lineNumber = counter;
            }

            const string sql = @"
                INSERT INTO sales_invoice_items (invoice_id, line_number, product_id, quantity, unit_price, discount_amount, tax_total, line_total)
                VALUES (@invoiceId, @lineNumber, @productId, @quantity, @unitPrice, @discountAmount, @taxTotal, @lineTotal)
                ON CONFLICT (invoice_id, line_number) DO UPDATE SET
                    product_id = EXCLUDED.product_id,
                    quantity = EXCLUDED.quantity,
                    unit_price = EXCLUDED.unit_price,
                    discount_amount = EXCLUDED.discount_amount,
                    tax_total = EXCLUDED.tax_total,
                    line_total = EXCLUDED.line_total,
                    updated_at = now();";

            await context.Postgres.ExecuteAsync(sql, new
            {
                invoiceId,
                lineNumber,
                productId,
                quantity,
                unitPrice,
                discountAmount,
                taxTotal,
                lineTotal
            }, context.Transaction);
        }

        await context.WriteSyncLogAsync("detalle_factura", rows.Count());
    }

    private static async Task LoadPaymentsAsync(EtlContext context, LoadMode mode)
    {
        Console.WriteLine("Sincronizando pagos...");
        var rows = (await context.Access.QueryAsync<PaymentRow>("SELECT numero_documento, tipo_documento, total_efectivo, total_tarjeta_credito, total_tarjeta_debito, total_cheque, total_bonos FROM detalle_pago")).ToList();
        foreach (var row in rows)
        {
            if (row.numero_documento is null)
            {
                continue;
            }

            if (!string.Equals(row.tipo_documento?.Trim(), "FACTURA", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var invoiceId = await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM sales_invoices WHERE invoice_number = @number", new { number = row.numero_documento.Value.ToString() }, context.Transaction);
            if (invoiceId is null)
            {
                continue;
            }

            var paidAt = DateTime.UtcNow;
            var payments = new Dictionary<string, decimal>
            {
                ["efectivo"] = SafeDecimal(row.total_efectivo),
                ["tarjeta_credito"] = SafeDecimal(row.total_tarjeta_credito),
                ["tarjeta_debito"] = SafeDecimal(row.total_tarjeta_debito),
                ["cheque"] = SafeDecimal(row.total_cheque),
                ["bonos"] = SafeDecimal(row.total_bonos)
            };

            foreach (var (method, amount) in payments)
            {
                if (amount <= 0)
                {
                    continue;
                }

                const string sql = @"
                    INSERT INTO sales_payments (invoice_id, payment_method, paid_at, amount, reference)
                    VALUES (@invoiceId, @method, @paidAt, @amount, @reference)
                    ON CONFLICT (invoice_id, payment_method, reference) DO UPDATE SET
                        amount = EXCLUDED.amount,
                        paid_at = EXCLUDED.paid_at,
                        updated_at = now();";

                await context.Postgres.ExecuteAsync(sql, new
                {
                    invoiceId,
                    method,
                    paidAt,
                    amount,
                    reference = method
                }, context.Transaction);
            }
        }

        await context.WriteSyncLogAsync("detalle_pago", rows.Count());
    }

    private static async Task LoadReceivablesAsync(EtlContext context, LoadMode mode)
    {
        Console.WriteLine("Sincronizando cuentas por cobrar...");
        var rows = (await context.Access.QueryAsync<InvoiceRow>("SELECT numero, codigo_cliente, fecha FROM cuentas_por_cobrar_gastos")).ToList();
        foreach (var row in rows)
        {
            if (row.numero is null || string.IsNullOrWhiteSpace(row.codigo_cliente))
            {
                continue;
            }

            var customerId = await context.Postgres.ExecuteScalarAsync<Guid?>("SELECT id FROM crm_customers WHERE code = @code", new { code = row.codigo_cliente.Trim() }, context.Transaction);
            if (customerId is null)
            {
                continue;
            }

            var originId = row.numero.Value.ToString();
            var issuedAt = row.fecha ?? DateTime.UtcNow;

            const string sql = @"
                INSERT INTO accounting_receivables (customer_id, origin_type, origin_id, issued_at, amount, balance, status)
                VALUES (@customerId, 'gasto', @originId, @issuedAt, 0, 0, 'open')
                ON CONFLICT (origin_type, origin_id) DO UPDATE SET
                    customer_id = EXCLUDED.customer_id,
                    issued_at = EXCLUDED.issued_at,
                    updated_at = now();";

            await context.Postgres.ExecuteAsync(sql, new { customerId, originId, issuedAt }, context.Transaction);
        }

        await context.WriteSyncLogAsync("cuentas_por_cobrar_gastos", rows.Count());
    }

    private static async Task VerifyTotalsAsync(EtlContext context)
    {
        Console.WriteLine("Ejecutando validaciones de verificación (modo verify)...");
        var accessTotals = await context.Access.QuerySingleAsync<AggregateRow>("SELECT COUNT(*) AS cantidad, 0 AS total FROM facturas");
        var postgresTotals = await context.Postgres.QuerySingleAsync<AggregateRow>("SELECT COUNT(*) AS cantidad, COALESCE(SUM(total),0) AS total FROM sales_invoices", transaction: context.Transaction);

        var payload = new
        {
            accessTotals.cantidad,
            accessTotals.total,
            postgresTotals.cantidad,
            postgresTotals.total,
            differenceCount = accessTotals.cantidad - postgresTotals.cantidad,
            differenceTotal = accessTotals.total - postgresTotals.total
        };

        Console.WriteLine(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
        await context.WriteSyncLogAsync("verify", 0, payload);
    }

    private static decimal SafeDecimal(double? value) => value.HasValue ? Convert.ToDecimal(value.Value) : 0m;
    private static decimal SafeDecimal(decimal? value) => value ?? 0m;

    private static decimal SafeDecimal(object? value)
    {
        if (value is null || value is DBNull)
        {
            return 0m;
        }

        return Convert.ToDecimal(value);
    }
}

internal enum LoadMode
{
    Full,
    Delta,
    Verify
}

internal sealed record EtlSettings
{
    public string AccessConnectionString { get; init; } = string.Empty;
    public string PostgresConnectionString { get; init; } = string.Empty;
}

internal sealed class EtlContext
{
    public EtlContext(EtlSettings settings, OleDbConnection access, NpgsqlConnection postgres, NpgsqlTransaction transaction)
    {
        Settings = settings;
        Access = access;
        Postgres = postgres;
        Transaction = transaction;
    }

    public EtlSettings Settings { get; }
    public OleDbConnection Access { get; }
    public NpgsqlConnection Postgres { get; }
    public NpgsqlTransaction Transaction { get; }

    public Task<int> WriteSyncLogAsync(string entity, int processedRows, object? payload = null)
    {
        const string sql = @"
            INSERT INTO sync_log (entity_name, entity_id, operation, payload, origin, processed)
            VALUES (@entity, NULL, 'insert', CAST(@payload AS jsonb), 'access', @processed);";

        var json = payload is null ? null : JsonSerializer.Serialize(payload);
        return Postgres.ExecuteAsync(sql, new { entity, payload = json, processed = processedRows > 0 }, Transaction);
    }
}

internal sealed record TaxRow(string? codigo, string? impuesto, double? porcentaje);
internal sealed record ProductRow(string? codigo, string? descripcion, string? marca, string? modelo, double? precio_compra, double? precio_venta, double? iva, double? impuesto2, double? impuesto3, double? cantidad_minima, string? codigo_referencia);
internal sealed record PriceListRow(string? nombre_lista_precios, string? codigo_producto, double? precio_venta);
internal sealed record CustomerRow(string? codigo, string? nit, string? nombre, string? razon_social, string? e_mail, string? telefono, string? celular, string? direccion, string? ciudad, string? departamento, double? saldo_puntos_acumulados, double? cupo_maximo_credito, string? lista_precios_productos, string? tipo_documento);
internal sealed record WarehouseRow(string? codigo, string? descripcion);
internal sealed record InvoiceRow(long? numero, string? codigo_cliente, DateTime? fecha, string? anulada = null);
internal sealed record InvoiceTotalRow(long? numero, double? subtotal, double? descuento, double? impuestos);
internal sealed record InvoiceItemRow(long? item, long? num_factura, string? cod_producto, double? cantidad, double? valor_unitario, double? descuento, double? impuesto, double? impuesto2, double? impuesto3);
internal sealed record PaymentRow(long? numero_documento, string? tipo_documento, double? total_efectivo, double? total_tarjeta_credito, double? total_tarjeta_debito, double? total_cheque, double? total_bonos);
internal sealed record AggregateRow(int cantidad, decimal total);
