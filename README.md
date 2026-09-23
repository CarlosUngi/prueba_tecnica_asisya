# 🚀 Prueba Técnica ASISYA - DEV II

Solución integral Backend (API REST en .NET 8), Frontend (SPA en React + TypeScript) y Persistencia (PostgreSQL + Redis) diseñada bajo principios de **Clean Architecture**, alta escalabilidad y rendimiento optimizado para el procesamiento masivo de datos.

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
1. **Inserción Masiva Nativa (`NpgsqlBinaryImporter` / PostgreSQL Copy):** Para el endpoint `POST /Product`, se utiliza la funcionalidad de transmisión binaria nativa de PostgreSQL (`COPY FROM STDIN BINARY`), lo que permite insertar los 100,000 registros en **menos de 2 segundos**, evitando el sobrecosto del rastreo de cambios en ORMs tradicionales.
2. **Consultas Sin Rastreo (`AsNoTracking`):** Las peticiones `GET /Products` emplean `.AsNoTracking()` para minimizar la huella de memoria.
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
│   ├── Api/                           # Controladores REST, Middlewares, Swagger
│   ├── Application/                   # Casos de Uso, DTOs y Mapeos
│   ├── Domain/                        # Entidades del sistema e Interfaces de Repositorios
│   └── Infrastructure/                # Persistencia (EF Core, Npgsql), JWT Auth y Redis
├── frontend/                          # SPA en React + Vite + TypeScript
│   └── src/
│       ├── components/                # Componentes reutilizables de UI
│       ├── features/                  # Módulos de negocio (Auth, Products)
│       ├── routes/                    # React Router DOM y AuthGuard
│       └── services/                  # Cliente Axios con interceptores JWT
├── database/                          # Scripts SQL e inicialización de BD
│   ├── init/
│   │   └── 01_init_schema.sql         # DDL de tablas, índices y datos iniciales (Seed)
│   └── scripts/                       # Scripts auxiliares de mantenimiento
├── tests/                             # Cobertura de pruebas automatizadas
│   ├── Backend.UnitTests/             # Pruebas unitarias (xUnit, NSubstitute/Moq)
│   └── Backend.IntegrationTests/      # Pruebas de integración (WebApplicationFactory + Testcontainers)
├── docker-compose.yml                 # Orquestador local (API + Frontend + PostgreSQL + Redis)
├── .gitignore                         # Reglas de exclusión para Git
└── README.md                          # Documentación formal del proyecto
```

---

## 🛠️ Ejecución Local con Docker

Para levantar todo el entorno de base de datos e infraestructura en local:

```bash
# Levantar los contenedores de PostgreSQL y Redis
docker compose up -d

# Verificar el estado de los contenedores
docker compose ps
```

### Accesos por Defecto:
- **PostgreSQL:** `localhost:5432` | DB: `asisya_db` | User: `asisya_user` | Pass: `asisya_password`
- **Redis Cache:** `localhost:6379`

---

## ☁️ Escalabilidad Horizontal en Entornos Cloud

Para escalar esta solución en producción ante millones de usuarios:

1. **API Stateless & Balanceador de Cargas:** La API no almacena sesión en memoria (autenticación JWT). Se despliegan $N$ réplicas en Kubernetes (EKS/GKE/AKS) detrás de un *Ingress Controller* o AWS ALB.
2. **Réplicas de Lectura en PostgreSQL (Read Replicas):** Las escrituras y la inserción masiva se dirigen al nodo primario, mientras que las consultas de lectura (`GET /Products`) se distribuyen entre las réplicas de lectura.
3. **Desacoplamiento con Colas de Mensajes:** Para cargas extremas, la API acepta las solicitudes masivas retornando `202 Accepted` y delega la inserción a consumidores en segundo plano vía **RabbitMQ** o **AWS SQS**.
