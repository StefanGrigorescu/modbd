SET SERVEROUTPUT ON;

-- Switch to GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER;

CREATE OR REPLACE FUNCTION New_Snowflake_Id (
    p_now IN TIMESTAMP,
    p_region_id IN NUMBER,
    p_random IN NUMBER
) RETURN NUMBER IS
BEGIN
    -- Generate the order ID in the format "{year:4}{month:2}{day:2}{region_id:2}{random:12}"
    RETURN TO_NUMBER(
        TO_CHAR(p_now, 'YYYYMMDD') || 
        LPAD(p_region_id, 2, '0') || 
        LPAD(p_random, 12, '0')
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

-- Modified INSERT_USER procedure for fragmented IDNT_USERS table
CREATE OR REPLACE PROCEDURE INSERT_USER (
    p_username IN NVARCHAR2,
    p_first_name IN NVARCHAR2,
    p_last_name IN NVARCHAR2,
    p_date_of_birth IN DATE,
    p_email IN VARCHAR2,
    p_phone_number IN VARCHAR2,
    p_password IN VARCHAR2,
    p_salt IN VARCHAR2,
    p_roles_csv IN VARCHAR2,
    p_region IN VARCHAR2, -- Region to determine which regional table to insert into
    p_now IN DATE DEFAULT SYSDATE,
    p_random IN NUMBER DEFAULT DBMS_RANDOM.VALUE,
    p_user_created OUT NUMBER 
) IS
    user_exists NUMBER := 0;
    user_id NUMBER;
    region_id NUMBER;
BEGIN
    -- Check if the user already exists by username or email
    SELECT COUNT(*) 
    INTO user_exists
    FROM IDNT_USERS
    WHERE UPPER(email) = UPPER(p_email);

    IF user_exists = 0 THEN
        SELECT COUNT(*) INTO user_exists
        FROM IDNT_USERS@ESHOP_MUNTENIA_LINK
        WHERE UPPER(username) = UPPER(p_username);
    END IF;
    
    IF user_exists = 0 THEN
        SELECT COUNT(*) INTO user_exists
        FROM IDNT_USERS@ESHOP_ROMANIA_LINK
        WHERE UPPER(username) = UPPER(p_username);
    END IF;

    IF user_exists = 0 THEN
        -- Find region id by name
        BEGIN
            SELECT id 
            INTO region_id
            FROM IDNT_REGIONS
            WHERE UPPER(name) = UPPER(p_region);
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                LOG_ERROR('INSERT_USER: Region ' || p_region || ' not found.');
                p_user_created := -1; -- Error occurred (region not found)
                RETURN;
        END;

        user_id := New_Snowflake_Id (
            p_region_id => region_id,
            p_now => p_now,
            p_random => p_random
        );
        
        -- Insert login info into the global IDNT_USERS table
        INSERT INTO IDNT_USERS (
            id, email, password, salt
        ) VALUES (
            user_id, p_email, p_password, p_salt
        ) RETURNING id INTO user_id;

        -- Log success for global insertion
        LOG_INFORMATION('INSERT_USER: Login info for user ' || p_email || ' created in global.');

        IF region_id = 0 THEN
            INSERT INTO IDNT_USERS@ESHOP_MUNTENIA_LINK (
                id, username, first_name, last_name, date_of_birth, phone_number
            ) VALUES (
                user_id, p_username, p_first_name, p_last_name, p_date_of_birth, p_phone_number
            );
        ELSE
            INSERT INTO IDNT_USERS@ESHOP_ROMANIA_LINK (
                id, username, first_name, last_name, date_of_birth, phone_number
            ) VALUES (
                user_id, p_username, p_first_name, p_last_name, p_date_of_birth, p_phone_number
            );
        END IF;

        -- Log success for regional insertion
        LOG_INFORMATION('INSERT_USER: Profile info for user ' || p_email || ' created in region ' || p_region);

        -- Split the roles CSV and insert each role
        FOR role_name IN (SELECT REGEXP_SUBSTR(p_roles_csv, '[^,]+', 1, LEVEL) AS role_name
                        FROM DUAL
                        CONNECT BY REGEXP_SUBSTR(p_roles_csv, '[^,]+', 1, LEVEL) IS NOT NULL
        ) LOOP
            INSERT_USER_ROLE(user_id, role_name.role_name);
        END LOOP;

        -- Set OUT parameter
        p_user_created := 1; -- User was successfully created
    ELSE
        LOG_DEBUG('INSERT_USER: User with email ' || p_email || ' already exists.');
        p_user_created := 0; -- User already exists
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_USER: Error creating user ' || p_email || ': ' || SQLERRM);
        ROLLBACK;
        p_user_created := -1; -- Error occurred
END;
/

CREATE OR REPLACE PROCEDURE INSERT_USER_ROLE (
    p_user_id IN NUMBER,
    p_role_name IN VARCHAR2
) IS
    role_id NUMBER;
BEGIN
    -- Check if the role exists
    BEGIN
        SELECT id INTO role_id
        FROM IDNT_ROLES
        WHERE UPPER(name) = UPPER(p_role_name);
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            role_id := NULL;
            LOG_ERROR('INSERT_USER_ROLE: Role ' || p_role_name || ' does not exist. No role assigned to user ID ' || p_user_id || '.');
            RETURN;
    END;

    INSERT INTO IDNT_USER_ROLES (user_id, role_id) 
    VALUES (p_user_id, role_id);
    
    LOG_INFORMATION('INSERT_USER_ROLE: Role ' || p_role_name || ' assigned to user ID ' || p_user_id);
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_USER_ROLE: Error assigning role ' || p_role_name || ' to user ID ' || p_user_id || ': ' || SQLERRM);
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
