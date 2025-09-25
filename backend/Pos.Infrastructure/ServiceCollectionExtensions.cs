using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.Infrastructure.Access;
using Pos.Infrastructure.Data;
using Pos.Infrastructure.Repositories;

namespace Pos.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PosDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException("Postgres connection string is not configured");
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__efmigrations_history", "public"));
        });

        services.AddScoped<IAccessConnectionFactory, OleDbAccessConnectionFactory>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ITaxRepository, TaxRepository>();
        services.AddScoped<IPriceListRepository, PriceListRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IWarehouseStockRepository, WarehouseStockRepository>();
        services.AddScoped<ISyncRepository, SyncRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
