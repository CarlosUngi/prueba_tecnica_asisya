# 🚀 Prueba Técnica ASISYA - DEV II

Solución integral Backend (API REST en .NET 8), Frontend (SPA en React + TypeScript) y Persistencia (PostgreSQL + Redis) diseñada bajo principios de **Clean Architecture**, alta escalabilidad y rendimiento optimizado para el procesamiento masivo de datos.

---

## 🔐 Credenciales de Acceso por Defecto (JWT Auth)

Para realizar pruebas en **Swagger UI** (`/swagger`) o desde la pantalla de **Login del Frontend (React)**:

| Usuario | Correo Electrónico | Contraseña por Defecto | Rol |
| :--- | :--- | :--- | :--- |
| **`admin`** | `admin@asisya.com` | **`Admin123!`** | `Admin` |

---

## 🏛️ Arquitectura del Sistema

### Backend (.NET 8 Clean Architecture)
```text
                     ┌─────────────────────────────────────────┐
                     │          Presentation (Api)             │
                     │  (Controllers, Middlewares, Swagger)   │
                     └────────────────────┬────────────────────┘
                                          │
                                          ▼
                     ┌─────────────────────────────────────────┐
                     │              Application                │
                     │    (DTOs, UseCases, Services, Maps)    │
                     └────────────────────┬────────────────────┘
                                          │
                                          ▼
                     ┌─────────────────────────────────────────┐
                     │                Domain                   │
                     │    (Entities, Repository Interfaces)    │
                     └────────────────────▲────────────────────┘
                                          │
                                          │
                     ┌────────────────────┴────────────────────┐
                     │             Infrastructure              │
                     │  (EF Core, PostgreSQL, JWT, Redis)      │
                     └─────────────────────────────────────────┘
```

### Frontend (React + Vite + TypeScript SPA)
```text
frontend/src/
├── components/                # Componentes reutilizables UI (Navbar, Botones)
├── features/
│   ├── auth/                  # Módulo de Autenticación (Context, Login Page)
│   └── products/              # Módulo de Productos (Catálogo, Paginación, Modales)
├── routes/                    # Enrutamiento modular y AuthGuard
└── services/                  # Cliente Axios con interceptor de JWT Bearer Token
```

---

## ⚡ Características Clave

1. **Carga Masiva de 100,000 Productos (PostgreSQL Binary COPY):** El endpoint `POST /api/products/bulk` inserta 100,000 registros en **< 2 segundos**. El Frontend cuenta con un botón interactivo para disparar esta carga en tiempo real.
2. **Frontend React SPA:**
   - Formularios interactivos con validación.
   - Paginación dinámica y búsqueda por texto y categoría (`SERVIDORES` / `CLOUD`).
   - Muestra las imágenes asociadas a cada categoría en la vista de detalle.
   - Interceptor Axios que adjunta el Bearer Token automáticamente a las peticiones y redirige al Login ante `401 Unauthorized`.
   - Protección de rutas privadas con `AuthGuard`.
3. **Integración CI/CD:** GitHub Actions en `.github/workflows/ci.yml` ejecutando `dotnet test`, `vite build` y `docker build`.

---

## 📂 Estructura de Carpetas del Repositorio

```text
prueba_tecnica_asisya/
├── .github/
│   └── workflows/
│       └── ci.yml                     # Pipeline de CI/CD (Build, Test, Lint, Docker)
├── backend/                           # API REST en .NET 8 (Clean Architecture)
│   ├── Api/                           # Controladores REST, Middlewares, Program.cs, Swagger
│   ├── Application/                   # Casos de Uso, DTOs y Mapeos
│   ├── Domain/                        # Entidades del sistema e Interfaces de Repositorios
│   ├── Infrastructure/                # Persistencia (EF Core, Npgsql), JWT Auth y Redis
│   ├── tests/                         # Cobertura de pruebas unitarias e integración
│   ├── Dockerfile                     # Contenedor multietapa para la API de .NET 8
│   └── Asisya.sln                     # Solución completa del Backend (.NET 8)
├── frontend/                          # SPA en React + Vite + TypeScript
│   ├── src/                           # Código fuente (Auth, Products, Interceptors, Routes)
│   ├── nginx.conf                     # Proxy Nginx para SPA y forwarding /api
│   ├── Dockerfile                     # Contenedor multietapa para React + Nginx
│   └── vite.config.ts                 # Configuración de Vite con Proxy hacia la API
├── database/                          # Scripts SQL e inicialización de BD
│   └── init/
│       └── 01_init_schema.sql         # DDL de tablas, índices y datos iniciales (Seed)
├── docker-compose.yml                 # Orquestador total (API + SPA + PostgreSQL + Redis)
├── .gitignore                         # Reglas de exclusión para Git
└── README.md                          # Documentación formal del proyecto
```

---

## 🐳 Guía Paso a Paso para Despliegue con Docker

### Opción A: Despliegue Completo con Docker Compose (Recomendado)

Con un solo comando puedes compilar e iniciar **todos** los 4 servicios (**Base de Datos, Redis, Backend API y Frontend SPA**):

```bash
# 1. Levantar toda la infraestructura y servicios en segundo plano
docker compose up -d --build

# 2. Verificar que los 4 contenedores estén en estado 'healthy' o 'running'
docker compose ps
```

#### URLs de Acceso tras ejecutar Docker Compose:
- 🌐 **Frontend (React SPA):** `http://localhost:8080`
- ⚡ **Backend API (Swagger):** `http://localhost:5000/swagger`
- 🗄️ **PostgreSQL:** `localhost:5432` (DB: `asisya_db`, User: `asisya_user`, Pass: `asisya_password`)
- 🔴 **Redis Cache:** `localhost:6379`

---

### Opción B: Construir y Ejecutar Contenedores Uno a Uno

Si deseas probar o construir los contenedores individualmente:

#### 1. Construir y ejecutar la Base de Datos (PostgreSQL):
```bash
# Levantar PostgreSQL solo
docker compose up -d asisya-db asisya-redis
```

#### 2. Construir e Iniciar el Contenedor del Backend (.NET 8):
```bash
# Construir la imagen Docker del Backend
docker build -t asisya-backend:latest ./backend

# Ejecutar el contenedor del Backend enlazado a la red de la BD
docker run -d \
  --name asisya_backend_api \
  -p 5000:5000 \
  --network prueba_tecnica_asisya_default \
  -e ConnectionStrings__DefaultConnection="Host=asisya-db;Port=5432;Database=asisya_db;Username=asisya_user;Password=asisya_password" \
  asisya-backend:latest
```

#### 3. Construir e Iniciar el Contenedor del Frontend (React + Nginx):
```bash
# Construir la imagen Docker del Frontend
docker build -t asisya-frontend:latest ./frontend

# Ejecutar el contenedor del Frontend
docker run -d \
  --name asisya_frontend_spa \
  -p 8080:80 \
  --network prueba_tecnica_asisya_default \
  asisya-frontend:latest
```

---

## 🛠️ Ejecución Local Sin Docker (Modo Desarrollo)

### 1. Iniciar Base de Datos y Redis:
```bash
docker compose up -d asisya-db asisya-redis
```

### 2. Ejecutar Pruebas y Backend:
```bash
cd backend

# Correr pruebas automatizadas
dotnet test

# Iniciar la API
cd Api
dotnet run
```

### 3. Ejecutar Frontend (React + Vite):
```bash
cd frontend
npm install
npm run dev
```

---

## ☁️ Escalabilidad Horizontal en Entornos Cloud

1. **API Stateless & Balanceador de Cargas:** La API no almacena sesión en memoria (autenticación JWT). Se despliegan $N$ réplicas en Kubernetes (EKS/GKE/AKS) detrás de un *Ingress Controller* o AWS ALB.
2. **Réplicas de Lectura en PostgreSQL (Read Replicas):** Las escrituras e inserciones masivas se dirigen al nodo primario, mientras que las consultas de lectura (`GET /api/products`) se distribuyen entre réplicas de lectura.
3. **Desacoplamiento con Colas de Mensajes:** Para cargas extremas, la API acepta las solicitudes masivas retornando `202 Accepted` y delega la inserción a consumidores en segundo plano vía **RabbitMQ** o **AWS SQS**.
