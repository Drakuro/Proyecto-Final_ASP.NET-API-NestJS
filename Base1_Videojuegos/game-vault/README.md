# Game Vault

Aplicación de catálogo de videojuegos compuesta por una API REST y un frontend web. El proyecto está organizado como un monorepo con dos aplicaciones independientes:

| Carpeta | Aplicación | Descripción |
|---|---|---|
| `game-vault-query-api` | API (backend) | API REST en NestJS con autenticación JWT y base de datos SQLite. |
| `game-vault-request` | Frontend (web) | SPA en Vue 3 + TypeScript + Vite que consume la API. |

## Stack tecnológico

**API (`game-vault-query-api`)**

- [NestJS](https://nestjs.com) 12 — framework para aplicaciones server-side en Node.js
- [Prisma](https://www.prisma.io) 7 con SQLite (adapter `better-sqlite3`)
- [JWT](https://jwt.io) (`@nestjs/jwt`) para autenticación por tokens
- [bcryptjs](https://github.com/dcodeIO/bcrypt.js) para el hash de contraseñas
- `class-validator` y `class-transformer` para validación de datos de entrada
- Jest para pruebas unitarias y e2e

**Frontend (`game-vault-request`)**

- [Vue](https://vuejs.org) 3 (Composition API / `<script setup>`)
- [Vue Router](https://router.vuejs.org) para navegación y guard de autenticación
- [Vite](https://vite.dev) 8 como bundler de desarrollo

## Requisitos previos

- Node.js 20+
- npm

## Puesta en marcha

### 0. Verificar los requisitos previos

Confirma que tienes Node.js 20+ y npm instalados:

```bash
node -v
npm -v
```

Si no los tienes, descarga e instala Node.js desde [nodejs.org](https://nodejs.org) (incluye npm). La Prisma CLI no necesita instalación global: el proyecto la usa con `npx prisma`. Si prefieres tenerla global: `npm install -g prisma`.

### 1. Instalar las dependencias de la API (`game-vault-query-api`)

Todas las dependencias de la API están declaradas en `game-vault-query-api/package.json` (NestJS, Prisma Client, JWT, bcryptjs, class-validator, etc.). Instálalas con:

```bash
cd game-vault-query-api
npm install
```

Este comando instala las dependencias de producción y de desarrollo, y crea la carpeta `node_modules`. Verifica que la descarga termine sin errores.

Genera el cliente de Prisma (necesario para que la app pueda conectarse a la base de datos):

```bash
npx prisma generate
```

### 2. Configurar las variables de entorno

Copia el archivo de ejemplo a `.env`:

```bash
cp .env.example .env
```

En Windows (PowerShell): `Copy-Item .env.example .env`.

Edita `.env` y define un `JWT_SECRET` aleatorio (el valor del ejemplo es solo un placeholder, no debe usarse en producción). Puedes generarlo con:

```bash
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
```

### 3. Preparar la base de datos

Aplica las migraciones de `prisma/migrations/` (crea `prisma/dev.db` con las tablas `User` y `Videojuego`):

```bash
npx prisma migrate dev
```

Carga los datos iniciales de videojuegos (opcional, pero recomendado la primera vez):

```bash
npm run db:seed
```

### 4. Instalar las dependencias del frontend (`game-vault-request`)

```bash
cd game-vault-request
npm install
```

Instala Vue 3, Vue Router y las herramientas de desarrollo (Vite, TypeScript, vue-tsc), declaradas en `game-vault-request/package.json`.

Con la instalación de dependencias y el entorno configurado, ya puedes ejecutar el proyecto.

## Ejecutar el proyecto

Necesitas dos terminales: una para la API y otra para el frontend.

### 1. API (`game-vault-query-api`)

```bash
cd game-vault-query-api
npm run start:dev        # modo desarrollo con recarga automática ante cambios
```

Alternativas:

```bash
npm run start            # modo desarrollo sin recarga
npm run build            # compilar a producción (nest build)
npm run start:prod       # ejecutar la compilación de producción
```

La API queda disponible en `http://localhost:3000`. Para verificar que responde, prueba `GET http://localhost:3000/videojuegos`.

### 2. Frontend (`game-vault-request`)

```bash
cd game-vault-request
npm run dev
```

El frontend queda disponible en la URL que muestre Vite (por defecto `http://localhost:5173`). Accede al catálogo en `/videojuegos`, regístrate en `/registro` o inicia sesión en `/login`.

## Variables de entorno (API)

Copiar `game-vault-query-api/.env.example` a `.env` y ajustar los valores. **Nunca subir el archivo `.env` real a git** (ya está en `.gitignore`).

| Variable | Descripción | Valor de ejemplo |
|---|---|---|
| `DATABASE_URL` | Ruta del archivo de base de datos SQLite. | `file:./prisma/dev.db` |
| `JWT_SECRET` | Secreto para firmar/verificar tokens JWT. Genera uno aleatorio de 64 caracteres hex (ver Puesta en marcha, paso 2). | `change_me_to_a_random_64_char_hex_string` |
| `JWT_EXPIRES_IN` | Tiempo de vida del token (formato `ms`). Por defecto: 7 días. | `7d` |

## Base de datos y datos iniciales

La API usa **SQLite** (archivo `game-vault-query-api/prisma/dev.db`). El esquema se define en `prisma/schema.prisma` e incluye dos modelos:

- **`User`** — `id`, `email` (único), `password` (hash bcrypt).
- **`Videojuego`** — `id`, `nombreDelJuego`, `imagenDelJuego`.

El seed (`prisma/seed.ts`, ejecutado con `npm run db:seed`) carga **16 videojuegos** de Steam si la tabla está vacía (es idempotente: no duplica datos). No crea usuarios.

Las migraciones se encuentran en `prisma/migrations/`.

## Endpoints de la API

Prefijo base: `http://localhost:3000`

### Autenticación `/auth`

| Método | Ruta | Protegido | Descripción |
|---|---|---|---|
| `POST` | `/auth/register` | No | Crea un usuario con `{ email, password }` (password mín. 8 caracteres). Devuelve el usuario sin la contraseña. |
| `POST` | `/auth/login` | No | Valida credenciales y devuelve `{ access_token }`. |

### Videojuegos `/videojuegos`

| Método | Ruta | Protegido | Descripción |
|---|---|---|---|
| `GET` | `/videojuegos` | No | Lista todos los videojuegos. |
| `GET` | `/videojuegos/:id` | No | Devuelve un videojuego por ID (404 si no existe). |
| `POST` | `/videojuegos` | Sí | Crea un videojuego con `{ nombreDelJuego, imagenDelJuego }` (201). |
| `PATCH` | `/videojuegos/:id` | Sí | Actualiza parcialmente un videojuego. |
| `DELETE` | `/videojuegos/:id` | Sí | Elimina un videojuego (404 si no existe). |

Los endpoints protegidos requieren el header:

```
Authorization: Bearer <access_token>
```

### Flujo de autenticación

1. `POST /auth/register` para crear el usuario.
2. `POST /auth/login` para obtener el `access_token`.
3. Enviar el token con `Authorization: Bearer <token>` en las operaciones de escritura de videojuegos.

## Estructura del código

### API — `game-vault-query-api/src`

```
src/
├── main.ts                 # Bootstrap: CORS, ValidationPipe global, puerto
├── app.module.ts           # Módulo raíz: importa Prisma, Videojuegos y Auth
├── prisma.service.ts       # Cliente de Prisma (conexión a SQLite)
├── prisma.module.ts        # Módulo global que expone PrismaService
├── auth/                   # Autenticación: JWT + bcrypt
│   ├── auth.controller.ts  #   POST /auth/register, POST /auth/login
│   ├── auth.service.ts     #   Lógica de registro y login, firma JWT
│   ├── auth.guard.ts       #   Guard que protege rutas con Bearer token
│   └── dto/                #   register.dto.ts, login.dto.ts
├── users/
│   └── users.service.ts    # Persistencia de usuarios (uso interno de Auth)
└── videojuegos/            # CRUD principal
    ├── videojuegos.controller.ts
    ├── videojuegos.service.ts
    ├── entities/videojuego.entity.ts
    └── dto/                # create-videojuego.dto.ts, update-videojuego.dto.ts
```

### Frontend — `game-vault-request/src`

```
src/
├── main.ts                 # Punto de entrada de la SPA
├── App.vue                 # Componente raíz
├── router/index.ts         # Rutas + guard de autenticación
├── views/
│   ├── LoginView.vue       # Inicio de sesión
│   ├── RegisterView.vue    # Registro de usuario
│   └── VideojuegosView.vue # Catálogo/gestión de videojuegos
└── style.css               # Estilos globales
```

Rutas del frontend: `/` redirige a `/videojuegos`; `/login` y `/registro` son públicas; `/videojuegos` requiere token (`localStorage.access_token`), si no hay sesión redirige a `/login`.

## Scripts útiles

**API (`game-vault-query-api`)**

| Script | Acción |
|---|---|
| `npm start` / `npm run start:dev` / `npm run start:prod` | Levantar la API |
| `npm run build` | Compilar con `nest build` |
| `npm run lint` | Análisis estático con oxlint |
| `npm run format` | Formatear con Prettier |
| `npm run db:seed` | Cargar datos iniciales de videojuegos |

**Frontend (`game-vault-request`)**

| Script | Acción |
|---|---|
| `npm run dev` | Servidor de desarrollo con recarga |
| `npm run build` | Compilación para producción |
| `npm run preview` | Previsualizar el build de producción |