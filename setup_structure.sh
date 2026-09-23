#!/bin/bash

# Exit on error
set -e

echo "🚀 Reorganizando y creando la estructura de carpetas en la raíz..."

# Eliminar carpeta src previa si existía
if [ -d "src" ]; then
  rm -rf src
fi

# Archivos base en la raíz y workflows
mkdir -p .github/workflows

# Backend (.NET 8 Clean Architecture)
mkdir -p backend/Domain/Entities
mkdir -p backend/Domain/Repositories
mkdir -p backend/Application/DTOs
mkdir -p backend/Application/UseCases
mkdir -p backend/Application/Mappings
mkdir -p backend/Infrastructure/Persistence/Configurations
mkdir -p backend/Infrastructure/Persistence/Repositories
mkdir -p backend/Infrastructure/Identity
mkdir -p backend/Api/Controllers
mkdir -p backend/Api/Middlewares

# Frontend (React + TypeScript)
mkdir -p frontend/src/components
mkdir -p frontend/src/features/auth/context
mkdir -p frontend/src/features/auth/pages
mkdir -p frontend/src/features/products/components
mkdir -p frontend/src/features/products/pages
mkdir -p frontend/src/services
mkdir -p frontend/src/routes

# Base de datos (Scripts SQL e inicialización)
mkdir -p database/init
mkdir -p database/scripts

# Pruebas (Tests)
mkdir -p tests/Backend.UnitTests
mkdir -p tests/Backend.IntegrationTests

# Crear archivos .gitkeep para preservar carpetas vacías en git
touch .github/workflows/.gitkeep
touch backend/Domain/Entities/.gitkeep
touch backend/Domain/Repositories/.gitkeep
touch backend/Application/DTOs/.gitkeep
touch backend/Application/UseCases/.gitkeep
touch backend/Application/Mappings/.gitkeep
touch backend/Infrastructure/Persistence/Configurations/.gitkeep
touch backend/Infrastructure/Persistence/Repositories/.gitkeep
touch backend/Infrastructure/Identity/.gitkeep
touch backend/Api/Controllers/.gitkeep
touch backend/Api/Middlewares/.gitkeep

touch frontend/src/components/.gitkeep
touch frontend/src/features/auth/context/.gitkeep
touch frontend/src/features/auth/pages/.gitkeep
touch frontend/src/features/products/components/.gitkeep
touch frontend/src/features/products/pages/.gitkeep
touch frontend/src/services/.gitkeep
touch frontend/src/routes/.gitkeep

touch database/init/.gitkeep
touch database/scripts/.gitkeep

touch tests/Backend.UnitTests/.gitkeep
touch tests/Backend.IntegrationTests/.gitkeep

echo "✅ Estructura reorganizada exitosamente con backend/, frontend/ y database/ en la raíz."
