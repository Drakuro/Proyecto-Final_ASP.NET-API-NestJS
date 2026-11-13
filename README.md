# Game Vault - API en ASP.NET Core (ABP) + cliente Vue

Migración del backend de **Game Vault** (originalmente NestJS + Prisma) a **ASP.NET Core** usando **ABP Framework**, con **Entity Framework Core + SQLite**, autenticación **JWT** y documentación **OpenAPI** con interfaz **Scalar**. El cliente Vue original se mantiene sin cambios de lógica; solo cambia la URL base de la API (variable de entorno).

## Estructura del repositorio

| Carpeta | Contenido |
|---|---|
| `aspnet-core/` | Backend nuevo (ASP.NET Core + ABP). Proyectos en `aspnet-core/src/PFNET.*` |
| `Base1_Videojuegos/game-vault/game-vault-request/` | Cliente web en Vue 3 (Vite) |
| `Base1_Videojuegos/game-vault/game-vault-query-api/` | API original en NestJS (solo referencia del contrato, no hace falta correrla) |
| `PLAN.md` | Plan de alineación y matriz de verificación |

## Requisitos

- **.NET 9 (SDK o runtime 9.x)**. Verifica con `dotnet --list-runtimes`: debe aparecer `Microsoft.NETCore.App 9.x`. Descarga: https://dotnet.microsoft.com/download/dotnet/9.0
- **Node.js 20.19 o superior** (o 22.12+; Vite 8 no funciona con versiones anteriores) y `npm`, solo para el cliente Vue. Verifica con `node --version`.
- **Git**.
- Un navegador normal (Chrome, Edge) con `localStorage` permitido.

## Puertos que se usan

| Servicio | URL |
|---|---|
| API (ASP.NET) | `http://localhost:3000` |
| Cliente Vue | `http://localhost:5173` |

El backend solo permite CORS desde los orígenes definidos en `aspnet-core/src/PFNET.Web.Host/appsettings.json` (incluye `http://localhost:5173`). Si el puerto 3000 o el 5173 están ocupados, cierra el programa que los usa antes de continuar.

## Paso a paso

### 1. Clonar el repositorio

```
git clone https://github.com/Drakuro/Proyecto-Final_ASP.NET-API-NestJS.git
cd Proyecto-Final_ASP.NET-API-NestJS
```

### 2. Crear la base de datos con las migraciones (Migrator)

La base de datos es un archivo SQLite que **no se sube al repositorio**: cada persona crea la suya con el Migrator. La cadena de conexión usa una **ruta relativa** (`Data Source=../../PFNET.db`), por lo que el archivo queda en `aspnet-core/PFNET.db`.

> **Importante:** como la ruta es relativa, `dotnet run` debe ejecutarse **dentro** de la carpeta de cada proyecto (`PFNET.Migrator` y `PFNET.Web.Host`), tal como se indica abajo. Si se ejecuta desde otra carpeta, la base de datos se crea en otro lugar.

```
cd aspnet-core/src/PFNET.Migrator
dotnet run
```

Cuando pregunte `Continue to migration for this host database and all tenants..? (Y/N):` escribe **`Y`** y Enter (con solo Enter se cancela).

Debes ver:

```
HOST database migration completed.
All databases have been migrated.
```

Pulsa Enter para salir. Esto crea las tablas (incluida `Videojuegos`). Los 16 videojuegos iniciales los carga el backend al arrancar.

### 3. Levantar el backend

```
cd ../PFNET.Web.Host
dotnet run
```

Debe aparecer `Now listening on: http://localhost:3000`. Deja esta terminal abierta. (Las advertencias `CS1591` de comentarios XML son normales.)

Comprueba que funciona abriendo `http://localhost:3000/videojuegos`: debe mostrar un arreglo JSON con 16 juegos.

### 4. Levantar el cliente Vue (en otra terminal)

Desde la raíz del repositorio:

```
cd Base1_Videojuegos/game-vault/game-vault-request
```

Crea el archivo `.env` copiando `.env.example` (con el mismo contenido):

```
VITE_API_BASE_URL=http://localhost:3000
```

Luego:

```
npm install
npm run dev
```

Abre `http://localhost:5173`. Si cambias la variable `VITE_API_BASE_URL`, reinicia `npm run dev`.

### 5. Usar la aplicación

1. Entra a `/registro` y crea una cuenta (correo válido y contraseña de **8 o más caracteres**).
2. Inicia sesión en `/login`: te redirige a `/videojuegos`.
3. Prueba búsqueda por nombre, paginación (4 por página), y crear, editar y eliminar videojuegos.
4. **Cerrar sesión** borra el token y vuelve a `/login`.

## Documentación de la API (OpenAPI + Scalar)

Con el backend corriendo:

| URL | Qué es |
|---|---|
| `http://localhost:3000/scalar` | Interfaz interactiva **Scalar** |
| `http://localhost:3000/openapi/v1.json` | Documento OpenAPI (`Microsoft.AspNetCore.OpenApi`) |
| `http://localhost:3000/swagger` | Swagger UI (Swashbuckle, el que trae ABP) |

### Probar endpoints protegidos en Scalar

1. Abre `POST /auth/register`, pulsa **Test Request**, escribe el cuerpo y envíalo:
   ```json
   { "email": "prueba@test.com", "password": "Password123" }
   ```
   (La contraseña va **entre comillas** y tiene 8 o más caracteres.)
2. Abre `POST /auth/login` con las mismas credenciales y copia el valor de `access_token`.
3. En el apartado **Authentication → Bearer Token** pega **solo el token** (sin escribir `Bearer `).
4. Prueba `POST`, `PATCH` y `DELETE` de `/videojuegos`. Sin token responden `401`.

## Endpoints

Mismo contrato que la API NestJS original (JSON en `camelCase`):

| Método | Ruta | Auth | Respuestas |
|---|---|---|---|
| `POST` | `/auth/register` | No | `201 { id, email }` · `409` correo repetido · `400` datos inválidos |
| `POST` | `/auth/login` | No | `200 { access_token }` · `401` credenciales incorrectas · `400` datos inválidos |
| `GET` | `/videojuegos` | No | `200` lista |
| `GET` | `/videojuegos/{id}` | No | `200` · `404` |
| `POST` | `/videojuegos` | **JWT** | `201` · `401` |
| `PATCH` | `/videojuegos/{id}` | **JWT** | `200` · `401` · `404` |
| `DELETE` | `/videojuegos/{id}` | **JWT** | `200` · `401` · `404` |

El token JWT dura 1 día y se envía como `Authorization: Bearer <token>`.

## Dónde está cada cosa (backend)

| Tema | Archivo |
|---|---|
| Entidad `Videojuego` | `aspnet-core/src/PFNET.Core/Videojuegos/Videojuego.cs` |
| DbContext + SQLite | `aspnet-core/src/PFNET.EntityFrameworkCore/EntityFrameworkCore/PFNETDbContext.cs`, `PFNETDbContextConfigurer.cs` |
| Migraciones | `aspnet-core/src/PFNET.EntityFrameworkCore/Migrations/` |
| Datos iniciales (16 juegos) | `.../EntityFrameworkCore/Seed/SeedHelper.cs` |
| DTOs y servicio de aplicación | `aspnet-core/src/PFNET.Application/Videojuegos/` |
| Controller de videojuegos | `aspnet-core/src/PFNET.Web.Core/Controllers/VideojuegosController.cs` |
| Registro y login (JWT) | `aspnet-core/src/PFNET.Web.Core/Controllers/AuthController.cs` |
| Configuración JWT | `aspnet-core/src/PFNET.Web.Core/PFNETWebCoreModule.cs`, `PFNET.Web.Host/Startup/AuthConfigurer.cs` |
| CORS, Swagger, OpenAPI y Scalar | `aspnet-core/src/PFNET.Web.Host/Startup/Startup.cs` |
| Esquema Bearer de OpenAPI | `aspnet-core/src/PFNET.Web.Host/Startup/BearerSecuritySchemeTransformer.cs` |
| Configuración (puerto, JWT, CORS, BD) | `aspnet-core/src/PFNET.Web.Host/appsettings.json` |
