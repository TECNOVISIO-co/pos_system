-- Secondary indexes to optimise lookups and filters

CREATE INDEX idx_core_users_email ON core_users (email) WHERE deleted_at IS NULL;
CREATE INDEX idx_catalog_products_name ON catalog_products USING gin (to_tsvector('spanish', name));
CREATE INDEX idx_catalog_products_barcode ON catalog_products (barcode);
CREATE INDEX idx_catalog_products_active ON catalog_products (is_active) WHERE deleted_at IS NULL;
CREATE INDEX idx_catalog_price_list_items_product ON catalog_price_list_items (product_id) WHERE deleted_at IS NULL;
CREATE INDEX idx_crm_customers_document ON crm_customers (document_type, document_number) WHERE deleted_at IS NULL;
CREATE INDEX idx_crm_customers_name_search ON crm_customers USING gin (to_tsvector('spanish', name));
CREATE INDEX idx_inventory_warehouse_stock_product ON inventory_warehouse_stocks (product_id);
CREATE INDEX idx_sales_invoices_customer ON sales_invoices (customer_id, issued_at DESC) WHERE deleted_at IS NULL;
CREATE INDEX idx_sales_invoices_status ON sales_invoices (status) WHERE deleted_at IS NULL;
CREATE INDEX idx_sales_payments_invoice ON sales_payments (invoice_id);
CREATE INDEX idx_accounting_receivables_status ON accounting_receivables (status) WHERE deleted_at IS NULL;
CREATE INDEX idx_sync_log_processed ON sync_log (processed, created_at DESC);
