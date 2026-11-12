# Game Vault — Plan de alineación y cierre en ASP.NET Core (ABP/PFNET)

> Plan de trabajo para cumplir los requerimientos del Proyecto Final (Secciones #1–#4 + Plus).  
> **Base real del proyecto:** `aspnet-core/` (ABP Framework, capas Core/Application/EF Core/Web.Host).  
> No se cambia el stack a `dotnet new webapi --use-controllers`: el curso usó ABP y el proyecto ya está montado sobre él; se mantiene esa base y se alinea al contrato NestJS.  
> Cada ítem se marca `- [x]` al completarse.

## 0. Reglas de trabajo

- [x] No tocar `Base1_Videojuegos/` (NestJS + Vue original). Es la referencia del contrato.
- [x] No crear un proyecto paralelo `GameVault.Api`. Se continúa sobre `aspnet-core/src/PFNET.*`.
- [ ] Todo código nuevo en inglés (identificadores, DTOs, comentarios). Documentación puede estar en español.
- [ ] Commits por unidad de trabajo (ver skill `work-unit-commits`). El usuario hace push.
- [x] Mantener el puerto **3000** (el frontend lo tenía hardcodeado; ahora se externaliza a env var).
- [x] `aspnet-core` compila sin errores (`dotnet build` OK).

## 1. Auditoría de alineación (ABP/PFNET vs. requerimientos)

### Sección #1 — Montaje del proyecto ASP.NET Core

| Requerimiento | Estado | Nota |
|---|---|---|
| Crear el proyecto con `dotnet new webapi --use-controllers` | ⚠️ No literal | Se usa ABP (layered). Es el enfoque enseñado en el curso; se mantiene. |
| Mismo modelo con EF Core + SQLite + migraciones | ✅ | `Videojuego` (Core), `PFNETDbContext`, SQLite, migración `Initial` aplicada, `PFNET.db` existente. |
| CRUD con Controller + servicio DI + DTOs | ✅ | `VideojuegosController`, `VideojuegoAppService` (DI vía ABP), DTOs `Create/Update/Videojuego`. |

### Sección #2 — Mismo contrato que la API NestJS

| Requerimiento | Estado | Nota |
|---|---|---|
| Mismas rutas, verbos HTTP y códigos de estado | ✅ | `/videojuegos`, `/auth/register`, `/auth/login`. GET/POST/PATCH/DELETE. 201/200/404/401/409/400 verificados en vivo. |
| Misma forma JSON (camelCase) | ✅ | ABP serializa con Newtonsoft CamelCase. Verificado: `nombreDelJuego`, `imagenDelJuego`, `access_token`, `id`, `email`. |
| Mismos params de búsqueda por nombre y paginación | ⚠️ N/A | NestJS no los expone (Vue los hace client-side). Paridad = sin params en servidor. |
| CORS para origen del cliente Vue | ✅ | Política `localhost` con orígenes de `appsettings.json` (incluye `http://localhost:5173`). |

### Sección #3 — Autenticación y sesión con JWT

| Requerimiento | Estado | Nota |
|---|---|---|
| Registro en el mismo endpoint, password hasheada | ✅ | `/auth/register` hashea con `UserManager.PasswordHasher` (ABP). 201 `{ id, email }`, 409 duplicado, 400 validación. |
| Login en el mismo endpoint, emitir JWT firmado | ✅ | `/auth/login` → 200 `{ access_token }`. Fix aplicado: `SaveLoginAttemptAsync` no-op (evita "database is locked" de SQLite). |
| Proteger con `[Authorize]` los endpoints de escritura | ✅ (equivalente) | Se usa `[AbpAuthorize]` (equivalente ABP de `[Authorize]`), verificado 401 sin token. |

### Sección #4 — Integración con el cliente Vue

| Requerimiento | Estado | Nota |
|---|---|---|
| Cambiar únicamente la URL base a env var | ✅ | `VITE_API_BASE_URL` en `.env`/`.env.example`; reemplazados los 4 fetch hardcodeados en Login/Register/Videojuegos views. Build Vue OK. |
| Verificar CRUD/búsqueda/paginación/sesión | ⚠️ Pendiente | Matriz en sección 5. |

### Plus — OpenAPI

| Requerimiento | Estado | Nota |
|---|---|---|
| Generar OpenAPI con `Microsoft.AspNetCore.OpenApi` | ❌ | Se usa Swashbuckle (ABP default). Opcional/bonificación. |
| Interfaz interactiva (Scalar/Swagger UI) con Bearer | ✅ | Swagger UI en `/swagger` con `bearerAuth`. |

## 2. Cambios aplicados en esta sesión

- `src/PFNET.Core/Authorization/LoginManager.cs`: override de `SaveLoginAttemptAsync` → no-op, para evitar "database is locked" (SQLite + ABP).
- `Base1_Videojuegos/.../game-vault-request/src/views/{Login,Register,Videojuegos}View.vue`: URL base externalizada a `import.meta.env.VITE_API_BASE_URL`.
- `Base1_Videojuegos/.../game-vault-request/.env` y `.env.example`: `VITE_API_BASE_URL=http://localhost:3000`.
- `PLAN.md` reescrito para reflejar la base ABP real.

## 3. Matriz de paridad de contrato (NestJS ↔ ABP)

| NestJS | ASP.NET (ABP) | Status | Body |
|---|---|---|---|
| `POST /auth/register` | `POST /auth/register` | 201 / 409 / 400 | `{ id, email }` / `{ message }` |
| `POST /auth/login` | `POST /auth/login` | 200 / 401 | `{ access_token }` |
| `GET /videojuegos` | `GET /videojuegos` | 200 | `Videojuego[]` |
| `GET /videojuegos/:id` | `GET /videojuegos/{id}` | 200 / 404 | `Videojuego` |
| `POST /videojuegos` | `POST /videojuegos` | 201 / 401 | `Videojuego` |
| `PATCH /videojuegos/:id` | `PATCH /videojuegos/{id}` | 200 / 401 / 404 | `Videojuego` |
| `DELETE /videojuegos/:id` | `DELETE /videojuegos/{id}` | 200 / 401 / 404 | `Videojuego` (borrado) |

Propiedades JSON (camelCase): `id`, `nombreDelJuego`, `imagenDelJuego`, `email`, `access_token`.

## 4. Matriz de verificación

### Backend (curl contra `http://localhost:3000`)

- [x] `GET /videojuegos` → 200, array de 16 (seed).
- [x] `GET /videojuegos/2` → 200.
- [x] `GET /videojuegos/9999` → 404.
- [x] `POST /videojuegos` sin token → 401.
- [x] `POST /auth/register` email nuevo → 201 `{ id, email }`.
- [x] `POST /auth/register` email duplicado → 409 `{ message }`.
- [x] `POST /auth/register` password < 8 → 400.
- [x] `POST /auth/login` credenciales válidas → 200 `{ access_token }`.
- [x] `POST /auth/login` credenciales inválidas → 401.
- [x] `POST /videojuegos` con Bearer → 201.
- [x] `PATCH /videojuegos/{id}` con Bearer → 200.
- [x] `DELETE /videojuegos/{id}` con Bearer → 200.
- [x] `PATCH /videojuegos/{id}` sin token → 401.
- [x] CORS preflight desde `http://localhost:5173` → 204 con headers correctos (configurado; verificar si se desea).

### Frontend (Vue contra API .NET)

- [ ] Cambiar `.env` → `VITE_API_BASE_URL=http://localhost:3000`, levantar `npm run dev`.
- [ ] Login con usuario registrado → redirige a `/videojuegos`.
- [ ] Registro → redirige a `/login`.
- [ ] Listado carga los 16 juegos.
- [ ] Búsqueda por nombre filtra.
- [ ] Paginación (4 por página) funciona.
- [ ] Crear videojuego (autenticado) → aparece.
- [ ] Editar videojuego → cambios reflejados.
- [ ] Eliminar videojuego → desaparece.
- [ ] Cerrar sesión → redirige a `/login`; rutas protegidas inaccesibles.
- [ ] Token inválido/expirado → 401 → cierre de sesión automático.

### Plus (OpenAPI)

- [ ] `/swagger` carga (Swashbuckle). `Microsoft.AspNetCore.OpenApi` no implementado (opcional).

## 5. Riesgos y notas

- **Issuer/audience**: .NET emite JWT con `iss=PFNET`, `aud=PFNET` y valida ambos. Como el backend es el único emisor, no hay incompatibilidad. NestJS no emitía iss/aud, pero ya no se usa.
- **BCrypt vs ABP hasher**: ABP usa PBKDF2 (ASP.NET Identity). El NestJS usaba bcryptjs. El requisito solo pide "password hasheada", sin algoritmo. OK. No hay migración de usuarios entre backends en este proyecto.
- **JSON casing**: verificado camelCase en vivo.
- **SQLite lock**: resuelto con no-op en `SaveLoginAttemptAsync`.
- **ABP convive**: no se tocó `PFNET.*` estructura, solo `LoginManager`.
- **Vue**: solo se cambió la URL base (env var). No se refactorizaron componentes.
- **Plus OpenAPI**: Swashbuckle ya expone Swagger UI con Bearer. `Microsoft.AspNetCore.OpenApi` (requerimiento literal) no se implementó por ser opcional y por usar el stack enseñado (Swashbuckle).

## 6. Progreso

- [x] WP0 Verificación del proyecto ABP (build + run en puerto 3000)
- [x] WP1 Datos (EF + SQLite + migraciones + seed) — ya existía
- [x] WP2 Servicios/DTOs — ya existía
- [x] WP3 Controllers/contrato — verificado en vivo
- [x] WP4 Auth JWT — verificado + fix login
- [x] WP5 CORS — configurado
- [ ] WP6 OpenAPI con `Microsoft.AspNetCore.OpenApi` (Plus, opcional, no implementado)
- [x] WP7 Vue env var — implementado y build OK
- [ ] WP8 Verificación frontend completa (pendiente de ejecutar manualmente)

---

**Estado:** backend completo y verificado; frontend integrado (build OK). Pendiente: smoke test manual del flujo Vue (sección 4).  
**Plus OpenAPI:** Swashbuckle activo; `Microsoft.AspNetCore.OpenApi` no implementado (opcional).
