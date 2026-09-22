#!/bin/bash

# Exit on error
set -e

echo "🚀 Creando estructura de carpetas para el proyecto ASISYA..."

# Archivos base en la raíz y workflows
mkdir -p .github/workflows

# Backend (.NET 8 Clean Architecture)
mkdir -p src/Backend/Domain/Entities
mkdir -p src/Backend/Domain/Repositories
mkdir -p src/Backend/Application/DTOs
mkdir -p src/Backend/Application/UseCases
mkdir -p src/Backend/Application/Mappings
mkdir -p src/Backend/Infrastructure/Persistence/Configurations
mkdir -p src/Backend/Infrastructure/Persistence/Repositories
mkdir -p src/Backend/Infrastructure/Identity
mkdir -p src/Backend/Api/Controllers
mkdir -p src/Backend/Api/Middlewares

# Frontend (React + TypeScript)
mkdir -p src/Frontend/src/components
mkdir -p src/Frontend/src/features/auth/context
mkdir -p src/Frontend/src/features/auth/pages
mkdir -p src/Frontend/src/features/products/components
mkdir -p src/Frontend/src/features/products/pages
mkdir -p src/Frontend/src/services
mkdir -p src/Frontend/src/routes

# Pruebas (Tests)
mkdir -p tests/Backend.UnitTests
mkdir -p tests/Backend.IntegrationTests

# Crear archivos .gitkeep para preservar carpetas vacías en git
touch .github/workflows/.gitkeep
touch src/Backend/Domain/Entities/.gitkeep
touch src/Backend/Domain/Repositories/.gitkeep
touch src/Backend/Application/DTOs/.gitkeep
touch src/Backend/Application/UseCases/.gitkeep
touch src/Backend/Application/Mappings/.gitkeep
touch src/Backend/Infrastructure/Persistence/Configurations/.gitkeep
touch src/Backend/Infrastructure/Persistence/Repositories/.gitkeep
touch src/Backend/Infrastructure/Identity/.gitkeep
touch src/Backend/Api/Controllers/.gitkeep
touch src/Backend/Api/Middlewares/.gitkeep
touch src/Frontend/src/components/.gitkeep
touch src/Frontend/src/features/auth/context/.gitkeep
touch src/Frontend/src/features/auth/pages/.gitkeep
touch src/Frontend/src/features/products/components/.gitkeep
touch src/Frontend/src/features/products/pages/.gitkeep
touch src/Frontend/src/services/.gitkeep
touch src/Frontend/src/routes/.gitkeep
touch tests/Backend.UnitTests/.gitkeep
touch tests/Backend.IntegrationTests/.gitkeep

echo "✅ Estructura de directorios creada exitosamente."
