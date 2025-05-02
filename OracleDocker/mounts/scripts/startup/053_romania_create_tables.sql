SET SERVEROUTPUT ON;

-- Switch to ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_ROMANIA_USER;

-- Dynamically include the audit setup script using the PROJECT_PATH environment variable
@&PROJECT_PATH\OracleDocker\mounts\scripts\startup\04_audit_setup.sql

CREATE OR REPLACE PROCEDURE TRY_CREATE_TABLE (
    tbl_name IN VARCHAR2,
    cols_and_constraints_csv IN VARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
    table_exists NUMBER := 0;
BEGIN
    -- Ensure the procedure operates under the correct schema
    EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = ESHOP_ROMANIA_USER';

    LOG_DEBUG(
        'eshop_romania/TRY_CREATE_TABLE: Trying to create table ' || tbl_name || 
        ' | Container = ' || SYS_CONTEXT('USERENV', 'CON_NAME') || 
        ' | Schema = ' || SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA') || 
        ' | Connected as = ' || SYS_CONTEXT('USERENV', 'SESSION_USER'), 
        created_by
    );

    -- Check if table exists
    SELECT COUNT(*) INTO table_exists
    FROM USER_TABLES
    WHERE UPPER(TABLE_NAME) = UPPER(tbl_name);

    IF table_exists = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE TABLE ' || UPPER(tbl_name) || ' ( ' ||
            cols_and_constraints_csv || ' 
        )';
        LOG_INFORMATION('TRY_CREATE_TABLE: Table ' || tbl_name || ' created.', created_by);
    ELSE
        LOG_INFORMATION('TRY_CREATE_TABLE: Table ' || tbl_name || ' already exists.', created_by);
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('TRY_CREATE_TABLE: Error creating table ' || tbl_name || ': ' || SQLERRM, created_by);
END;
/

BEGIN
    -- Check and create IDNT_Users table
    TRY_CREATE_TABLE('IDNT_USERS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        username NVARCHAR2(25) UNIQUE NOT NULL,
        first_name NVARCHAR2(150) NOT NULL,
        last_name NVARCHAR2(150) NOT NULL,
        date_of_birth DATE NOT NULL,
        phone_number VARCHAR2(25),
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'romania_create_tables');

    -- Check and create SLS_Order_Statuses table
    TRY_CREATE_TABLE('SLS_ORDER_STATUSES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name VARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'romania_create_tables');

    -- Check and create SLS_Orders table
    TRY_CREATE_TABLE('SLS_ORDERS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        customer_id NUMBER NOT NULL,
        customer_region_id NUMBER NOT NULL,
        address NVARCHAR2(850) NOT NULL,
        status_id NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_orders_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_sls_orders_sls_order_statuses FOREIGN KEY (status_id) REFERENCES SLS_Order_Statuses(id)
    ', 'romania_create_tables');

    -- Check and create SLS_Order_Items table
    TRY_CREATE_TABLE('SLS_ORDER_ITEMS', ' 
        order_id NUMBER,
        product_id NUMBER,
        quantity NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_order_items_sls_orders FOREIGN KEY (order_id) REFERENCES SLS_Orders(id),
        -- CONSTRAINT fk_sls_order_items_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id), 
        CONSTRAINT chk_sls_order_items_quantity_positive 
            CHECK (quantity >= 0), 
        PRIMARY KEY (order_id, product_id)
    ', 'romania_create_tables');

    -- Check and create SLS_Order_Bundles table
    TRY_CREATE_TABLE('SLS_ORDER_BUNDLES', ' 
        order_id NUMBER,
        bundle_id NUMBER,
        quantity NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_order_bundles_sls_orders FOREIGN KEY (order_id) REFERENCES SLS_Orders(id),
        -- CONSTRAINT fk_sls_order_bundles_sls_bundle_discounts FOREIGN KEY (bundle_id) REFERENCES SLS_Bundle_Discounts(id), 
        CONSTRAINT chk_sls_order_bundles_quantity_positive 
            CHECK (quantity >= 0), 
        PRIMARY KEY (order_id, bundle_id)
    ', 'romania_create_tables');

    -- Check and create BLG_Invoice_Statuses table
    TRY_CREATE_TABLE('BLG_INVOICE_STATUSES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name VARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'romania_create_tables');

    -- Check and create BLG_Invoices table
    TRY_CREATE_TABLE('BLG_INVOICES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        customer_id NUMBER NOT NULL,
        customer_region_id NUMBER NOT NULL,
        total_discount_in_eur NUMBER(10, 2) DEFAULT 0,
        status_id NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_blg_invoices_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_blg_invoices_blg_invoice_statuses FOREIGN KEY (status_id) REFERENCES BLG_Invoice_Statuses(id)
    ', 'romania_create_tables');

    -- Check and create BLG_Invoice_Items table
    TRY_CREATE_TABLE('BLG_INVOICE_ITEMS', ' 
        invoice_id NUMBER,
        product_id NUMBER,
        product_name NVARCHAR2(150) NOT NULL,
        product_description NVARCHAR2(850) DEFAULT NULL,
        product_price_in_eur NUMBER(10, 2) NOT NULL,
        products_applied_discount_in_eur NUMBER(10, 2) DEFAULT 0,
        products_price_after_discount_in_eur NUMBER(10, 2) NOT NULL,
        quantity NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_blg_invoice_items_blg_invoices FOREIGN KEY (invoice_id) REFERENCES BLG_Invoices(id),
        CONSTRAINT chk_blg_invoice_items_product_price_in_eur_positive 
            CHECK (product_price_in_eur >= 0), 
        CONSTRAINT chk_blg_invoice_items_products_applied_discount_in_eur_positive 
            CHECK (products_applied_discount_in_eur >= 0), 
        CONSTRAINT chk_blg_invoice_items_products_price_after_discount_in_eur_positive 
            CHECK (products_price_after_discount_in_eur >= 0), 
        CONSTRAINT chk_blg_invoice_items_quantity_positive 
            CHECK (quantity >= 0), 
        PRIMARY KEY (invoice_id, product_id)
    ', 'romania_create_tables');

    -- Check and create BLG_Invoice_Bundles table
    TRY_CREATE_TABLE('BLG_INVOICE_BUNDLES', ' 
        invoice_id NUMBER,
        bundle_id NUMBER,
        bundle_price_in_eur NUMBER(10, 2) NOT NULL,
        bundles_applied_discount_in_eur NUMBER(10, 2) DEFAULT 0,
        bundles_price_after_discount_in_eur NUMBER(10, 2) NOT NULL,
        quantity NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_blg_invoice_bundles_blg_invoices FOREIGN KEY (invoice_id) REFERENCES BLG_Invoices(id), 
        CONSTRAINT chk_blg_invoice_bundles_bundle_price_in_eur_positive 
            CHECK (bundle_price_in_eur >= 0), 
        CONSTRAINT chk_blg_invoice_bundles_bundles_applied_discount_in_eur_positive 
            CHECK (bundles_applied_discount_in_eur >= 0), 
        CONSTRAINT chk_blg_invoice_bundles_bundles_price_after_discount_in_eur_positive 
            CHECK (bundles_price_after_discount_in_eur >= 0), 
        CONSTRAINT chk_blg_invoice_bundles_quantity_positive 
            CHECK (quantity >= 0), 
        PRIMARY KEY (invoice_id, bundle_id)
    ', 'romania_create_tables');

    LOG_INFORMATION('Finished creating tables.', 'romania_create_tables');
END;
/
