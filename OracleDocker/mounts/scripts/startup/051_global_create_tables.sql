SET SERVEROUTPUT ON;

-- Switch to GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER;

CREATE OR REPLACE PROCEDURE TRY_CREATE_TABLE (
    tbl_name IN VARCHAR2,
    cols_and_constraints_csv IN VARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
    table_exists NUMBER := 0;
BEGIN
    -- Ensure the procedure operates under the correct schema
    EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER';

    LOG_DEBUG(
        'eshop_global/TRY_CREATE_TABLE: Trying to create table ' || tbl_name || 
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
    EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER';

    LOG_DEBUG(
        'eshop_global/TRY_CREATE_SHARD: Trying to create shard ' || shard_name || 
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
        'IDNT_REGIONS',
        'SELECT *
        FROM IDNT_REGIONS@eshop_oltp_link',
        '', 'global_create_tables');

    TRY_CREATE_SHARD(
        'IDNT_CITIES',
        'SELECT *
        FROM IDNT_CITIES@eshop_oltp_link',
        '
        ALTER TABLE IDNT_CITIES ADD CONSTRAINT fk_idnt_cities_idnt_regions FOREIGN KEY (region_id) REFERENCES IDNT_Regions(id);
        ', 'global_create_tables');

    TRY_CREATE_SHARD(
        'IDNT_ROLES',
        'SELECT *
        FROM IDNT_ROLES@eshop_oltp_link',
        '', 'global_create_tables');
    
    TRY_CREATE_SHARD(
        'IDNT_USERS',
        'SELECT 
            id,
            email,
            password,
            salt
        FROM IDNT_USERS@eshop_oltp_link
        ', 'global_create_tables');

    TRY_CREATE_SHARD(
        'IDNT_USER_ROLES',
        'SELECT *
        FROM IDNT_USER_ROLES@eshop_oltp_link',
        '
        ALTER TABLE IDNT_USER_ROLES ADD CONSTRAINT fk_idnt_user_roles_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id);
        ALTER TABLE IDNT_USER_ROLES ADD CONSTRAINT fk_idnt_user_roles_idnt_roles FOREIGN KEY (role_id) REFERENCES IDNT_Roles(id);
        ', 'global_create_tables');

    TRY_CREATE_SHARD(
        'IDNT_USER_ADDRESSES',
        'SELECT *
        FROM IDNT_USER_ADDRESSES@eshop_oltp_link',
        '
        ALTER TABLE IDNT_USER_ADDRESSES ADD CONSTRAINT fk_idnt_user_addresses_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id);
        ALTER TABLE IDNT_USER_ADDRESSES ADD CONSTRAINT fk_idnt_user_addresses_idnt_cities FOREIGN KEY (city_id) REFERENCES IDNT_Cities(id);
        ', 'global_create_tables');

    TRY_CREATE_SHARD(
        'IDNT_INVITATION_LINKS_WITH_ROLE',
        'SELECT *
        FROM IDNT_INVITATION_LINKS_WITH_ROLE@eshop_oltp_link',
        '
        ALTER TABLE IDNT_INVITATION_LINKS_WITH_ROLE ADD CONSTRAINT fk_idnt_invitation_links_with_role_idnt_users FOREIGN KEY (sender_id) REFERENCES IDNT_Users(id);
        ALTER TABLE IDNT_INVITATION_LINKS_WITH_ROLE ADD CONSTRAINT fk_idnt_invitation_links_with_role_idnt_roles FOREIGN KEY (role_id) REFERENCES IDNT_Roles(id);
        ', 'global_create_tables');

    TRY_CREATE_SHARD(
        'IDNT_NOTIFICATIONS',
        'SELECT *
        FROM IDNT_NOTIFICATIONS@eshop_oltp_link',
        '
        ALTER TABLE IDNT_NOTIFICATIONS ADD CONSTRAINT fk_idnt_notifications_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id);
        ', 'global_create_tables');

    TRY_CREATE_SHARD(
        'PCHS_VENDORS',
        'SELECT *
        FROM PCHS_VENDORS@eshop_oltp_link',
        '', 'global_create_tables');

    TRY_CREATE_SHARD(
        'PCHS_PRODUCTS',
        'SELECT *
        FROM PCHS_PRODUCTS@eshop_oltp_link',
        '
        ALTER TABLE PCHS_PRODUCTS ADD CONSTRAINT fk_pchs_products_pchs_vendors FOREIGN KEY (vendor_id) REFERENCES PCHS_Vendors(id); 
        ALTER TABLE PCHS_PRODUCTS ADD CONSTRAINT chk_pchs_products_product_price_in_eur_positive CHECK (price_in_eur >= 0);
        ', 'global_create_tables');
    
    LOG_INFORMATION('Finished creating tables.', 'global_create_tables');
END;
/


CREATE OR REPLACE VIEW IDNT_CUSTOMERS AS
    SELECT u.*
    FROM IDNT_USERS u
    WHERE NOT EXISTS (
        SELECT 1
        FROM IDNT_USER_ROLES ur
        WHERE u.id = ur.user_id
    );
/
