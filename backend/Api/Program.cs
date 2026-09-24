using System.Reflection;
using Asisya.Api.Middlewares;
using Asisya.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios a la inyección de dependencias
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger / OpenAPI con soporte para JWT Bearer y Documentación
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API ASISYA - Gestión de Productos y Categorías",
        Version = "v1",
        Description = "API REST de alto rendimiento (.NET 8) con soporte para inserción masiva de 100,000 productos, JWT y Clean Architecture."
    });

    c.EnableAnnotations();

    // Configurar esquema de seguridad JWT Bearer en Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT obtenido en el endpoint POST /api/auth/login.\n\nEjemplo: 'eyJhbGciOiJIUzI1Ni...'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Incluir comentarios de documentación XML en Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Registrar la capa de Infraestructura (PostgreSQL, Repositorios, JWT, Redis)
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar política de CORS para permitir al Frontend de React (Vite)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// 2. Configurar la canalización de peticiones HTTP (Middleware Pipeline)
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API ASISYA v1");
        c.RoutePrefix = "swagger"; // Disponible en /swagger
        c.DisplayRequestDuration();
    });
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Permitir que WebApplicationFactory acceda a Program en Pruebas de Integración
public partial class Program { }
