SET SERVEROUTPUT ON;

-- Switch to GLOBAL PDB
ALTER SESSION SET CONTAINER = ESHOP_GLOBAL;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER;

-- Create TRY_CREATE_TABLE procedure
CREATE OR REPLACE PROCEDURE TRY_CREATE_TABLE (
    tbl_name IN VARCHAR2,
    cols_and_constraints_csv IN VARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
    table_exists NUMBER := 0;
BEGIN
    -- Ensure the procedure operates under the correct schema
    EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER';

    -- Debug log (optional, remove if not needed)
    DBMS_OUTPUT.PUT_LINE(
        'eshop_global/TRY_CREATE_TABLE: Trying to create table ' || tbl_name || 
        ' | Container = ' || SYS_CONTEXT('USERENV', 'CON_NAME') || 
        ' | Schema = ' || SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA') || 
        ' | Connected as = ' || SYS_CONTEXT('USERENV', 'SESSION_USER')
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
        DBMS_OUTPUT.PUT_LINE('TRY_CREATE_TABLE: Table ' || tbl_name || ' created.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('TRY_CREATE_TABLE: Table ' || tbl_name || ' already exists.');
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('TRY_CREATE_TABLE: Error creating table ' || tbl_name || ': ' || SQLERRM);
END;
/

BEGIN
    -- Create SLS_Products table
    TRY_CREATE_TABLE('SLS_PRODUCTS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name NVARCHAR2(150) NOT NULL,
        description NVARCHAR2(850) DEFAULT NULL,
        price_in_eur DECIMAL NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'global_create_tables');

    -- Create SLS_Product_Categories table
    TRY_CREATE_TABLE('SLS_PRODUCT_CATEGORIES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name NVARCHAR2(150) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'global_create_tables');

    -- Create SLS_Product_Subcategories table
    TRY_CREATE_TABLE('SLS_PRODUCT_SUBCATEGORIES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        category_id NUMBER NOT NULL,
        name NVARCHAR2(150) NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_product_subcategories_sls_product_categories FOREIGN KEY (category_id) REFERENCES SLS_Product_Categories(id)
    ', 'global_create_tables');

    -- Create SLS_Product_Tags table
    TRY_CREATE_TABLE('SLS_PRODUCT_TAGS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name NVARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'global_create_tables');

    -- Create SLS_Product_Product_Tags table
    TRY_CREATE_TABLE('SLS_PRODUCT_PRODUCT_TAGS', ' 
        product_id NUMBER NOT NULL,
        tag_id NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        PRIMARY KEY (product_id, tag_id),
        CONSTRAINT fk_sls_product_product_tags_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id),
        CONSTRAINT fk_sls_product_product_tags_sls_product_tags FOREIGN KEY (tag_id) REFERENCES SLS_Product_Tags(id)
    ', 'global_create_tables');

    -- Create SLS_Discounts table
    TRY_CREATE_TABLE('SLS_DISCOUNTS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        product_id NUMBER NOT NULL,
        discount_type_id NUMBER NOT NULL,
        name VARCHAR2(25) NOT NULL,
        description NVARCHAR2(850) DEFAULT NULL,
        amount DECIMAL NOT NULL,
        starts_on DATE NOT NULL,
        ends_on DATE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_discounts_sls_products FOREIGN KEY (product_id) REFERENCES SLS_Products(id),
        CONSTRAINT fk_sls_discounts_sls_discount_types FOREIGN KEY (discount_type_id) REFERENCES SLS_Discount_Types(id)
    ', 'global_create_tables');

    -- Create SLS_Bundle_Discounts table
    TRY_CREATE_TABLE('SLS_BUNDLE_DISCOUNTS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        discount_type_id NUMBER NOT NULL,
        name VARCHAR2(25) NOT NULL,
        description NVARCHAR2(850) DEFAULT NULL,
        amount DECIMAL NOT NULL,
        starts_on DATE NOT NULL,
        ends_on DATE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_sls_bundle_discounts_sls_discount_types FOREIGN KEY (discount_type_id) REFERENCES SLS_Discount_Types(id)
    ', 'global_create_tables');

    --LOG_INFORMATION('Finished creating additional global tables.', 'global_create_tables');
END;
/