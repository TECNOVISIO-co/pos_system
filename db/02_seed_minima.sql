-- Minimal seed data for SalesSwift POS

INSERT INTO core_roles (code, name, description)
VALUES ('admin', 'Administrador', 'Rol con todos los permisos del sistema')
ON CONFLICT (code) DO NOTHING;

INSERT INTO core_permissions (code, description, area)
VALUES
    ('products.read', 'Consultar productos', 'catalogo'),
    ('products.write', 'Crear/editar productos', 'catalogo'),
    ('sales.pos', 'Operar el punto de venta', 'ventas'),
    ('sales.reports', 'Ver reportes de ventas', 'ventas'),
    ('customers.manage', 'Administrar clientes', 'crm'),
    ('sync.manage', 'Gestionar procesos de sincronización', 'sync')
ON CONFLICT (code) DO NOTHING;

INSERT INTO core_role_permissions (role_id, permission_id)
SELECT r.id, p.id
FROM core_roles r
JOIN core_permissions p ON r.code = 'admin'
WHERE p.code IN ('products.read','products.write','sales.pos','sales.reports','customers.manage','sync.manage')
ON CONFLICT DO NOTHING;

INSERT INTO core_users (username, full_name, email, password_hash)
VALUES ('admin', 'Administrador Principal', 'admin@example.com', '$2a$12$placeholderhash')
ON CONFLICT (username) DO NOTHING;

INSERT INTO core_user_roles (user_id, role_id)
SELECT u.id, r.id
FROM core_users u
JOIN core_roles r ON r.code = 'admin'
WHERE u.username = 'admin'
ON CONFLICT DO NOTHING;

INSERT INTO catalog_taxes (code, name, rate, scope)
VALUES
    ('IVA19', 'IVA 19%', 19.00, 'sales'),
    ('IVA05', 'IVA 5%', 5.00, 'sales'),
    ('INC08', 'Impuesto al Consumo 8%', 8.00, 'sales')
ON CONFLICT (code) DO NOTHING;

INSERT INTO catalog_price_lists (code, name, is_default)
VALUES ('BASE', 'Lista Base', true)
ON CONFLICT (code) DO NOTHING;

INSERT INTO inventory_warehouses (code, name, is_default, allow_sales)
VALUES ('MAIN', 'Bodega Principal', true, true)
ON CONFLICT (code) DO NOTHING;
