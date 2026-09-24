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
│   │   ├── Backend.UnitTests/         # Pruebas unitarias (xUnit, NSubstitute, FluentAssertions)
│   │   └── Backend.IntegrationTests/  # Pruebas de integración
│   └── Asisya.sln                     # Solución completa del Backend (.NET 8)
├── frontend/                          # SPA en React + Vite + TypeScript
│   ├── src/                           # Código fuente (Auth, Products, Interceptors, Routes)
│   ├── package.json                   # Dependencias de React, Vite y Axios
│   └── vite.config.ts                 # Configuración de Vite con Proxy hacia la API
├── database/                          # Scripts SQL e inicialización de BD
│   └── init/
│       └── 01_init_schema.sql         # DDL de tablas, índices y datos iniciales (Seed)
├── docker-compose.yml                 # Orquestador local (PostgreSQL + Redis)
├── .gitignore                         # Reglas de exclusión para Git
└── README.md                          # Documentación formal del proyecto
```

---

## 🛠️ Instrucciones de Construcción y Ejecución Local

### 1. Iniciar la Base de Datos con Docker:
```bash
docker compose up -d
```

### 2. Ejecutar el Backend y las Pruebas (.NET 8):
```bash
cd backend

# Ejecutar Pruebas Unitarias
dotnet test

# Ejecutar la API REST
cd Api
dotnet run
```
*La API estará disponible en `http://localhost:5000` y el Swagger UI en `http://localhost:5000/swagger`.*

### 3. Ejecutar el Frontend (React + Vite):
En una nueva ventana de terminal:
```bash
cd frontend
npm install
npm run dev
```
*El Frontend estará disponible en **`http://localhost:5173`**.*

---

## ☁️ Escalabilidad Horizontal en Entornos Cloud

1. **API Stateless & Balanceador de Cargas:** La API no almacena sesión en memoria (autenticación JWT). Se despliegan $N$ réplicas en Kubernetes (EKS/GKE/AKS) detrás de un *Ingress Controller* o AWS ALB.
2. **Réplicas de Lectura en PostgreSQL (Read Replicas):** Las escrituras e inserciones masivas se dirigen al nodo primario, mientras que las consultas de lectura (`GET /api/products`) se distribuyen entre réplicas de lectura.
3. **Desacoplamiento con Colas de Mensajes:** Para cargas extremas, la API acepta las solicitudes masivas retornando `202 Accepted` y delega la inserción a consumidores en segundo plano vía **RabbitMQ** o **AWS SQS**.
