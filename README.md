#  Prueba Técnica ASISYA - DEV II
**Solución Fullstack para Gestión de Productos y Categorías con Carga Masiva y Alta Escalabilidad**

---

##  1. Pasos para Encender el Ambiente con Docker

Para levantar toda la infraestructura del proyecto (**Base de Datos PostgreSQL, Caché Redis, Backend API .NET 8 y Frontend SPA React**) con un solo comando:

###  En Linux (Fedora / Ubuntu / Debian)
```bash
# 1. Asegurarte de estar en la raíz del proyecto
cd prueba_tecnica_asisya

# 2. Levantar y compilar todos los servicios en segundo plano
docker compose up -d --build

# 3. Verificar que los 4 contenedores estén corriendo correctamente
docker compose ps
```

###  En Windows (PowerShell / Símbolo del Sistema)
```powershell
# 1. Abrir PowerShell en la raíz del proyecto
cd prueba_tecnica_asisya

# 2. Compilar e iniciar los servicios
docker compose up -d --build

# 3. Validar el estado de los contenedores
docker compose ps
```

> **Para detener el ambiente en cualquier momento:** `docker compose down`

---

##  2. Rutas de Acceso y Credenciales

### URLs del Sistema
-  **Frontend SPA (React):** [http://localhost:8080](http://localhost:8080)
-  **Backend REST API (Swagger UI):** [http://localhost:5000/swagger](http://localhost:5000/swagger)

### Credenciales de Usuario por Defecto (JWT Auth)
| Usuario | Correo Electrónico | Contraseña | Rol |
| :--- | :--- | :--- | :--- |
| **`admin`** | `admin@asisya.com` | **`Admin123!`** | `Admin` |

###  Credenciales de la Base de Datos (PostgreSQL)
- **Host:** `localhost` (o `asisya-db` dentro de Docker)
- **Puerto:** `5432`
- **Base de Datos:** `asisya_db`
- **Usuario:** `asisya_user`
- **Contraseña:** `asisya_password`
- **Caché Redis:** `localhost:6379`

---

##  3. Arquitectura de la Solución y Decisiones Técnicas

### Decisión de Repositorio: Monorepo
Decidí organizar la solución como un **Monorepo** (`backend/`, `frontend/`, `database/`, `.github/`). Esta decisión facilita centralizar la prueba técnica, simplifica la revisión del código y permite orquestar con un único `docker-compose.yml` toda la infraestructura del sistema sin requerir múltiples repositorios separados.

###  Arquitectura Hexagonal (Ports & Adapters)
Para la construcción del Backend en .NET 8 elegí la **Arquitectura Hexagonal (Puertos y Adaptadores)**. 

La razón principal de esta elección es **aislar por completo el dominio y la lógica de negocio pura de las librerías, marcos de trabajo (frameworks) y motores de base de datos**. 
- **Flexibilidad Futura:** Si el día de mañana la compañía decide migrar la base de datos relacional (PostgreSQL) a un motor no relacional (MongoDB, DynamoDB) o incluso enviar analíticas masivas a **Google BigQuery**, el núcleo del dominio y los casos de uso permanecerán intactos. Solo se requerirá crear un nuevo adaptando de infraestructura.

### Carga Masiva (Bulk Insert) basada en Flujo de Bytes Binario
Para cumplir con el requerimiento de procesar **100,000 productos** de forma ultra eficiente vía `POST /api/products/bulk`, descarté la iteración tradicional del ORM (`foreach -> Add -> SaveChanges`), ya que colapsaría la memoria.

En su lugar, implementé la **estrategia de streaming binario nativo de PostgreSQL (`NpgsqlBinaryImporter / COPY FROM STDIN BINARY`)**:
- La API abre una transmisión de bytes directa hacia el motor de almacenamiento de PostgreSQL.
- Los 100,000 registros son serializados en un flujo binario y transferidos en un solo bloque de red sin pasar por el rastreador de cambios (*Change Tracker*) del ORM.
- **Resultado:** Inserción completa de los 100,000 productos en **menos de 2 segundos**.

### Manejo de Concurrencia con Redis
Incorporé **Redis** dentro de la arquitectura como una capa de caché distribuida en memoria. Esto permite almacenar en memoria los catálogos de productos y categorías de acceso frecuente, mitigando la carga directa en la base de datos cuando existen múltiples peticiones concurrentes simultáneas.

###  Autenticación y Flujo JWT
1. El usuario inicia sesión en la vista de Login con sus credenciales.
2. El Backend valida las credenciales y genera un **Token JWT firmado (HMAC SHA-256)** con expiración.
3. El Frontend recibe el token y lo almacena de forma segura en `localStorage`.
4. Mediante un **Interceptor de Axios** (Middleware en el cliente), el token se adjunta automáticamente en el encabezado `Authorization: Bearer <token>` de todas las peticiones HTTP subsiguientes.
5. Si el token vence o es inválido, el interceptor captura la respuesta `401 Unauthorized` y redirige automáticamente al usuario a la pantalla de Login.

---

##  4. Estructura de Carpetas del Proyecto

```text
prueba_tecnica_asisya/
├── backend/                           # Capa Backend (.NET 8 - Arquitectura Hexagonal)
│   ├── Api/                           # Adaptador REST (Controllers, Middlewares, Program.cs, Swagger)
│   ├── Application/                   # Casos de Uso, DTOs y Mapeos de negocio
│   ├── Domain/                        # Núcleo puro (Entidades, Interfaces de Repositorios)
│   ├── Infrastructure/                # Adaptadores de Persistencia (EF Core, Npgsql Binary, JWT, Redis)
│   ├── tests/                         # Suite de Pruebas Automatizadas
│   │   ├── Backend.UnitTests/         # Pruebas Unitarias (xUnit, NSubstitute, FluentAssertions)
│   │   └── Backend.IntegrationTests/  # Pruebas de Integración (WebApplicationFactory + InMemory DB)
│   ├── Dockerfile                     # Imagen Docker multietapa para la API
│   └── Asisya.sln                     # Solución completa de C#
├── frontend/                          # Capa Frontend (SPA React + Vite + TypeScript)
│   ├── src/
│   │   ├── components/                # Componentes UI (Navbar, Modales)
│   │   ├── features/                  # Módulos de Auth (Context, Login) y Products (Catálogo, Bulk)
│   │   ├── routes/                    # Enrutamiento modular y AuthGuard
│   │   └── services/                  # Cliente Axios e Interceptor de JWT
│   ├── nginx.conf                     # Configuración de Nginx Reverse Proxy para la SPA
│   └── Dockerfile                     # Imagen Docker multietapa (React + Nginx)
├── database/                          # Persistencia de Base de Datos
│   └── init/
│       └── 01_init_schema.sql         # DDL SQL con tablas, llaves foráneas, índices y Seed Data
├── .github/
│   └── workflows/
│       └── ci.yml                     # Pipeline de CI/CD (Build, Test, Lint, Docker)
├── docker-compose.yml                 # Orquestador de la solución completa
├── .gitignore                         # Exclusiones de Git
└── README.md                          # Documentación del proyecto
```

---

##  5. Orquestación con Docker Compose

El archivo `docker-compose.yml` en la raíz ensambla los 4 contenedores requeridos para que la solución funcione de manera autónoma:

1. **`asisya-db`**: Motor PostgreSQL 16 que ejecuta automáticamente el script `01_init_schema.sql` al iniciar.
2. **`asisya-redis`**: Servidor Redis 7 para la gestión de caché y alto rendimiento.
3. **`asisya-backend`**: API REST compilada en .NET 8 que se conecta a PostgreSQL y Redis.
4. **`asisya-frontend`**: Servidor Nginx que aloja la SPA de React y redirige las llamadas `/api/` hacia el Backend mediante Proxy Inverso.

---

##  6. Propuesta de Escalabilidad Horizontal en la Nube

Para llevar este sistema a producción y responder a un crecimiento exponencial de usuarios y volumen de datos, propongo la siguiente estrategia evolutiva por fases:

### 1. Fase Inicial: Arquitectura Serverless (Cloud Run / AWS Lambda)
Inicialmente propondría desplegar la API y el Frontend utilizando servicios **Serverless** basados en contenedores, como **Google Cloud Run** o **AWS Lambda / Container Apps**. Esta arquitectura permite escalar automáticamente de 0 a cientos de instancias bajo demanda con un esquema de costos muy eficiente (pago por uso real).

### 2. Fase de Crecimiento Moderado: Servidores Dedicados + Balanceador de Carga
A medida que el tráfico sea constante y predecible, migraría a una infraestructura de **servidores dedicados (Compute Engines / EC2)** respaldada por un **Balanceador de Carga de aplicación en la nube (AWS ALB / GCP Load Balancer)** configurado con políticas de auto-escalado (*Auto Scaling Groups*).

### 3. Fase de Desacoplamiento: Colas de Mensajes (Message Brokers)
*Antes de escalar a clústeres complejos*, para soportar picos de carga masiva (ej. millones de productos recibidos en simultáneo), implementaría un desacoplamiento mediante **Colas de Mensajes (RabbitMQ, AWS SQS o Google Pub/Sub)**. La API aceptaría la solicitud retornando de inmediato un `202 Accepted` y enviará los mensajes a la cola para que workers dedicados los procesen en segundo plano sin saturar los servidores HTTP.

### 4. Fase de Gran Escala: Kubernetes (EKS / GKE)
Si el volumen exige una alta densidad de microservicios y disponibilidad global, implementaría un clúster de **Kubernetes (GKE / EKS)**. El balanceador de carga de la nube distribuirá el tráfico hacia el **Ingress Controller de Kubernetes**, y el clúster gestionará automáticamente el escalado de *Pods* ($HPA$) y nodos según el consumo de CPU y memoria de la API.
