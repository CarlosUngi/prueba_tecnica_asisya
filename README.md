# 🚀 Prueba Técnica ASISYA - DEV II

Solución integral Backend (API REST en .NET 8), Frontend (SPA en React + TypeScript) y Persistencia (PostgreSQL + Redis) diseñada bajo principios de **Clean Architecture**, alta escalabilidad y rendimiento optimizado para el procesamiento masivo de datos.

---

## 🔐 Credenciales de Acceso por Defecto (JWT Auth)

Para realizar pruebas en **Swagger UI** (`/swagger`) o desde el **Frontend (React)**:

| Usuario | Correo Electrónico | Contraseña por Defecto | Rol |
| :--- | :--- | :--- | :--- |
| **`admin`** | `admin@asisya.com` | **`Admin123!`** | `Admin` |

> 💡 **Nota:** Con estas credenciales obtienes el token JWT en el endpoint `POST /api/auth/login` para autenticar el botón **`Authorize`** de Swagger.

---

## 🏛️ Arquitectura del Sistema

La solución Backend está estructurada bajo los principios de **Clean Architecture (Arquitectura Limpia / DDD)** para garantizar el desacoplamiento, la mantenibilidad y la facilitación de pruebas automatizadas:

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

### ⚡ Estrategias para Cargas Masivas (100,000 Productos) y Alto Rendimiento
1. **Inserción Masiva Nativa (`NpgsqlBinaryImporter` / PostgreSQL Copy):** Para el endpoint `POST /api/products/bulk`, se utiliza la transmisión binaria nativa de PostgreSQL (`COPY FROM STDIN BINARY`), logrando insertar 100,000 registros en **< 2 segundos**.
2. **Consultas Sin Rastreo (`AsNoTracking`):** Las peticiones `GET /api/products` emplean `.AsNoTracking()` para minimizar la huella de memoria.
3. **Caché Distribuida con Redis:** Almacenamiento en memoria para acelerar consultas repetitivas de categorías y catálogos de productos.
4. **Índices de Base de Datos:** Índices explícitos en `category_id`, `product_name` y `unit_price` para filtrado y paginación veloz.

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
│   └── Asisya.sln                     # Solución completa del Backend (.NET 8)
├── frontend/                          # SPA en React + Vite + TypeScript
│   └── src/
│       ├── components/                # Componentes reutilizables de UI
│       ├── features/                  # Módulos de negocio (Auth, Products)
│       ├── routes/                    # React Router DOM y AuthGuard
│       └── services/                  # Cliente Axios con interceptores JWT
├── database/                          # Scripts SQL e inicialización de BD
│   └── init/
│       └── 01_init_schema.sql         # DDL de tablas, índices y datos iniciales (Seed)
├── tests/                             # Cobertura de pruebas automatizadas
│   ├── Backend.UnitTests/             # Pruebas unitarias (xUnit, NSubstitute/Moq)
│   └── Backend.IntegrationTests/      # Pruebas de integración (WebApplicationFactory + Testcontainers)
├── docker-compose.yml                 # Orquestador local (PostgreSQL + Redis)
├── .gitignore                         # Reglas de exclusión para Git
└── README.md                          # Documentación formal del proyecto
```

---

## 🛠️ Instrucciones de Construcción y Ejecución Local

### Prerrequisitos Comunes
- Docker y Docker Compose instalados.
- SDK de .NET 8.0 instalado.

---

### 🐧 Ejecución en Linux (Fedora / Ubuntu / Debian)

#### 1. Clonar el repositorio y levantar la Base de Datos con Docker:
```bash
# Iniciar contenedores de PostgreSQL y Redis en segundo plano
docker compose up -d

# Verificar el estado de los contenedores
docker compose ps
```

#### 2. Instalar .NET 8 SDK (Si no está instalado):
```bash
# En Fedora:
sudo dnf install -y dotnet-sdk-8.0

# En Ubuntu/Debian:
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0
```

#### 3. Restaurar y ejecutar la API del Backend:
```bash
# Navegar a la carpeta del backend
cd backend/Api

# Restaurar paquetes y compilar
dotnet restore
dotnet build

# Ejecutar la API
dotnet run
```
Abre en tu navegador: **`http://localhost:5000/swagger`** o **`http://localhost:5001/swagger`**.

---

### 🪟 Ejecución en Windows (PowerShell / CMD)

#### 1. Levantar Infraestructura de Base de Datos:
Abre PowerShell o Símbolo del Sistema en la raíz del proyecto:
```powershell
docker compose up -d
docker compose ps
```

#### 2. Compilar y Ejecutar la API con .NET CLI:
```powershell
cd backend\Api
dotnet restore
dotnet build
dotnet run
```
Navega a **`http://localhost:5000/swagger`**.

---

## ☁️ Escalabilidad Horizontal en Entornos Cloud

Para escalar esta solución en producción ante millones de usuarios:

1. **API Stateless & Balanceador de Cargas:** La API no almacena sesión en memoria (autenticación JWT). Se despliegan $N$ réplicas en Kubernetes (EKS/GKE/AKS) detrás de un *Ingress Controller* o AWS ALB.
2. **Réplicas de Lectura en PostgreSQL (Read Replicas):** Las escrituras e inserciones masivas se dirigen al nodo primario, mientras que las consultas de lectura (`GET /api/products`) se distribuyen entre réplicas de lectura.
3. **Desacoplamiento con Colas de Mensajes:** Para cargas extremas, la API acepta las solicitudes masivas retornando `202 Accepted` y delega la inserción a consumidores en segundo plano vía **RabbitMQ** o **AWS SQS**.
