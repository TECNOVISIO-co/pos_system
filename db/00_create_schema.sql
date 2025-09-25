-- Schema creation for the modern PostgreSQL model (SalesSwift POS)
-- Generated during Phase 0 - database modernization

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE OR REPLACE FUNCTION gen_uuid_v7()
RETURNS uuid
LANGUAGE plpgsql
AS $$
DECLARE
    unix_ts_ms bigint;
    rand_bytes bytea;
    bytes bytea;
BEGIN
    unix_ts_ms := floor(extract(epoch FROM clock_timestamp()) * 1000);
    rand_bytes := gen_random_bytes(10);

    bytes := set_byte(set_byte(set_byte(set_byte(
        set_byte(set_byte(set_byte(set_byte(
        set_byte(set_byte(set_byte(set_byte(
        set_byte(set_byte(set_byte(set_byte(
            repeat(E'\000', 16)::bytea,
            0, ((unix_ts_ms >> 40) & 255)::int),
            1, ((unix_ts_ms >> 32) & 255)::int),
            2, ((unix_ts_ms >> 24) & 255)::int),
            3, ((unix_ts_ms >> 16) & 255)::int),
            4, ((unix_ts_ms >> 8) & 255)::int),
            5, (unix_ts_ms & 255)::int),
            6, ((0x70) | ((get_byte(rand_bytes, 0) >> 4) & 0x0F))),
            7, ((get_byte(rand_bytes, 0) & 0x0F) << 4) | ((get_byte(rand_bytes, 1) >> 4) & 0x0F)),
            8, (0x80 | (get_byte(rand_bytes, 1) & 0x3F))),
            9, get_byte(rand_bytes, 2)),
            10, get_byte(rand_bytes, 3)),
            11, get_byte(rand_bytes, 4)),
            12, get_byte(rand_bytes, 5)),
            13, get_byte(rand_bytes, 6)),
            14, get_byte(rand_bytes, 7)),
            15, get_byte(rand_bytes, 8)
    );

    RETURN encode(bytes, 'hex')::uuid;
END;
$$;

CREATE TABLE core_users (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    username text NOT NULL UNIQUE,
    full_name text NOT NULL,
    email text UNIQUE,
    password_hash text NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    last_login_at timestamptz,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz
);

CREATE TABLE core_roles (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    code text NOT NULL UNIQUE,
    name text NOT NULL,
    description text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz
);

CREATE TABLE core_permissions (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    code text NOT NULL UNIQUE,
    description text NOT NULL,
    area text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz
);

CREATE TABLE core_role_permissions (
    role_id uuid NOT NULL REFERENCES core_roles(id) ON DELETE CASCADE,
    permission_id uuid NOT NULL REFERENCES core_permissions(id) ON DELETE CASCADE,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    PRIMARY KEY (role_id, permission_id)
);

CREATE TABLE core_user_roles (
    user_id uuid NOT NULL REFERENCES core_users(id) ON DELETE CASCADE,
    role_id uuid NOT NULL REFERENCES core_roles(id) ON DELETE CASCADE,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    PRIMARY KEY (user_id, role_id)
);

CREATE TABLE catalog_taxes (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    code text NOT NULL UNIQUE,
    name text NOT NULL,
    rate numeric(5,2) NOT NULL CHECK (rate >= 0 AND rate <= 100),
    scope text NOT NULL DEFAULT 'sales',
    is_compound boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz
);

CREATE TABLE catalog_price_lists (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    code text NOT NULL UNIQUE,
    name text NOT NULL,
    currency_code char(3) NOT NULL DEFAULT 'COP',
    is_default boolean NOT NULL DEFAULT false,
    valid_from date,
    valid_until date,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz
);

CREATE TABLE catalog_products (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    sku text NOT NULL UNIQUE,
    name text NOT NULL,
    description text,
    barcode text,
    brand text,
    model text,
    unit_of_measure text NOT NULL DEFAULT 'und',
    cost numeric(18,2) NOT NULL DEFAULT 0,
    price numeric(18,2) NOT NULL DEFAULT 0,
    is_active boolean NOT NULL DEFAULT true,
    min_stock numeric(12,3) NOT NULL DEFAULT 0,
    allow_negative_stock boolean NOT NULL DEFAULT false,
    tax_rule_id uuid REFERENCES catalog_taxes(id),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz
);

CREATE TABLE catalog_product_taxes (
    product_id uuid NOT NULL REFERENCES catalog_products(id) ON DELETE CASCADE,
    tax_id uuid NOT NULL REFERENCES catalog_taxes(id),
    priority smallint NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    PRIMARY KEY (product_id, tax_id)
);

CREATE TABLE catalog_price_list_items (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    price_list_id uuid NOT NULL REFERENCES catalog_price_lists(id) ON DELETE CASCADE,
    product_id uuid NOT NULL REFERENCES catalog_products(id) ON DELETE CASCADE,
    price numeric(18,2) NOT NULL,
    currency_code char(3) NOT NULL DEFAULT 'COP',
    valid_from date,
    valid_until date,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz,
    UNIQUE (price_list_id, product_id)
);

CREATE TABLE crm_customers (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    code text NOT NULL UNIQUE,
    document_type text NOT NULL,
    document_number text NOT NULL,
    name text NOT NULL,
    trade_name text,
    email text,
    phone text,
    mobile text,
    address text,
    city text,
    state text,
    country text DEFAULT 'CO',
    postal_code text,
    credit_limit numeric(18,2) NOT NULL DEFAULT 0,
    available_credit numeric(18,2) NOT NULL DEFAULT 0,
    price_list_id uuid REFERENCES catalog_price_lists(id),
    tax_responsibility text,
    birthdate date,
    loyalty_points numeric(12,3) NOT NULL DEFAULT 0,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz,
    UNIQUE (document_type, document_number)
);

CREATE TABLE inventory_warehouses (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    code text NOT NULL UNIQUE,
    name text NOT NULL,
    location text,
    is_default boolean NOT NULL DEFAULT false,
    allow_sales boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz
);

CREATE TABLE inventory_warehouse_stocks (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    warehouse_id uuid NOT NULL REFERENCES inventory_warehouses(id) ON DELETE CASCADE,
    product_id uuid NOT NULL REFERENCES catalog_products(id) ON DELETE CASCADE,
    on_hand numeric(12,3) NOT NULL DEFAULT 0,
    reserved numeric(12,3) NOT NULL DEFAULT 0,
    available numeric(12,3) GENERATED ALWAYS AS (on_hand - reserved) STORED,
    updated_at timestamptz NOT NULL DEFAULT now(),
    UNIQUE (warehouse_id, product_id)
);

CREATE TABLE sales_invoices (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    invoice_number text NOT NULL,
    customer_id uuid NOT NULL REFERENCES crm_customers(id),
    status text NOT NULL DEFAULT 'draft' CHECK (status IN ('draft','posted','cancelled')),
    issued_at timestamptz NOT NULL,
    due_at timestamptz,
    warehouse_id uuid REFERENCES inventory_warehouses(id),
    subtotal numeric(18,2) NOT NULL DEFAULT 0,
    discount_total numeric(18,2) NOT NULL DEFAULT 0,
    tax_total numeric(18,2) NOT NULL DEFAULT 0,
    total numeric(18,2) NOT NULL DEFAULT 0,
    total_paid numeric(18,2) NOT NULL DEFAULT 0,
    balance_due numeric(18,2) GENERATED ALWAYS AS (total - total_paid) STORED,
    currency_code char(3) NOT NULL DEFAULT 'COP',
    notes text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz,
    UNIQUE (invoice_number)
);

CREATE TABLE sales_invoice_items (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    invoice_id uuid NOT NULL REFERENCES sales_invoices(id) ON DELETE CASCADE,
    line_number integer NOT NULL,
    product_id uuid NOT NULL REFERENCES catalog_products(id),
    description text,
    quantity numeric(12,3) NOT NULL CHECK (quantity > 0),
    unit_price numeric(18,2) NOT NULL,
    discount_rate numeric(5,2) NOT NULL DEFAULT 0 CHECK (discount_rate >= 0 AND discount_rate <= 100),
    discount_amount numeric(18,2) NOT NULL DEFAULT 0,
    tax_total numeric(18,2) NOT NULL DEFAULT 0,
    line_total numeric(18,2) NOT NULL DEFAULT 0,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz,
    UNIQUE (invoice_id, line_number)
);

CREATE TABLE sales_payments (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    invoice_id uuid REFERENCES sales_invoices(id) ON DELETE SET NULL,
    customer_id uuid REFERENCES crm_customers(id),
    payment_method text NOT NULL,
    reference text,
    paid_at timestamptz NOT NULL,
    amount numeric(18,2) NOT NULL CHECK (amount >= 0),
    currency_code char(3) NOT NULL DEFAULT 'COP',
    exchange_rate numeric(18,6) NOT NULL DEFAULT 1,
    notes text,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz,
    CONSTRAINT uq_sales_payments_invoice_method UNIQUE (invoice_id, payment_method, reference)
);

CREATE TABLE accounting_receivables (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    customer_id uuid NOT NULL REFERENCES crm_customers(id),
    origin_type text NOT NULL,
    origin_id uuid NOT NULL,
    issued_at timestamptz NOT NULL,
    due_at timestamptz,
    amount numeric(18,2) NOT NULL,
    balance numeric(18,2) NOT NULL,
    currency_code char(3) NOT NULL DEFAULT 'COP',
    status text NOT NULL DEFAULT 'open' CHECK (status IN ('open','partially_paid','paid','written_off')),
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid,
    updated_by uuid,
    deleted_at timestamptz,
    UNIQUE (origin_type, origin_id)
);

CREATE TABLE accounting_receivable_payments (
    id uuid PRIMARY KEY DEFAULT gen_uuid_v7(),
    receivable_id uuid NOT NULL REFERENCES accounting_receivables(id) ON DELETE CASCADE,
    payment_id uuid REFERENCES sales_payments(id) ON DELETE SET NULL,
    amount numeric(18,2) NOT NULL CHECK (amount >= 0),
    applied_at timestamptz NOT NULL DEFAULT now(),
    created_at timestamptz NOT NULL DEFAULT now(),
    created_by uuid
);

CREATE TABLE sync_log (
    id bigserial PRIMARY KEY,
    entity_name text NOT NULL,
    entity_id uuid,
    operation text NOT NULL CHECK (operation IN ('insert','update','delete')),
    payload jsonb,
    origin text NOT NULL DEFAULT 'access',
    processed boolean NOT NULL DEFAULT false,
    error_message text,
    created_at timestamptz NOT NULL DEFAULT now()
);
