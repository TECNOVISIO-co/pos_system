# Plan de Migración Access → PostgreSQL

Este documento describe la estrategia de migración progresiva desde la base de datos Access `bd1.mdb` hacia el nuevo esquema PostgreSQL definido en `/db/00_create_schema.sql`.

## 1. Preparación

1. **Inventario de objetos**: ejecutar `python3 scripts/generate_data_dictionary.py` para obtener el diccionario actualizado.
2. **Validación de datos**: con `mdb-export` generar conteos por tabla y totales críticos (ventas, abonos) para servir de baseline.
3. **Infraestructura**: aprovisionar PostgreSQL 16 con extensiones `pgcrypto` y GIN habilitados. Configurar roles con acceso restringido.
4. **Configuración de conectores**:
   - En un host Windows, instalar el proveedor ACE OLEDB 2016 para acceso a `bd1.mdb`.
   - Registrar credenciales de PostgreSQL en el almacén seguro (`dotnet user-secrets` para la API o variables en pipeline de ETL).

## 2. Estrategia de Sincronización

### 2.1 Carga Inicial

1. Ejecutar el script `/db/00_create_schema.sql` seguido de `/db/01_indexes.sql` y `/db/02_seed_minima.sql` en un esquema vacío.
2. Correr el ETL `dotnet run --project etl/etl_access_to_pg.csproj` en modo `FullLoad`, que:
   - Lee las tablas clave (`productos`, `clientes`, `impuestos`, `listas_precios_productos`, `facturas`, `detalle_factura`, `detalle_pago`, `cuentas_por_cobrar_gastos`, `detalle_cuentas_por_cobrar_gastos`, `bodegas`, `detalle_producto_bodega`, `usuarios`, `vendedores`, `movimientos`).
   - Limpia y mapea los datos hacia el nuevo modelo (`catalog_*`, `crm_customers`, `sales_*`, `inventory_*`, `accounting_*`).
   - Registra el resultado de cada lote en `sync_log` con `origin = 'access'`.

### 2.2 Cargas Incrementales

- Utilizar las columnas de fecha disponibles (`fecha`, `ultima_modificacion`, `actualizado`, según tabla). Cuando no existan, emplear la marca de tiempo del ETL y comparar con un hash calculado por fila.
- `sync_log` almacenará la última marca de cada tabla: `{ entity_name, last_access_id, last_access_timestamp }`.
- Incrementales programados cada 15 minutos mientras Access siga activo.

### 2.3 Estrategia Dual-Write (Opcional)

- Durante la fase de convivencia, exponer una capa de servicios en la API .NET que escriba en PostgreSQL y replique los cambios relevantes hacia Access mediante el conector OLEDB.
- Para minimizar riesgos, limitar el dual-write a entidades maestras (`productos`, `clientes`). Las transacciones de ventas permanecen en Access hasta el cutover.

## 3. Cutover Final

1. Programar ventana fuera de horario. Comunicar congelamiento de operaciones en Access.
2. Tomar respaldo completo de `bd1.mdb` y de la instancia PostgreSQL.
3. Deshabilitar accesos de escritura al sistema legado (solo lectura para verificaciones).
4. Ejecutar ETL en modo `DeltaFinal` para replicar últimos cambios pendientes.
5. Validar:
   - Conteos por tabla entre Access y PostgreSQL.
   - Sumas de totales (`subtotal`, `impuestos`, `total` en facturas; `amount` en pagos).
   - Muestreo aleatorio ≥1% de registros para revisar integridad.
   - Intentos de violación de FK/CHECK en PostgreSQL (deben fallar).
6. Actualizar la API y la app Flutter para apuntar a PostgreSQL.
7. Monitorear logs y métricas durante las primeras 24 horas.

## 4. Plan de Contingencia

- **Rollback inmediato**: restaurar la cadena de conexión de la API y aplicaciones hacia Access y reprocesar los deltas guardados en `sync_log` cuando el problema se resuelva.
- **Fallback parcial**: permitir que únicamente los procesos críticos (facturación) regresen a Access mientras se mantiene PostgreSQL en modo sólo lectura.
- **Reprocesamiento**: todos los lotes ETL generan archivos CSV de respaldo (`/backups/access_export/<tabla>_<timestamp>.csv`). En caso de corrupción, se puede reimportar desde dichos archivos.

## 5. Roles y Responsables

| Rol | Responsabilidades |
| --- | --- |
| Arquitectura | Aprobar modelo de datos, políticas de integridad y estrategia de sincronización. |
| DBA | Crear objetos en PostgreSQL, tunear índices, ejecutar validaciones de integridad. |
| Equipo ETL | Mantener el pipeline `etl_access_to_pg`, monitorear logs, ejecutar cargas incrementales/finales. |
| QA | Diseñar y ejecutar pruebas de conteo, sumatorias y validaciones de negocio. |
| Operaciones | Coordinar ventana de cutover, respaldos y rollback. |

## 6. Próximos Pasos

- Automatizar la ejecución del ETL en CI (GitHub Actions) para validar que los mapeos continúan funcionando.
- Documentar en `/docs/04_api_contracts.yaml` los contratos expuestos por la API moderna (fase 1).
- Integrar chequeos automáticos (`dotnet test`, `dotnet format`) en el pipeline antes de liberar nuevas versiones.
