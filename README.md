# Tienda de servicios — APIs .NET

Dos servicios ASP.NET Core 8 con PostgreSQL: **AuthService** gestiona cuentas y JWT; **ProductService** gestiona catálogo, categorías y reseñas. El frontend vive en `TiendaServicios-FRONTEND`.

## Arranque local

Requiere .NET SDK 8 y dos bases PostgreSQL configuradas según cada contexto EF Core.

Configura en cada proceso:
- `ConnectionStrings__DefaultConnection`: conexión de su base de datos.
- `Jwt__Secret`: secreto robusto compartido por ambos servicios.
- `Jwt__Issuer` y `Jwt__Audience`: iguales en ambos procesos.
- `Cors__AllowedOrigins__0`: origen exacto del frontend (sin ruta ni barra final).

Usa variables del entorno o `dotnet user-secrets` en desarrollo. No copies credenciales reales a los archivos versionados.

```sh
dotnet restore AuthService/AuthService.csproj
dotnet restore ProductService/ProductService.csproj
dotnet run --project AuthService --urls http://localhost:5003
# En otra terminal:
dotnet run --project ProductService --urls http://localhost:5174
```

En desarrollo se admite por defecto `http://localhost:3000`. En producción, las llamadas de navegador desde otros orígenes requieren configurar esta lista; si está vacía se mantiene deshabilitado CORS sin impedir el arranque de la API. No se admite un comodín con credenciales.

AuthService requiere aplicar sus migraciones antes de usarlo. Ambos servicios incluyen migraciones, pero ProductService todavía llama a `EnsureCreated()` al arrancar: hay que unificar esa estrategia antes de operar una base persistente. No mezcles creación automática y migraciones sobre datos reales sin revisar el esquema.

## Contratos y permisos

| Servicio | Rutas base | Acceso |
| --- | --- | --- |
| AuthService | `/api/auth/register`, `/api/auth/login` | registro e inicio públicos |
| AuthService | `/api/user` | consultar atributos de autorización por acción |
| ProductService | `/api/products`, `/api/categories`, `/api/reviews` | lecturas y escrituras según cada controlador |

Swagger está habilitado en desarrollo. El registro público siempre crea el rol **Cliente**, aunque un cliente envíe `Role: "Admin"`. Los cambios de rol pertenecen a las acciones administrativas autorizadas. Se valida longitud de contraseña y formato de campos antes de ejecutar el controlador.

Los filtros del catálogo aceptan páginas desde 1 y tamaños de 1 a 100. Los rangos de precio invertidos o negativos se rechazan con 400.

## Verificación

```sh
dotnet build AuthService/AuthService.csproj
dotnet build ProductService/ProductService.csproj
dotnet run --project tests/RegressionChecks
```

Las regresiones usan EF Core InMemory, no conexiones reales. Cubren escalamiento de rol en registro, contraseña incorrecta, hashing y filtros inválidos. No sustituyen pruebas de autorización HTTP ni restricciones de PostgreSQL.

## Pendientes

Añadir pruebas de integración, índices únicos para evitar registros simultáneos duplicados, rate limiting de login, migraciones consistentes, manejo uniforme de errores y despliegue reproducible. Revisar y rotar fuera de Git cualquier secreto de configuraciones históricas antes de publicar una demo. Los archivos ZIP versionados son copias históricas; el código ejecutable está en las carpetas de los servicios.
