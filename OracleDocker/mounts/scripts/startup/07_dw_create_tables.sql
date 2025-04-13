SET SERVEROUTPUT ON;

-- Switch to DW PDB
ALTER SESSION SET CONTAINER = eshop_dw;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_DW_USER;

-- Create TRY_CREATE_TABLE procedure
CREATE OR REPLACE PROCEDURE TRY_CREATE_TABLE (
    tbl_name IN VARCHAR2,
    cols_and_constraints_csv IN VARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
    table_exists NUMBER := 0;
BEGIN
    -- Ensure the procedure operates under the correct schema
    EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = ESHOP_DW_USER';

    LOG_DEBUG(
        'eshop_dw/TRY_CREATE_TABLE: Trying to create table ' || tbl_name || 
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
    EXECUTE IMMEDIATE '
    -- Check and create DWH_DIM_Time table
    CREATE TABLE DWH_DIM_TIME (
        dw_id NUMBER,  -- Custom numeric date format yyyymmdd
        full_date DATE, 
        year NUMBER, 
        quarter NUMBER, 
        month NUMBER, 
        day NUMBER, 
        day_of_week NVARCHAR2(50), 
        is_weekend NUMBER(1), 
        created_on DATE DEFAULT SYSDATE, 
        CONSTRAINT pk_dwh_dim_time PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    )
    PARTITION BY RANGE (dw_id) (
        PARTITION time_jan_2022 VALUES LESS THAN (20220201),
        PARTITION time_feb_2022 VALUES LESS THAN (20220301),
        PARTITION time_mar_2022 VALUES LESS THAN (20220401),
        PARTITION time_apr_2022 VALUES LESS THAN (20220501),
        PARTITION time_may_2022 VALUES LESS THAN (20220601),
        PARTITION time_jun_2022 VALUES LESS THAN (20220701),
        PARTITION time_jul_2022 VALUES LESS THAN (20220801),
        PARTITION time_aug_2022 VALUES LESS THAN (20220901),
        PARTITION time_sep_2022 VALUES LESS THAN (20221001),
        PARTITION time_oct_2022 VALUES LESS THAN (20221101),
        PARTITION time_nov_2022 VALUES LESS THAN (20221201),
        PARTITION time_dec_2022 VALUES LESS THAN (20230101),
        PARTITION time_jan_2023 VALUES LESS THAN (20230201),
        PARTITION time_feb_2023 VALUES LESS THAN (20230301),
        PARTITION time_mar_2023 VALUES LESS THAN (20230401),
        PARTITION time_apr_2023 VALUES LESS THAN (20230501),
        PARTITION time_may_2023 VALUES LESS THAN (20230601),
        PARTITION time_jun_2023 VALUES LESS THAN (20230701),
        PARTITION time_jul_2023 VALUES LESS THAN (20230801),
        PARTITION time_aug_2023 VALUES LESS THAN (20230901),
        PARTITION time_sep_2023 VALUES LESS THAN (20231001),
        PARTITION time_oct_2023 VALUES LESS THAN (20231101),
        PARTITION time_nov_2023 VALUES LESS THAN (20231201),
        PARTITION time_dec_2023 VALUES LESS THAN (20240101),
        PARTITION time_jan_2024 VALUES LESS THAN (20240201),
        PARTITION time_feb_2024 VALUES LESS THAN (20240301),
        PARTITION time_mar_2024 VALUES LESS THAN (20240401),
        PARTITION time_apr_2024 VALUES LESS THAN (20240501),
        PARTITION time_may_2024 VALUES LESS THAN (20240601),
        PARTITION time_jun_2024 VALUES LESS THAN (20240701),
        PARTITION time_jul_2024 VALUES LESS THAN (20240801),
        PARTITION time_aug_2024 VALUES LESS THAN (20240901),
        PARTITION time_sep_2024 VALUES LESS THAN (20241001),
        PARTITION time_oct_2024 VALUES LESS THAN (20241101),
        PARTITION time_nov_2024 VALUES LESS THAN (20241201),
        PARTITION time_dec_2024 VALUES LESS THAN (20250101),
        PARTITION time_jan_2025 VALUES LESS THAN (20250201),
        PARTITION time_feb_2025 VALUES LESS THAN (20250301),
        PARTITION time_mar_2025 VALUES LESS THAN (20250401),
        PARTITION time_apr_2025 VALUES LESS THAN (20250501),
        PARTITION time_may_2025 VALUES LESS THAN (20250601),
        PARTITION time_jun_2025 VALUES LESS THAN (20250701),
        PARTITION time_jul_2025 VALUES LESS THAN (20250801),
        PARTITION time_aug_2025 VALUES LESS THAN (20250901),
        PARTITION time_sep_2025 VALUES LESS THAN (20251001),
        PARTITION time_oct_2025 VALUES LESS THAN (20251101),
        PARTITION time_nov_2025 VALUES LESS THAN (20251201),
        PARTITION time_dec_2025 VALUES LESS THAN (20260101),
        PARTITION time_max VALUES LESS THAN (MAXVALUE)
    )
    ';

    -- Check and create DWH_DIM_Regions table
    TRY_CREATE_TABLE('DWH_DIM_REGIONS', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        name NVARCHAR2(150),
        created_on DATE DEFAULT SYSDATE, 
        CONSTRAINT pk_dwh_dim_regions PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_DIM_Customers table
    TRY_CREATE_TABLE('DWH_DIM_CUSTOMERS', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        first_name NVARCHAR2(150),
        last_name NVARCHAR2(150),
        date_of_birth DATE,
        created_on DATE DEFAULT SYSDATE, 
        CONSTRAINT pk_dwh_dim_customers PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_DIM_Categories table
    TRY_CREATE_TABLE('DWH_DIM_CATEGORIES', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        name NVARCHAR2(150),
        created_on DATE DEFAULT SYSDATE, 
        CONSTRAINT pk_dwh_dim_categories PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_DIM_Subcategories table
    TRY_CREATE_TABLE('DWH_DIM_SUBCATEGORIES', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        category_id NUMBER,
        name NVARCHAR2(150),
        created_on DATE DEFAULT SYSDATE,
        CONSTRAINT fk_dwh_dim_subcategories_dwh_dim_categories 
            FOREIGN KEY (category_id) 
            REFERENCES DWH_DIM_Categories(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT pk_dwh_dim_subcategories PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_DIM_Products table
    TRY_CREATE_TABLE('DWH_DIM_PRODUCTS', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        category_id NUMBER,
        subcategory_id NUMBER,
        name NVARCHAR2(150),
        description NVARCHAR2(850),
        created_on DATE DEFAULT SYSDATE,
        valid_from DATE,
        valid_to DATE,
        is_current NUMBER(1),
        CONSTRAINT fk_dwh_dim_products_dwh_dim_categories 
            FOREIGN KEY (category_id) 
            REFERENCES DWH_DIM_Categories(dw_id)
            RELY DISABLE NOVALIDATE,
        CONSTRAINT fk_dwh_dim_products_dwh_dim_subcategories 
            FOREIGN KEY (subcategory_id) 
            REFERENCES DWH_DIM_Subcategories(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT pk_dwh_dim_products PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_DIM_Discounts table
    TRY_CREATE_TABLE('DWH_DIM_DISCOUNTS', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        product_id NUMBER,
        name VARCHAR2(25),
        description NVARCHAR2(850),
        starts_on DATE,
        ends_on DATE,
        created_on DATE DEFAULT SYSDATE,
        valid_from DATE,
        valid_to DATE,
        is_current NUMBER(1),
        CONSTRAINT fk_dwh_dim_discounts_dwh_dim_products 
            FOREIGN KEY (product_id) 
            REFERENCES DWH_DIM_Products(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT pk_dwh_dim_discounts PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_DIM_Bundles table
    TRY_CREATE_TABLE('DWH_DIM_BUNDLES', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        name VARCHAR2(25),
        description NVARCHAR2(850),
        starts_on DATE,
        ends_on DATE,
        created_on DATE DEFAULT SYSDATE,
        valid_from DATE,
        valid_to DATE,
        is_current NUMBER(1), 
        CONSTRAINT pk_dwh_dim_bundles PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_DIM_Coupons table
    TRY_CREATE_TABLE('DWH_DIM_COUPONS', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        product_id NUMBER,
        reason VARCHAR2(25),
        starts_on DATE,
        ends_on DATE,
        created_on DATE DEFAULT SYSDATE,
        valid_from DATE,
        valid_to DATE,
        is_current NUMBER(1),
        CONSTRAINT fk_dwh_dim_coupons_dwh_dim_products 
            FOREIGN KEY (product_id) 
            REFERENCES DWH_DIM_Products(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT pk_dwh_dim_coupons PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_DIM_Invoices table
    TRY_CREATE_TABLE('DWH_DIM_INVOICES', ' 
        dw_id NUMBER GENERATED ALWAYS AS IDENTITY,
        id NUMBER,
        status VARCHAR2(25),
        created_on DATE DEFAULT SYSDATE,
        valid_from DATE,
        valid_to DATE,
        is_current NUMBER(1), 
        CONSTRAINT pk_dwh_dim_invoices PRIMARY KEY (dw_id) RELY DISABLE NOVALIDATE 
    ', 'dw_create_tables');

    -- Check and create DWH_FACT_Invoice_Items table
    TRY_CREATE_TABLE('DWH_FACT_INVOICE_ITEMS', ' 
        product_id NUMBER,
        invoice_id NUMBER,
        region_id NUMBER,
        time_id NUMBER,
        product_price_in_eur NUMBER(10, 2),
        products_applied_discount_in_eur NUMBER(10, 2),
        products_price_after_discount_in_eur NUMBER(10, 2),
        quantity NUMBER,
        created_on DATE DEFAULT SYSDATE,
        CONSTRAINT fk_dwh_fact_invoice_items_dwh_dim_products 
            FOREIGN KEY (product_id) 
            REFERENCES DWH_DIM_Products(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_invoice_items_dwh_dim_invoices 
            FOREIGN KEY (invoice_id) 
            REFERENCES DWH_DIM_Invoices(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_invoice_items_dwh_dim_regions 
            FOREIGN KEY (region_id) 
            REFERENCES DWH_DIM_Regions(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_invoice_items_dwh_dim_time 
            FOREIGN KEY (time_id) 
            REFERENCES DWH_DIM_Time(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT chk_dwh_fact_invoice_items_product_price_in_eur_positive 
            CHECK (product_price_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_invoice_items_products_applied_discount_in_eur_positive 
            CHECK (products_applied_discount_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_invoice_items_products_price_after_discount_in_eur_positive 
            CHECK (products_price_after_discount_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_invoice_items_quantity_positive 
            CHECK (quantity >= 0) 
            RELY, 
        PRIMARY KEY (product_id, invoice_id)
    ', 'dw_create_tables');

    -- Check and create DWH_FACT_Invoice_Bundles table
    TRY_CREATE_TABLE('DWH_FACT_INVOICE_BUNDLES', ' 
        bundle_id NUMBER,
        invoice_id NUMBER,
        region_id NUMBER,
        time_id NUMBER,
        bundle_price_in_eur NUMBER(10, 2),
        bundles_applied_discount_in_eur NUMBER(10, 2),
        bundles_price_after_discount_in_eur NUMBER(10, 2),
        quantity NUMBER,
        created_on DATE DEFAULT SYSDATE,
        CONSTRAINT fk_dwh_fact_invoice_bundles_dwh_dim_bundles 
            FOREIGN KEY (bundle_id) 
            REFERENCES DWH_DIM_Bundles(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_invoice_bundles_dwh_dim_invoices 
            FOREIGN KEY (invoice_id) 
            REFERENCES DWH_DIM_Invoices(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_invoice_bundles_dwh_dim_regions 
            FOREIGN KEY (region_id) 
            REFERENCES DWH_DIM_Regions(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_invoice_bundles_dwh_dim_time 
            FOREIGN KEY (time_id) 
            REFERENCES DWH_DIM_Time(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT chk_dwh_fact_invoice_bundles_bundle_price_in_eur_positive 
            CHECK (bundle_price_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_invoice_bundles_bundles_applied_discount_in_eur_positive 
            CHECK (bundles_applied_discount_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_invoice_bundles_bundles_price_after_discount_in_eur_positive 
            CHECK (bundles_price_after_discount_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_invoice_bundles_quantity_positive 
            CHECK (quantity >= 0) 
            RELY, 
        PRIMARY KEY (bundle_id, invoice_id)
    ', 'dw_create_tables');

    -- Check and create DWH_FACT_Applied_Discounts table
    TRY_CREATE_TABLE('DWH_FACT_APPLIED_DISCOUNTS', ' 
        discount_id NUMBER,
        product_id NUMBER,
        invoice_id NUMBER,
        customer_id NUMBER,
        region_id NUMBER,
        time_id NUMBER,
        product_price_in_eur NUMBER(10, 2),
        products_applied_discount_in_eur NUMBER(10, 2),
        products_price_after_discount_in_eur NUMBER(10, 2),
        quantity NUMBER,
        created_on DATE DEFAULT SYSDATE,
        CONSTRAINT fk_dwh_fact_applied_discounts_dwh_dim_discounts 
            FOREIGN KEY (discount_id) 
            REFERENCES DWH_DIM_Discounts(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_discounts_dwh_dim_products 
            FOREIGN KEY (product_id) 
            REFERENCES DWH_DIM_Products(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_discounts_dwh_dim_invoices 
            FOREIGN KEY (invoice_id) 
            REFERENCES DWH_DIM_Invoices(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_discounts_dwh_dim_customers 
            FOREIGN KEY (customer_id) 
            REFERENCES DWH_DIM_Customers(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_discounts_dwh_dim_regions 
            FOREIGN KEY (region_id) 
            REFERENCES DWH_DIM_Regions(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_discounts_dwh_dim_time 
            FOREIGN KEY (time_id) 
            REFERENCES DWH_DIM_Time(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT chk_dwh_fact_applied_discounts_product_price_in_eur_positive 
            CHECK (product_price_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_applied_discounts_products_applied_discount_in_eur_positive 
            CHECK (products_applied_discount_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_applied_discounts_products_price_after_discount_in_eur_positive 
            CHECK (products_price_after_discount_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_applied_discounts_quantity_positive 
            CHECK (quantity >= 0) 
            RELY, 
        PRIMARY KEY (discount_id, product_id, invoice_id)
    ', 'dw_create_tables');

    -- Check and create DWH_FACT_Applied_Coupons table
    TRY_CREATE_TABLE('DWH_FACT_APPLIED_COUPONS', ' 
        coupon_id NUMBER,
        product_id NUMBER,
        invoice_id NUMBER,
        customer_id NUMBER,
        region_id NUMBER,
        time_id NUMBER,
        product_price_in_eur NUMBER(10, 2),
        products_applied_discount_in_eur NUMBER(10, 2),
        products_price_after_discount_in_eur NUMBER(10, 2),
        created_on DATE DEFAULT SYSDATE,
        CONSTRAINT fk_dwh_fact_applied_coupons_dwh_dim_coupons 
            FOREIGN KEY (coupon_id) 
            REFERENCES DWH_DIM_Coupons(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_coupons_dwh_dim_products 
            FOREIGN KEY (product_id) 
            REFERENCES DWH_DIM_Products(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_coupons_dwh_dim_invoices 
            FOREIGN KEY (invoice_id) 
            REFERENCES DWH_DIM_Invoices(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_coupons_dwh_dim_customers 
            FOREIGN KEY (customer_id) 
            REFERENCES DWH_DIM_Customers(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_coupons_dwh_dim_regions 
            FOREIGN KEY (region_id) 
            REFERENCES DWH_DIM_Regions(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT fk_dwh_fact_applied_coupons_dwh_dim_time 
            FOREIGN KEY (time_id) 
            REFERENCES DWH_DIM_Time(dw_id) 
            RELY DISABLE NOVALIDATE, 
        CONSTRAINT chk_dwh_fact_applied_coupons_product_price_in_eur_positive 
            CHECK (product_price_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_applied_coupons_products_applied_discount_in_eur_positive 
            CHECK (products_applied_discount_in_eur >= 0) 
            RELY, 
        CONSTRAINT chk_dwh_fact_applied_coupons_products_price_after_discount_in_eur_positive 
            CHECK (products_price_after_discount_in_eur >= 0) 
            RELY, 
        -- -- A coupon is applied only once, on a single unit of a product
        -- CONSTRAINT chk_dwh_fact_applied_coupons_quantity_positive 
        --     CHECK (quantity >= 0) 
        --     RELY, 
        PRIMARY KEY (coupon_id, product_id, invoice_id)
    ', 'dw_create_tables');
    
    LOG_INFORMATION('Finished creating tables.', 'dw_create_tables');
END;
/
