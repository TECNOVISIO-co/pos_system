# SalesSwift POS Modernisation

Este repositorio agrupa los artefactos iniciales para modernizar el sistema POS heredado basado en Microsoft Access hacia una arquitectura compuesta por PostgreSQL, una API ASP.NET Core y un cliente Flutter multiplataforma.

## Estructura

```
docs/                    Documentación funcional y técnica
  01_data_dictionary.md  Diccionario generado automáticamente a partir de bd1.mdb
  03_migration_plan.md   Plan de migración y estrategia de cutover
db/                      Scripts SQL para el nuevo modelo de datos en PostgreSQL
etl/                     Proyecto .NET (console) que orquesta el ETL Access → PostgreSQL
backend/                 Solución ASP.NET Core (Dominio, Infraestructura, API y pruebas)
scripts/                 Utilidades auxiliares (p. ej. generación del diccionario)
bd1.mdb                  Base de datos Access legada (solo lectura)
```

## Primeros pasos

1. **Diccionario de datos**: `python3 scripts/generate_data_dictionary.py`.
2. **Crear esquema moderno**: ejecutar los scripts de `/db` en PostgreSQL (`00_create_schema.sql`, `01_indexes.sql`, `02_seed_minima.sql`).
3. **Configurar ETL**:
   - Ajustar `etl/appsettings.json` con las cadenas de conexión reales.
   - Instalar el proveedor ACE OLEDB 2016 en el host Windows que ejecutará el ETL.
   - Instalar .NET 8 SDK y ejecutar `dotnet restore`/`dotnet run --project etl/etl_access_to_pg.csproj`.
4. **Backend API (ASP.NET Core)**:
   - Ubicarse en `/backend` y ejecutar `dotnet restore Pos.sln`.
   - Ajustar `Pos.Api/appsettings.Development.json` (cadenas de conexión PostgreSQL, CORS y parámetros JWT).
   - Ejecutar `dotnet build Pos.sln` y `dotnet test Pos.sln` para validar la solución.
   - Iniciar la API con `dotnet run --project Pos.Api/Pos.Api.csproj` (requiere PostgreSQL en ejecución).
## Próximas fases

- Extender la API con sincronización bidireccional, control de inventario avanzado y reportes.
- Crear la aplicación Flutter (`flutter_pos`) siguiendo las fases Web → Escritorio → Móvil.
- Incorporar pipelines CI/CD con `dotnet format`, `dotnet test`, `flutter analyze` y suites de pruebas automatizadas.

Consulte `/docs/03_migration_plan.md` para la estrategia completa de migración.