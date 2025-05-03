SET SERVEROUTPUT ON;

-- Switch to OLTP PDB
ALTER SESSION SET CONTAINER = eshop_oltp;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_OLTP_USER;

-- Create TRY_CREATE_TABLE procedure
CREATE OR REPLACE PROCEDURE TRY_CREATE_TABLE (
    tbl_name IN VARCHAR2,
    cols_and_constraints_csv IN VARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
    table_exists NUMBER := 0;
BEGIN
    -- Ensure the procedure operates under the correct schema
    EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = ESHOP_OLTP_USER';

    LOG_DEBUG(
        'eshop_oltp/TRY_CREATE_TABLE: Trying to create table ' || tbl_name || 
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
    -- Check and create IDNT_Regions table
    TRY_CREATE_TABLE('IDNT_REGIONS', ' 
        id NUMBER PRIMARY KEY,
        name NVARCHAR2(150) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create IDNT_Cities table
    TRY_CREATE_TABLE('IDNT_CITIES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        region_id NUMBER NOT NULL,
        name NVARCHAR2(150) NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_cities_idnt_regions FOREIGN KEY (region_id) REFERENCES IDNT_Regions(id)
    ', 'oltp_create_tables');

    -- Check and create IDNT_Roles table
    TRY_CREATE_TABLE('IDNT_ROLES', ' 
        id NUMBER PRIMARY KEY,
        name VARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create IDNT_Users table
    TRY_CREATE_TABLE('IDNT_USERS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        username NVARCHAR2(25) UNIQUE NOT NULL,
        first_name NVARCHAR2(150) NOT NULL,
        last_name NVARCHAR2(150) NOT NULL,
        date_of_birth DATE NOT NULL,
        email VARCHAR2(320) UNIQUE NOT NULL,
        phone_number VARCHAR2(25),
        password VARCHAR2(255) NOT NULL,
        salt VARCHAR2(255) NOT NULL,
        region_id NUMBER DEFAULT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create IDNT_User_Roles table
    TRY_CREATE_TABLE('IDNT_USER_ROLES', ' 
        user_id NUMBER,
        role_id NUMBER,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        CONSTRAINT fk_idnt_user_roles_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_idnt_user_roles_idnt_roles FOREIGN KEY (role_id) REFERENCES IDNT_Roles(id),
        PRIMARY KEY (user_id, role_id)
    ', 'oltp_create_tables');

    -- Check and create IDNT_User_Addresses table
    TRY_CREATE_TABLE('IDNT_USER_ADDRESSES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        user_id NUMBER NOT NULL,
        city_id NUMBER NOT NULL,
        street NVARCHAR2(250) DEFAULT NULL,
        str_number NUMBER DEFAULT NULL,
        postal_code VARCHAR2(25) DEFAULT NULL,
        other_details NVARCHAR2(250) DEFAULT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_user_addresses_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_idnt_user_addresses_idnt_cities FOREIGN KEY (city_id) REFERENCES IDNT_Cities(id)
    ', 'oltp_create_tables');

    -- Check and create IDNT_Invitation_Links_With_Role table
    TRY_CREATE_TABLE('IDNT_INVITATION_LINKS_WITH_ROLE', ' 
        value VARCHAR2(150) PRIMARY KEY,
        sender_id NUMBER NOT NULL,
        role_id NUMBER DEFAULT NULL,
        expires_on DATE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_invitation_links_with_role_idnt_users FOREIGN KEY (sender_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_idnt_invitation_links_with_role_idnt_roles FOREIGN KEY (role_id) REFERENCES IDNT_Roles(id)
    ', 'oltp_create_tables');

    -- Check and create IDNT_Notifications table
    TRY_CREATE_TABLE('IDNT_NOTIFICATIONS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        user_id NUMBER NOT NULL, 
        message NVARCHAR2(850) NOT NULL,
        metadata CLOB,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_notifications_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id)
    ', 'oltp_create_tables');

    -- Check and create PCHS_Vendors table
    TRY_CREATE_TABLE('PCHS_VENDORS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name NVARCHAR2(150) UNIQUE NOT NULL,
        contact_info NVARCHAR2(850) DEFAULT NULL,
        notes NVARCHAR2(850) DEFAULT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');
    
    -- Check and create PCHS_Products table
    TRY_CREATE_TABLE('PCHS_PRODUCTS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        vendor_id NUMBER NOT NULL,
        price_in_eur NUMBER(10, 2) NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_pchs_products_pchs_vendors FOREIGN KEY (vendor_id) REFERENCES PCHS_Vendors(id), 
        CONSTRAINT chk_pchs_products_product_price_in_eur_positive 
            CHECK (price_in_eur >= 0) 
    ', 'oltp_create_tables');

    -- Check and create SLS_Products table
    TRY_CREATE_TABLE('SLS_PRODUCTS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name NVARCHAR2(150) NOT NULL,
        description NVARCHAR2(850) DEFAULT NULL,
        price_in_eur NUMBER(10, 2) NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL, 
        CONSTRAINT chk_sls_products_product_price_in_eur_positive 
            CHECK (price_in_eur >= 0) 
    ', 'oltp_create_tables');

    -- Check and create SLS_Product_Categories table
    TRY_CREATE_TABLE('SLS_PRODUCT_CATEGORIES', ' 
        id NUMBER PRIMARY KEY,
        name NVARCHAR2(150) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create SLS_Product_Subcategories table
    TRY_CREATE_TABLE('SLS_PRODUCT_SUBCATEGORIES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        category_id NUMBER NOT NULL,
        name NVARCHAR2(150) NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_product_subcategories_sls_product_categories FOREIGN KEY (category_id) REFERENCES SLS_Product_Categories(id)
    ', 'oltp_create_tables');

    -- Check and create SLS_Product_Product_Subcategories table
    TRY_CREATE_TABLE('SLS_PRODUCT_PRODUCT_SUBCATEGORIES', ' 
        product_id NUMBER,
        subcategory_id NUMBER,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        CONSTRAINT fk_sls_product_product_subcategories_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id),
        CONSTRAINT fk_sls_product_product_subcategories_sls_product_subcategories FOREIGN KEY (subcategory_id) REFERENCES SLS_Product_Subcategories(id),
        PRIMARY KEY (product_id, subcategory_id)
    ', 'oltp_create_tables');

    -- Check and create SLS_Product_Tags table
    TRY_CREATE_TABLE('SLS_PRODUCT_TAGS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name NVARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create SLS_Product_Product_Tags table
    TRY_CREATE_TABLE('SLS_PRODUCT_PRODUCT_TAGS', ' 
        product_id NUMBER,
        tag_id NUMBER,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        CONSTRAINT fk_sls_product_product_tags_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id),
        CONSTRAINT fk_sls_product_product_tags_sls_product_tags FOREIGN KEY (tag_id) REFERENCES SLS_Product_Tags(id),
        PRIMARY KEY (product_id, tag_id)
    ', 'oltp_create_tables');

    -- Check and create SLS_Discount_Types table
    TRY_CREATE_TABLE('SLS_DISCOUNT_TYPES', ' 
        id NUMBER PRIMARY KEY,
        name VARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create SLS_Discount_Reasons table
    TRY_CREATE_TABLE('SLS_DISCOUNT_REASONS', ' 
        id NUMBER PRIMARY KEY,
        name VARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create SLS_Discounts table
    TRY_CREATE_TABLE('SLS_DISCOUNTS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        product_id NUMBER NOT NULL,
        discount_type_id NUMBER NOT NULL,
        name VARCHAR2(25) NOT NULL,
        description NVARCHAR2(850) DEFAULT NULL,
        amount NUMBER(10, 2) NOT NULL,
        starts_on DATE NOT NULL,
        ends_on DATE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_discounts_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id),
        CONSTRAINT fk_sls_discounts_sls_discount_types FOREIGN KEY (discount_type_id) REFERENCES SLS_Discount_Types(id)
    ', 'oltp_create_tables');

    -- Check and create SLS_Bundle_Discounts table
    TRY_CREATE_TABLE('SLS_BUNDLE_DISCOUNTS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        discount_type_id NUMBER NOT NULL,
        name VARCHAR2(25) NOT NULL,
        description NVARCHAR2(850) DEFAULT NULL,
        amount NUMBER(10, 2) NOT NULL,
        starts_on DATE NOT NULL,
        ends_on DATE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_bundle_discounts_sls_discount_types FOREIGN KEY (discount_type_id) REFERENCES SLS_Discount_Types(id)
    ', 'oltp_create_tables');

    -- Check and create SLS_Bundle_Discount_Products table
    TRY_CREATE_TABLE('SLS_BUNDLE_DISCOUNT_PRODUCTS', ' 
        bundle_id NUMBER,
        product_id NUMBER,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        CONSTRAINT fk_sls_bundle_discount_products_sls_bundle_discounts FOREIGN KEY (bundle_id) REFERENCES SLS_Bundle_Discounts(id),
        CONSTRAINT fk_sls_bundle_discount_products_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id),
        PRIMARY KEY (bundle_id, product_id)
    ', 'oltp_create_tables');

    -- Check and create SLS_My_Wishlist_Items table
    TRY_CREATE_TABLE('SLS_MY_WISHLIST_ITEMS', ' 
        customer_id NUMBER,
        product_id NUMBER,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        CONSTRAINT fk_sls_my_wishlist_items_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_sls_my_wishlist_items_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id),
        PRIMARY KEY (customer_id, product_id)
    ', 'oltp_create_tables');

    -- Check and create SLS_My_Shopping_Cart_Items table
    TRY_CREATE_TABLE('SLS_MY_SHOPPING_CART_ITEMS', ' 
        customer_id NUMBER,
        product_id NUMBER,
        quantity NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_my_shopping_cart_items_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_sls_my_shopping_cart_items_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id), 
        CONSTRAINT chk_sls_my_shopping_cart_items_quantity_positive 
            CHECK (quantity >= 0), 
        PRIMARY KEY (customer_id, product_id)
    ', 'oltp_create_tables');

    -- Check and create SLS_Order_Statuses table
    TRY_CREATE_TABLE('SLS_ORDER_STATUSES', ' 
        id NUMBER PRIMARY KEY,
        name VARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create SLS_Orders table
    TRY_CREATE_TABLE('SLS_ORDERS', ' 
        id NUMBER PRIMARY KEY,
        customer_id NUMBER NOT NULL,
        customer_region_id NUMBER NOT NULL,
        address NVARCHAR2(850) NOT NULL,
        status_id NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_orders_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_sls_orders_sls_order_statuses FOREIGN KEY (status_id) REFERENCES SLS_Order_Statuses(id)
    ', 'oltp_create_tables');

    -- Check and create SLS_My_Discount_Coupons table
    TRY_CREATE_TABLE('SLS_MY_DISCOUNT_COUPONS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY, 
        code VARCHAR2(25) NOT NULL, 
        product_id NUMBER NOT NULL, 
        discount_type_id NUMBER NOT NULL, 
        order_id NUMBER DEFAULT NULL, 
        amount NUMBER(10, 2) NOT NULL, 
        starts_on DATE NOT NULL, 
        ends_on DATE NOT NULL, 
        discount_reason_id NUMBER NOT NULL, 
        created_on DATE DEFAULT SYSDATE NOT NULL, 
        last_updated_on DATE DEFAULT NULL, 
        CONSTRAINT fk_sls_my_discount_coupons_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id), 
        CONSTRAINT fk_sls_my_discount_coupons_sls_discount_types FOREIGN KEY (discount_type_id) REFERENCES SLS_Discount_Types(id), 
        CONSTRAINT fk_sls_my_discount_coupons_sls_orders FOREIGN KEY (order_id) REFERENCES SLS_Orders(id), 
        CONSTRAINT fk_sls_my_discount_coupons_sls_discount_reasons FOREIGN KEY (discount_reason_id) REFERENCES SLS_Discount_Reasons(id) 
    ', 'oltp_create_tables');

    -- Check and create SLS_Order_Items table
    TRY_CREATE_TABLE('SLS_ORDER_ITEMS', ' 
        order_id NUMBER,
        product_id NUMBER,
        quantity NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_order_items_sls_orders FOREIGN KEY (order_id) REFERENCES SLS_Orders(id),
        CONSTRAINT fk_sls_order_items_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id), 
        CONSTRAINT chk_sls_order_items_quantity_positive 
            CHECK (quantity >= 0), 
        PRIMARY KEY (order_id, product_id)
    ', 'oltp_create_tables');

    -- Check and create SLS_Order_Bundles table
    TRY_CREATE_TABLE('SLS_ORDER_BUNDLES', ' 
        order_id NUMBER,
        bundle_id NUMBER,
        quantity NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_order_bundles_sls_orders FOREIGN KEY (order_id) REFERENCES SLS_Orders(id),
        CONSTRAINT fk_sls_order_bundles_sls_bundle_discounts FOREIGN KEY (bundle_id) REFERENCES SLS_Bundle_Discounts(id), 
        CONSTRAINT chk_sls_order_bundles_quantity_positive 
            CHECK (quantity >= 0), 
        PRIMARY KEY (order_id, bundle_id)
    ', 'oltp_create_tables');

    -- Check and create BLG_Invoice_Statuses table
    TRY_CREATE_TABLE('BLG_INVOICE_STATUSES', ' 
        id NUMBER PRIMARY KEY,
        name VARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'oltp_create_tables');

    -- Check and create BLG_Invoices table
    TRY_CREATE_TABLE('BLG_INVOICES', ' 
        id NUMBER PRIMARY KEY,
        customer_id NUMBER NOT NULL,
        total_discount_in_eur NUMBER(10, 2) DEFAULT 0,
        status_id NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_blg_invoices_idnt_users FOREIGN KEY (customer_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_blg_invoices_blg_invoice_statuses FOREIGN KEY (status_id) REFERENCES BLG_Invoice_Statuses(id)
    ', 'oltp_create_tables');

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
    ', 'oltp_create_tables');

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
    ', 'oltp_create_tables');
    
    LOG_INFORMATION('Finished creating tables.', 'oltp_create_tables');
END;
/
