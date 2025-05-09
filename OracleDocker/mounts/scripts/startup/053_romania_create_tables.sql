SET SERVEROUTPUT ON;

-- Switch to ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_ROMANIA_USER;

CREATE OR REPLACE FUNCTION New_Snowflake_Id (
    p_now IN TIMESTAMP,
    p_region_id IN NUMBER,
    p_random IN NUMBER
) RETURN NUMBER IS
BEGIN
    -- Generate the order ID in the format "{year:4}{month:2}{day:2}{region_id:2}{random:8}"
    RETURN TO_NUMBER(
        TO_CHAR(p_now, 'YYYYMMDD') || 
        LPAD(p_region_id, 2, '0') || 
        LPAD(MOD(p_random, 100000000), 8, '0')  -- set p_random to its last 8 digits (and pad if necessary)
    );
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('New_Snowflake_Id: Error generating snowflake ID: ' || SQLERRM);
        RETURN NULL;
END;
/


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
        BEGIN
            EXECUTE IMMEDIATE '
                CREATE TABLE ' || UPPER(tbl_name) || ' ( ' ||
                cols_and_constraints_csv || ' 
            )';
            LOG_INFORMATION('TRY_CREATE_TABLE: Table ' || tbl_name || ' created.', created_by);
        EXCEPTION
            WHEN OTHERS THEN
                LOG_ERROR('TRY_CREATE_TABLE: Error creating table ' || tbl_name || ': ' || SQLERRM, created_by);
                RAISE_APPLICATION_ERROR(-20001, 'Error creating table ' || tbl_name || ': ' || SQLERRM);
        END;
    ELSE
        LOG_DEBUG('TRY_CREATE_TABLE: Table ' || tbl_name || ' already exists.', created_by);
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('TRY_CREATE_TABLE: Error creating table ' || tbl_name || ': ' || SQLERRM, created_by);
END;
/


CREATE OR REPLACE PROCEDURE TRY_CREATE_SHARD (
    shard_name IN VARCHAR2,
    shard_query IN VARCHAR2,
    alter_commands IN VARCHAR2 DEFAULT '',
    created_by IN VARCHAR2 DEFAULT NULL
) IS
    shard_exists NUMBER := 0;
BEGIN 
    -- Ensure the procedure operates under the correct schema
    EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = ESHOP_ROMANIA_USER';

    LOG_DEBUG(
        'eshop_romania/TRY_CREATE_SHARD: Trying to create shard ' || shard_name || 
        ' | Container = ' || SYS_CONTEXT('USERENV', 'CON_NAME') || 
        ' | Schema = ' || SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA') || 
        ' | Connected as = ' || SYS_CONTEXT('USERENV', 'SESSION_USER'), 
        created_by
    );

    -- Check if shard exists
    SELECT COUNT(*) 
    INTO shard_exists
    FROM USER_TABLES
    WHERE UPPER(TABLE_NAME) = UPPER(shard_name);

    IF shard_exists = 0 THEN
        BEGIN
            EXECUTE IMMEDIATE '
                CREATE TABLE ' || UPPER(shard_name) || ' AS ' || shard_query
            ;
            LOG_INFORMATION('TRY_CREATE_SHARD: Shard ' || shard_name || ' created.', created_by);

            IF alter_commands IS NOT NULL AND alter_commands <> '' THEN
                EXECUTE IMMEDIATE alter_commands;
                LOG_INFORMATION('TRY_CREATE_SHARD: Alter commands executed for ' || shard_name || '.', created_by);
            END IF;
        EXCEPTION
            WHEN OTHERS THEN
                LOG_ERROR('TRY_CREATE_SHARD: Error creating shard ' || shard_name || ': ' || SQLERRM, created_by);
                RAISE_APPLICATION_ERROR(-20001, 'Error creating shard ' || shard_name || ': ' || SQLERRM);
        END;
    ELSE
        LOG_DEBUG('TRY_CREATE_SHARD: Shard ' || shard_name || ' already exists.', created_by);
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('TRY_CREATE_SHARD: Error creating shard ' || shard_name || ': ' || SQLERRM, created_by);
END;
/


BEGIN
    TRY_CREATE_SHARD(
        'IDNT_USERS',
        'SELECT 
            id,
            username,
            first_name,
            last_name,
            date_of_birth,
            phone_number,
            region_id,
            created_on,
            last_updated_on
        FROM IDNT_USERS@eshop_oltp_link
        WHERE region_id <> 0',
        '
        ALTER TABLE IDNT_USERS ADD CONSTRAINT pk_idnt_users PRIMARY KEY (id);
        ALTER TABLE IDNT_USERS ADD CONSTRAINT uq_idnt_users_username UNIQUE (username);
        ALTER TABLE IDNT_USERS ADD CONSTRAINT uq_idnt_users_email UNIQUE (email);
        ALTER TABLE IDNT_USERS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE IDNT_USERS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_PRODUCTS',
        'SELECT * 
        FROM SLS_PRODUCTS@eshop_oltp_link',
        '
        ALTER TABLE SLS_PRODUCTS ADD CONSTRAINT pk_sls_products PRIMARY KEY (id);
        ALTER TABLE SLS_PRODUCTS MODIFY id NUMBER GENERATED ALWAYS AS IDENTITY;
        ALTER TABLE SLS_PRODUCTS ADD CONSTRAINT chk_sls_products_product_price_in_eur_positive CHECK (price_in_eur >= 0);
        ALTER TABLE SLS_PRODUCTS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_PRODUCTS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_PRODUCT_CATEGORIES',
        'SELECT *  
        FROM SLS_PRODUCT_CATEGORIES@eshop_oltp_link',
        '
        ALTER TABLE SLS_PRODUCT_CATEGORIES ADD CONSTRAINT pk_sls_product_categories PRIMARY KEY (id);
        ALTER TABLE SLS_PRODUCT_CATEGORIES ADD CONSTRAINT uq_sls_product_categories_name UNIQUE (name);
        ALTER TABLE SLS_PRODUCT_CATEGORIES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_PRODUCT_CATEGORIES MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_PRODUCT_SUBCATEGORIES',
        'SELECT * 
        FROM SLS_PRODUCT_SUBCATEGORIES@eshop_oltp_link',
        '
        ALTER TABLE SLS_PRODUCT_SUBCATEGORIES ADD CONSTRAINT pk_sls_product_subcategories PRIMARY KEY (id);
        ALTER TABLE SLS_PRODUCT_SUBCATEGORIES MODIFY id NUMBER GENERATED ALWAYS AS IDENTITY;
        ALTER TABLE SLS_PRODUCT_SUBCATEGORIES ADD CONSTRAINT fk_sls_product_subcategories_sls_product_categories FOREIGN KEY (category_id) REFERENCES SLS_PRODUCT_CATEGORIES(id);
        ALTER TABLE SLS_PRODUCT_SUBCATEGORIES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_PRODUCT_SUBCATEGORIES MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_PRODUCT_PRODUCT_SUBCATEGORIES',
        'SELECT * 
        FROM SLS_PRODUCT_PRODUCT_SUBCATEGORIES@eshop_oltp_link',
        '
        ALTER TABLE SLS_PRODUCT_PRODUCT_SUBCATEGORIES ADD CONSTRAINT pk_sls_product_product_subcategories PRIMARY KEY (product_id, subcategory_id);
        ALTER TABLE SLS_PRODUCT_PRODUCT_SUBCATEGORIES ADD CONSTRAINT fk_sls_product_product_subcategories_sls_products FOREIGN KEY (product_id) REFERENCES SLS_PRODUCTS(id);
        ALTER TABLE SLS_PRODUCT_PRODUCT_SUBCATEGORIES ADD CONSTRAINT fk_sls_product_product_subcategories_sls_product_subcategories FOREIGN KEY (subcategory_id) REFERENCES SLS_PRODUCT_SUBCATEGORIES(id);
        ALTER TABLE SLS_PRODUCT_PRODUCT_SUBCATEGORIES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_PRODUCT_TAGS',
        'SELECT id, name, created_on, last_updated_on 
        FROM SLS_PRODUCT_TAGS@eshop_oltp_link',
        '
        ALTER TABLE SLS_PRODUCT_TAGS ADD CONSTRAINT pk_sls_product_tags PRIMARY KEY (id);
        ALTER TABLE SLS_PRODUCT_TAGS ADD CONSTRAINT uq_sls_product_tags_name UNIQUE (name);
        ALTER TABLE SLS_PRODUCT_TAGS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_PRODUCT_TAGS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_PRODUCT_PRODUCT_TAGS',
        'SELECT * 
        FROM SLS_PRODUCT_PRODUCT_TAGS@eshop_oltp_link',
        '
        ALTER TABLE SLS_PRODUCT_PRODUCT_TAGS ADD CONSTRAINT pk_sls_product_product_tags PRIMARY KEY (product_id, tag_id);
        ALTER TABLE SLS_PRODUCT_PRODUCT_TAGS ADD CONSTRAINT fk_sls_product_product_tags_sls_products FOREIGN KEY (product_id) REFERENCES SLS_PRODUCTS(id);
        ALTER TABLE SLS_PRODUCT_PRODUCT_TAGS ADD CONSTRAINT fk_sls_product_product_tags_sls_product_tags FOREIGN KEY (tag_id) REFERENCES SLS_PRODUCT_TAGS(id);
        ALTER TABLE SLS_PRODUCT_PRODUCT_TAGS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_ORDER_STATUSES',
        'SELECT * 
        FROM SLS_ORDER_STATUSES@eshop_oltp_link',
        '
        ALTER TABLE SLS_ORDER_STATUSES ADD CONSTRAINT pk_sls_order_statuses PRIMARY KEY (id);
        ALTER TABLE SLS_ORDER_STATUSES ADD CONSTRAINT uq_sls_order_statuses_name UNIQUE (name);
        ALTER TABLE SLS_ORDER_STATUSES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_ORDER_STATUSES MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_DISCOUNT_TYPES',
        'SELECT * 
        FROM SLS_DISCOUNT_TYPES@eshop_oltp_link',
        '
        ALTER TABLE SLS_DISCOUNT_TYPES ADD CONSTRAINT pk_sls_discount_types PRIMARY KEY (id);
        ALTER TABLE SLS_DISCOUNT_TYPES ADD CONSTRAINT uq_sls_discount_types_name UNIQUE (name);
        ALTER TABLE SLS_DISCOUNT_TYPES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_DISCOUNT_TYPES MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_DISCOUNT_REASONS',
        'SELECT * 
        FROM SLS_DISCOUNT_REASONS@eshop_oltp_link',
        '
        ALTER TABLE SLS_DISCOUNT_REASONS ADD CONSTRAINT pk_sls_discount_reasons PRIMARY KEY (id);
        ALTER TABLE SLS_DISCOUNT_REASONS ADD CONSTRAINT uq_sls_discount_reasons_name UNIQUE (name);
        ALTER TABLE SLS_DISCOUNT_REASONS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_DISCOUNT_REASONS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_DISCOUNTS',
        'SELECT * 
        FROM SLS_DISCOUNTS@eshop_oltp_link',
        '
        ALTER TABLE SLS_DISCOUNTS ADD CONSTRAINT pk_sls_discounts PRIMARY KEY (id);
        ALTER TABLE SLS_DISCOUNTS ADD CONSTRAINT fk_sls_discounts_sls_products FOREIGN KEY (product_id) REFERENCES SLS_PRODUCTS(id);
        ALTER TABLE SLS_DISCOUNTS ADD CONSTRAINT fk_sls_discounts_sls_discount_types FOREIGN KEY (discount_type_id) REFERENCES SLS_DISCOUNT_TYPES(id);
        ALTER TABLE SLS_DISCOUNTS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_DISCOUNTS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_BUNDLE_DISCOUNTS',
        'SELECT * 
        FROM SLS_BUNDLE_DISCOUNTS@eshop_oltp_link',
        '
        ALTER TABLE SLS_BUNDLE_DISCOUNTS ADD CONSTRAINT pk_sls_bundle_discounts PRIMARY KEY (id);
        ALTER TABLE SLS_BUNDLE_DISCOUNTS ADD CONSTRAINT fk_sls_bundle_discounts_sls_discount_types FOREIGN KEY (discount_type_id) REFERENCES SLS_DISCOUNT_TYPES(id);
        ALTER TABLE SLS_BUNDLE_DISCOUNTS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_BUNDLE_DISCOUNTS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_BUNDLE_DISCOUNT_PRODUCTS',
        'SELECT * 
        FROM SLS_BUNDLE_DISCOUNT_PRODUCTS@eshop_oltp_link',
        '
        ALTER TABLE SLS_BUNDLE_DISCOUNT_PRODUCTS ADD CONSTRAINT pk_sls_bundle_discount_products PRIMARY KEY (bundle_id, product_id);
        ALTER TABLE SLS_BUNDLE_DISCOUNT_PRODUCTS ADD CONSTRAINT fk_sls_bundle_discount_products_sls_bundle_discounts FOREIGN KEY (bundle_id) REFERENCES SLS_BUNDLE_DISCOUNTS(id);
        ALTER TABLE SLS_BUNDLE_DISCOUNT_PRODUCTS ADD CONSTRAINT fk_sls_bundle_discount_products_sls_products FOREIGN KEY (product_id) REFERENCES SLS_PRODUCTS(id);
        ALTER TABLE SLS_BUNDLE_DISCOUNT_PRODUCTS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_MY_WISHLIST_ITEMS',
        'SELECT * 
        FROM SLS_MY_WISHLIST_ITEMS@eshop_oltp_link',
        '
        ALTER TABLE SLS_MY_WISHLIST_ITEMS ADD CONSTRAINT pk_sls_my_wishlist_items PRIMARY KEY (customer_id, product_id);
        ALTER TABLE SLS_MY_WISHLIST_ITEMS ADD CONSTRAINT fk_sls_my_wishlist_items_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_USERS(id);
        ALTER TABLE SLS_MY_WISHLIST_ITEMS ADD CONSTRAINT fk_sls_my_wishlist_items_sls_products FOREIGN KEY (product_id) REFERENCES SLS_PRODUCTS(id);
        ALTER TABLE SLS_MY_WISHLIST_ITEMS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_MY_SHOPPING_CART_ITEMS',
        'SELECT * 
        FROM SLS_MY_SHOPPING_CART_ITEMS@eshop_oltp_link',
        '
        ALTER TABLE SLS_MY_SHOPPING_CART_ITEMS ADD CONSTRAINT pk_sls_my_shopping_cart_items PRIMARY KEY (customer_id, product_id);
        ALTER TABLE SLS_MY_SHOPPING_CART_ITEMS ADD CONSTRAINT fk_sls_my_shopping_cart_items_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_USERS(id);
        ALTER TABLE SLS_MY_SHOPPING_CART_ITEMS ADD CONSTRAINT fk_sls_my_shopping_cart_items_sls_products FOREIGN KEY (product_id) REFERENCES SLS_PRODUCTS(id);
        ALTER TABLE SLS_MY_SHOPPING_CART_ITEMS ADD CONSTRAINT chk_sls_my_shopping_cart_items_quantity_positive CHECK (quantity >= 0);
        ALTER TABLE SLS_MY_SHOPPING_CART_ITEMS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_MY_SHOPPING_CART_ITEMS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_ORDERS',
        'SELECT * 
        FROM SLS_ORDERS@eshop_oltp_link
        WHERE customer_region_id <> 0',
        '
        ALTER TABLE SLS_ORDERS ADD CONSTRAINT pk_sls_orders PRIMARY KEY (id);
        ALTER TABLE SLS_ORDERS ADD CONSTRAINT fk_sls_orders_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_USERS(id);
        ALTER TABLE SLS_ORDERS ADD CONSTRAINT fk_sls_orders_sls_order_statuses FOREIGN KEY (status_id) REFERENCES SLS_ORDER_STATUSES(id);
        ALTER TABLE SLS_ORDERS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_ORDERS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_ORDER_ITEMS',
        'SELECT oi.* 
        FROM SLS_ORDER_ITEMS@eshop_oltp_link oi
        INNER JOIN SLS_ORDERS@eshop_oltp_link o 
            ON oi.order_id = o.id
        WHERE o.customer_region_id <> 0',
        '
        ALTER TABLE SLS_ORDER_ITEMS ADD CONSTRAINT pk_sls_order_items PRIMARY KEY (order_id, product_id);
        ALTER TABLE SLS_ORDER_ITEMS ADD CONSTRAINT fk_sls_order_items_sls_orders FOREIGN KEY (order_id) REFERENCES SLS_ORDERS(id);
        ALTER TABLE SLS_ORDER_ITEMS ADD CONSTRAINT fk_sls_order_items_sls_products FOREIGN KEY (product_id) REFERENCES SLS_PRODUCTS(id);
        ALTER TABLE SLS_ORDER_ITEMS ADD CONSTRAINT chk_sls_order_items_quantity_positive CHECK (quantity >= 0);
        ALTER TABLE SLS_ORDER_ITEMS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_ORDER_ITEMS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_ORDER_BUNDLES',
        'SELECT ob.* 
        FROM SLS_ORDER_BUNDLES@eshop_oltp_link ob
        INNER JOIN SLS_ORDERS@eshop_oltp_link o 
            ON ob.order_id = o.id
        WHERE o.customer_region_id <> 0',
        '
        ALTER TABLE SLS_ORDER_BUNDLES ADD CONSTRAINT pk_sls_order_bundles PRIMARY KEY (order_id, bundle_id);
        ALTER TABLE SLS_ORDER_BUNDLES ADD CONSTRAINT fk_sls_order_bundles_sls_orders FOREIGN KEY (order_id) REFERENCES SLS_ORDERS(id);
        ALTER TABLE SLS_ORDER_BUNDLES ADD CONSTRAINT fk_sls_order_bundles_sls_bundle_discounts FOREIGN KEY (bundle_id) REFERENCES SLS_BUNDLE_DISCOUNTS(id);
        ALTER TABLE SLS_ORDER_BUNDLES ADD CONSTRAINT chk_sls_order_bundles_quantity_positive CHECK (quantity >= 0);
        ALTER TABLE SLS_ORDER_BUNDLES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_ORDER_BUNDLES MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'SLS_MY_DISCOUNT_COUPONS', ' 
        SELECT *
        FROM SLS_MY_DISCOUNT_COUPONS@eshop_oltp_link',
        '
        ALTER TABLE SLS_MY_DISCOUNT_COUPONS CONSTRAINT fk_sls_my_discount_coupons_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id);
        ALTER TABLE SLS_MY_DISCOUNT_COUPONS CONSTRAINT fk_sls_my_discount_coupons_sls_discount_types FOREIGN KEY (discount_type_id) REFERENCES SLS_Discount_Types(id);
        ALTER TABLE SLS_MY_DISCOUNT_COUPONS CONSTRAINT fk_sls_my_discount_coupons_sls_orders FOREIGN KEY (order_id) REFERENCES SLS_Orders(id);
        ALTER TABLE SLS_MY_DISCOUNT_COUPONS CONSTRAINT fk_sls_my_discount_coupons_sls_discount_reasons FOREIGN KEY (discount_reason_id) REFERENCES SLS_Discount_Reasons(id);
        ALTER TABLE SLS_MY_DISCOUNT_COUPONS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE SLS_MY_DISCOUNT_COUPONS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables');

    TRY_CREATE_SHARD(
        'BLG_INVOICE_STATUSES',
        'SELECT * 
        FROM BLG_INVOICE_STATUSES@eshop_oltp_link',
        '
        ALTER TABLE BLG_INVOICE_STATUSES ADD CONSTRAINT pk_blg_invoice_statuses PRIMARY KEY (id);
        ALTER TABLE BLG_INVOICE_STATUSES ADD CONSTRAINT uq_blg_invoice_statuses_name UNIQUE (name);
        ALTER TABLE BLG_INVOICE_STATUSES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE BLG_INVOICE_STATUSES MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'BLG_INVOICES',
        'SELECT * 
        FROM BLG_INVOICES@eshop_oltp_link
        WHERE customer_region_id <> 0',
        '
        ALTER TABLE BLG_INVOICES ADD CONSTRAINT pk_blg_invoices PRIMARY KEY (id);
        ALTER TABLE BLG_INVOICES ADD CONSTRAINT fk_blg_invoices_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_USERS(id);
        ALTER TABLE BLG_INVOICES ADD CONSTRAINT fk_blg_invoices_blg_invoice_statuses FOREIGN KEY (status_id) REFERENCES BLG_INVOICE_STATUSES(id);
        ALTER TABLE BLG_INVOICES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE BLG_INVOICES MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    TRY_CREATE_SHARD(
        'BLG_INVOICE_ITEMS',
        'SELECT ii.* 
        FROM BLG_INVOICE_ITEMS@eshop_oltp_link ii
        INNER JOIN BLG_INVOICES@eshop_oltp_link i 
            ON ii.invoice_id = i.id
        WHERE i.customer_region_id <> 0',
        '
        ALTER TABLE BLG_INVOICE_ITEMS ADD CONSTRAINT pk_blg_invoice_items PRIMARY KEY (invoice_id, product_id);
        ALTER TABLE BLG_INVOICE_ITEMS ADD CONSTRAINT fk_blg_invoice_items_blg_invoices FOREIGN KEY (invoice_id) REFERENCES BLG_INVOICES(id);
        ALTER TABLE BLG_INVOICE_ITEMS ADD CONSTRAINT chk_blg_invoice_items_product_price_in_eur_positive CHECK (product_price_in_eur >= 0);
        ALTER TABLE BLG_INVOICE_ITEMS ADD CONSTRAINT chk_blg_invoice_items_products_applied_discount_in_eur_positive CHECK (products_applied_discount_in_eur >= 0);
        ALTER TABLE BLG_INVOICE_ITEMS ADD CONSTRAINT chk_blg_invoice_items_products_price_after_discount_in_eur_positive CHECK (products_price_after_discount_in_eur >= 0);
        ALTER TABLE BLG_INVOICE_ITEMS ADD CONSTRAINT chk_blg_invoice_items_quantity_positive CHECK (quantity >= 0);
        ALTER TABLE BLG_INVOICE_ITEMS MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE BLG_INVOICE_ITEMS MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );
    
    TRY_CREATE_SHARD(
        'BLG_INVOICE_BUNDLES',
        'SELECT ib.* 
        FROM BLG_INVOICE_BUNDLES@eshop_oltp_link ib
        INNER JOIN BLG_INVOICES@eshop_oltp_link i 
            ON ib.invoice_id = i.id
        WHERE i.customer_region_id <> 0',
        '
        ALTER TABLE BLG_INVOICE_BUNDLES ADD CONSTRAINT pk_blg_invoice_bundles PRIMARY KEY (invoice_id, bundle_id);
        ALTER TABLE BLG_INVOICE_BUNDLES ADD CONSTRAINT fk_blg_invoice_bundles_blg_invoices FOREIGN KEY (invoice_id) REFERENCES BLG_INVOICES(id);
        ALTER TABLE BLG_INVOICE_BUNDLES ADD CONSTRAINT chk_blg_invoice_bundles_bundle_price_in_eur_positive CHECK (bundle_price_in_eur >= 0);
        ALTER TABLE BLG_INVOICE_BUNDLES ADD CONSTRAINT chk_blg_invoice_bundles_bundles_applied_discount_in_eur_positive CHECK (bundles_applied_discount_in_eur >= 0);
        ALTER TABLE BLG_INVOICE_BUNDLES ADD CONSTRAINT chk_blg_invoice_bundles_bundles_price_after_discount_in_eur_positive CHECK (bundles_price_after_discount_in_eur >= 0);
        ALTER TABLE BLG_INVOICE_BUNDLES ADD CONSTRAINT chk_blg_invoice_bundles_quantity_positive CHECK (quantity >= 0);
        ALTER TABLE BLG_INVOICE_BUNDLES MODIFY created_on DATE DEFAULT SYSDATE NOT NULL;
        ALTER TABLE BLG_INVOICE_BUNDLES MODIFY last_updated_on DATE DEFAULT NULL;
        ', 'romania_create_tables'
    );

    LOG_INFORMATION('Finished creating tables.', 'romania_create_tables');
END;
/
