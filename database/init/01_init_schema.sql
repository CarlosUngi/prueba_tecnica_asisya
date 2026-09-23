-- =============================================================================
-- DDL para la Prueba Técnica ASISYA (PostgreSQL)
-- Modelo Relacional Completo: Categorías, Productos, Proveedores y Usuarios
-- =============================================================================

-- Extensión para generación de UUIDs
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- -----------------------------------------------------------------------------
-- 1. Tabla de Usuarios (Para Autenticación JWT)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL DEFAULT 'User',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- -----------------------------------------------------------------------------
-- 2. Tabla de Categorías (Categories)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS categories (
    category_id SERIAL PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    picture TEXT
);

-- -----------------------------------------------------------------------------
-- 3. Tabla de Proveedores (Suppliers)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS suppliers (
    supplier_id SERIAL PRIMARY KEY,
    company_name VARCHAR(100) NOT NULL,
    contact_name VARCHAR(100),
    contact_title VARCHAR(50),
    address VARCHAR(255),
    city VARCHAR(50),
    region VARCHAR(50),
    postal_code VARCHAR(20),
    country VARCHAR(50),
    phone VARCHAR(30),
    fax VARCHAR(30),
    home_page TEXT
);

-- -----------------------------------------------------------------------------
-- 4. Tabla de Productos (Products) con Llaves Foráneas Explícitas
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS products (
    product_id SERIAL PRIMARY KEY,
    product_name VARCHAR(150) NOT NULL,
    supplier_id INT,
    category_id INT NOT NULL,
    quantity_per_unit VARCHAR(50),
    unit_price NUMERIC(18, 2) NOT NULL DEFAULT 0.00,
    units_in_stock INT NOT NULL DEFAULT 0,
    units_on_order INT NOT NULL DEFAULT 0,
    reorder_level INT NOT NULL DEFAULT 0,
    discontinued BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,

    -- Llaves Foráneas Explícitas
    CONSTRAINT fk_products_categories 
        FOREIGN KEY (category_id) 
        REFERENCES categories(category_id) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE,

    CONSTRAINT fk_products_suppliers 
        FOREIGN KEY (supplier_id) 
        REFERENCES suppliers(supplier_id) 
        ON DELETE SET NULL 
        ON UPDATE CASCADE
);

-- =============================================================================
-- ÍNDICES DE ALTO RENDIMIENTO (Optimizados para lecturas, búsquedas y paginación)
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_products_category_id ON products(category_id);
CREATE INDEX IF NOT EXISTS idx_products_supplier_id ON products(supplier_id);
CREATE INDEX IF NOT EXISTS idx_products_product_name ON products(product_name);
CREATE INDEX IF NOT EXISTS idx_products_unit_price ON products(unit_price);
CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);

-- =============================================================================
-- DATOS INICIALES (SEED DATA)
-- =============================================================================

-- Categorías requeridas según especificación de la prueba
INSERT INTO categories (category_name, description, picture)
VALUES 
    ('SERVIDORES', 'Servidores físicos y dedicados de alto rendimiento', 'https://via.placeholder.com/150?text=Servidores'),
    ('CLOUD', 'Instancias y servicios en la nube computacional', 'https://via.placeholder.com/150?text=Cloud')
ON CONFLICT (category_name) DO NOTHING;

-- Usuario Administrador por defecto para pruebas de JWT
INSERT INTO users (username, email, password_hash, role)
VALUES 
    ('admin', 'admin@asisya.com', '$2a$11$qRzP6wzD5vQ8M.7gS3sC6.8u8u9X8fG4fK8vQ8M7gS3sC68u8u9X8', 'Admin')
ON CONFLICT (username) DO NOTHING;
