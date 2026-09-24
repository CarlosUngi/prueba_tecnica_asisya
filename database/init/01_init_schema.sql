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
-- ÍNDICES DE ALTO RENDIMIENTO
-- =============================================================================
CREATE INDEX IF NOT EXISTS idx_products_category_id ON products(category_id);
CREATE INDEX IF NOT EXISTS idx_products_supplier_id ON products(supplier_id);
CREATE INDEX IF NOT EXISTS idx_products_product_name ON products(product_name);
CREATE INDEX IF NOT EXISTS idx_products_unit_price ON products(unit_price);
CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);

-- =============================================================================
-- DATOS INICIALES DE PRUEBA (SEED DATA)
-- =============================================================================

-- 1. Categorías obligatorias según requerimiento
INSERT INTO categories (category_id, category_name, description, picture)
VALUES 
    (1, 'SERVIDORES', 'Servidores físicos y dedicados de alto rendimiento', 'https://via.placeholder.com/150?text=Servidores'),
    (2, 'CLOUD', 'Instancias y servicios en la nube computacional', 'https://via.placeholder.com/150?text=Cloud')
ON CONFLICT (category_id) DO UPDATE 
SET category_name = EXCLUDED.category_name, description = EXCLUDED.description, picture = EXCLUDED.picture;

-- 2. Proveedores iniciales
INSERT INTO suppliers (supplier_id, company_name, contact_name, contact_title, country, phone, home_page)
VALUES 
    (1, 'Dell Technologies', 'Michael Dell', 'Sales Manager', 'USA', '+1 800 624-9897', 'https://www.dell.com'),
    (2, 'Amazon Web Services', 'Andy Jassy', 'Cloud Representative', 'USA', '+1 800 282-3867', 'https://aws.amazon.com'),
    (3, 'Microsoft Corporation', 'Satya Nadella', 'Enterprise Lead', 'USA', '+1 800 642-7676', 'https://azure.microsoft.com')
ON CONFLICT (supplier_id) DO NOTHING;

-- 3. Usuario administrador por defecto para autenticación JWT
INSERT INTO users (username, email, password_hash, role)
VALUES 
    ('admin', 'admin@asisya.com', 'Admin123!', 'Admin')
ON CONFLICT (username) DO UPDATE 
SET password_hash = 'Admin123!';

-- 4. Productos de prueba iniciales
INSERT INTO products (product_name, supplier_id, category_id, quantity_per_unit, unit_price, units_in_stock, units_on_order, reorder_level, discontinued)
VALUES 
    ('Dell PowerEdge R750 Rack Server', 1, 1, '1 servidor', 3450.00, 15, 0, 5, FALSE),
    ('AWS EC2 Dedicated Host c5.metal', 2, 2, '1 instancia', 1250.50, 50, 10, 10, FALSE),
    ('Azure Virtual Machine D8s v5', 3, 2, '1 VM', 890.00, 100, 20, 15, FALSE),
    ('Lenovo ThinkSystem SR650 V2', 1, 1, '1 servidor', 2990.00, 8, 2, 3, FALSE)
ON CONFLICT DO NOTHING;

-- Ajustar la secuencia de IDs de tablas para evitar colisiones en futuras inserciones
SELECT setval('categories_category_id_seq', (SELECT MAX(category_id) FROM categories));
SELECT setval('suppliers_supplier_id_seq', (SELECT MAX(supplier_id) FROM suppliers));
SELECT setval('products_product_id_seq', (SELECT MAX(product_id) FROM products));
