# Game Vault API — ASP.NET Core (ABP / PFNET)

Backend .NET que reemplaza la API de NestJS del Taller #1, sirviendo los mismos endpoints para que la app Vue funcione sin cambios.

## Stack

- .NET 9 (ASP.NET Core, ABP Framework 10.2.0)
- Entity Framework Core 9.0.5 + SQLite (portabilidad)
- JWT Bearer (auth)
- Swagger / OpenAPI

## Prerrequisitos

- .NET SDK 9.0+ (y runtime 9.0)
- (Opcional) SQL Server LocalDB si se quiere volver a SQL Server
- Node.js 20+ (solo para el frontend Vue, no para la API)

## Dependencias (NuGet)

### PFNET.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Sqlite 9.0.5
- Microsoft.EntityFrameworkCore.Tools 9.0.5
- Microsoft.EntityFrameworkCore.Design 9.0.5

### PFNET.Web.Core
- Microsoft.AspNetCore.Authentication.JwtBearer 9.0.5
- Swashbuckle.AspNetCore 8.1.2
- Abp.AspNetCore 10.2.0
- Abp.ZeroCore 10.2.0
- Abp.AspNetCore.SignalR 10.2.0

### PFNET.Core
- Abp.AutoMapper 10.2.0
- Abp.ZeroCore.EntityFrameworkCore 10.2.0
- Castle.Windsor.MsDependencyInjection 4.1.0

### PFNET.Application
- Abp.AutoMapper (vía PFNET.Core)

## Configuración

Archivo: src/PFNET.Web.Host/appsettings.json

`json
{
  "ConnectionStrings": {
    "Default": "Data Source=C:/.../aspnet-core/PFNET.db"
  },
  "App": {
    "ServerRootAddress": "http://localhost:3000/",
    "CorsOrigins": "http://localhost:4200,http://localhost:8080,http://localhost:8081,http://localhost:3000,http://localhost:5173"
  },
  "Authentication": {
    "JwtBearer": {
      "IsEnabled": "true",
      "SecurityKey": "PFNET_D3A1C608B4BD4141B4FDD3203FF15D91",
      "Issuer": "PFNET",
      "Audience": "PFNET"
    }
  },
  "Kestrel": {
    "Endpoints": { "Http": { "Url": "http://localhost:3000/" } }
  }
}
`

- El migrator usa src/PFNET.Migrator/appsettings.json con la misma cadena de conexión.
- El puerto es **3000** (la Vue hardcodea http://localhost:3000).

## Comandos de ejecución

> **Importante — proyecto de inicio (Visual Studio):** antes de ejecutar, establece el proyecto de inicio segun la accion:
> - Migraciones (add-migration en PMC): establecer **PFNET.EntityFrameworkCore** como proyecto de inicio.
> - Levantar la API: establecer **PFNET.Web.Host** como proyecto de inicio.
> - Ejecutar migraciones: correr **PFNET.Migrator** (app de consola) con Depurar -> Iniciar sin depurar y responder Y.
>
> Con dotnet ef por CLI no hace falta cambiar el proyecto de inicio: se pasan --project y --startup-project.


### 1. Restaurar dependencias
`ash
cd aspnet-core
dotnet restore PFNET.sln
`

### 2. Compilar
`ash
dotnet build PFNET.sln
`

### 3. Crear / aplicar migraciones (primera vez o tras cambios de modelo)
Desde la raíz spnet-core:
`ash
dotnet ef migrations add <Nombre> \
  --project src/PFNET.EntityFrameworkCore/PFNET.EntityFrameworkCore.csproj \
  --startup-project src/PFNET.Web.Host/PFNET.Web.Host.csproj
`

Aplicar migraciones (crea la BD SQLite y ejecuta el seed):
`ash
cd src/PFNET.Migrator
echo Y | dotnet run
`

### 4. Levantar la API
`ash
cd src/PFNET.Web.Host
dotnet run
`
La API queda en http://localhost:3000. Swagger en http://localhost:3000/swagger/index.html.

### 5. Levantar el frontend Vue (sin cambios)
`ash
cd ../Base1_Videojuegos/game-vault/game-vault-request
npm install
npm run dev
`
Frontend en http://localhost:5173.

## Endpoints principales (paridad con NestJS)

Base: http://localhost:3000

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| POST | /auth/register | No | Crea usuario {email, password} (mín. 8 chars). Devuelve {id, email}. |
| POST | /auth/login | No | Devuelve {access_token}. |
| GET | /videojuegos | No | Lista todos (16 seed). |
| GET | /videojuegos/{id} | No | Por ID (404 si no existe). |
| POST | /videojuegos | Bearer | Crea (201). |
| PATCH | /videojuegos/{id} | Bearer | Update parcial. |
| DELETE | /videojuegos/{id} | Bearer | Elimina (404 si no existe). |

Endpoints ABP adicionales (formato envuelto): /api/services/app/Videojuego/*, /api/TokenAuth/Authenticate.

## Usuario de ejemplo
- Email: demo@gamevault.com
- Password: Demo1234


### 6. Comandos utiles adicionales

Detener la API:
```bash
# Ctrl+C en la terminal donde corre dotnet run
```

Reiniciar la base de datos (borra datos y re-ejecuta seed):
```bash
# 1. Detener la API
# 2. Borrar el archivo de BD
rm PFNET.db PFNET.db-shm PFNET.db-wal
# 3. Volver a migrar
cd src/PFNET.Migrator
echo Y | dotnet run
cd ../PFNET.Web.Host
dotnet run
```

Probar registro y login con PowerShell:
```powershell
# Registro
Invoke-WebRequest -Uri "http://localhost:3000/auth/register" -Method Post -ContentType "application/json" -Body '{"email":"demo@gamevault.com","password":"Demo1234"}' -UseBasicParsing

# Login
Invoke-RestMethod -Uri "http://localhost:3000/auth/login" -Method Post -ContentType "application/json" -Body '{"email":"demo@gamevault.com","password":"Demo1234"}'
```

Listar videojuegos:
```powershell
Invoke-RestMethod -Uri "http://localhost:3000/videojuegos" -Method Get
```

Crear un videojuego (con token):
```powershell
$token = (Invoke-RestMethod -Uri "http://localhost:3000/auth/login" -Method Post -ContentType "application/json" -Body '{"email":"demo@gamevault.com","password":"Demo1234"}').access_token
Invoke-WebRequest -Uri "http://localhost:3000/videojuegos" -Method Post -Headers @{Authorization="Bearer $token"} -ContentType "application/json" -Body '{"nombreDelJuego":"Halo 3","imagenDelJuego":"halo3.png"}' -UseBasicParsing
```

Cambiar el puerto (si 3000 esta ocupado):
Editar src/PFNET.Web.Host/appsettings.json -> Kestrel.Endpoints.Http.Url y App.ServerRootAddress, y actualizar la URL en la Vue.

Ver logs en consola: la API imprime logs de EF Core y auth al nivel Debug (configurable en appsettings.json -> Logging).

### 7. Notas de portabilidad
- La BD SQLite (PFNET.db) se crea automaticamente al correr el migrator.
- Si tu compañera usa otra ruta, ajustar ConnectionStrings.Default en appsettings.json (WebHost y Migrator) con una ruta valida (usar / en lugar de \).
- El frontend Vue hardcodea http://localhost:3000; si cambias el puerto de la API, actualiza tambien la Vue.

## Notas
- Base de datos: SQLite (PFNET.db en spnet-core/). Portátil (recomendación del curso).
- Las migraciones y el seed son idempotentes.
- No se modificó el frontend Vue ni el proyecto PFNET original más allá de la integración de Game Vault.



