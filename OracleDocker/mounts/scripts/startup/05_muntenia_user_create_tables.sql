SET SERVEROUTPUT ON;

-- Switch to MUNTENIA PDB
ALTER SESSION SET CONTAINER = ESHOP_MUNTENIA;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_MUNTENIA_USER;

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
    EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = ESHOP_MUNTENIA_USER';

    LOG_DEBUG(
        'eshop_muntenia/TRY_CREATE_TABLE: Trying to create table ' || tbl_name || 
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
    -- Create IDNT_Regions table
    TRY_CREATE_TABLE('IDNT_REGIONS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name NVARCHAR2(150) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'muntenia_create_tables');

    -- Create IDNT_Cities table
    TRY_CREATE_TABLE('IDNT_CITIES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        region_id NUMBER NOT NULL,
        name NVARCHAR2(150) NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_cities_idnt_regions FOREIGN KEY (region_id) REFERENCES IDNT_Regions(id)
    ', 'muntenia_create_tables');

    -- Create IDNT_Roles table
    TRY_CREATE_TABLE('IDNT_ROLES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name VARCHAR2(25) UNIQUE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL
    ', 'muntenia_create_tables');

    -- Create IDNT_Users table
    TRY_CREATE_TABLE('IDNT_USERS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        username NVARCHAR2(25) UNIQUE NOT NULL,
        first_name NVARCHAR2(150) NOT NULL,
        last_name NVARCHAR2(150) NOT NULL,
        date_of_birth DATE NOT NULL,
        email VARCHAR2(320) UNIQUE NOT NULL,
        phone_number VARCHAR2(25),
        password VARCHAR2 NOT NULL,
        salt VARCHAR2 NOT NULL,
        region_id NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_users_idnt_regions FOREIGN KEY (region_id) REFERENCES IDNT_Regions(id)
    ', 'muntenia_create_tables');

    -- Create IDNT_User_Roles table
    TRY_CREATE_TABLE('IDNT_USER_ROLES', ' 
        user_id NUMBER NOT NULL,
        role_id NUMBER NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        PRIMARY KEY (user_id, role_id),
        CONSTRAINT fk_idnt_user_roles_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_idnt_user_roles_idnt_roles FOREIGN KEY (role_id) REFERENCES IDNT_Roles(id)
    ', 'muntenia_create_tables');

    -- Create IDNT_User_Addresses table
    TRY_CREATE_TABLE('IDNT_USER_ADDRESSES', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        user_id NUMBER NOT NULL,
        city_id NUMBER NOT NULL,
        street NVARCHAR2(250) DEFAULT NULL,
        number NUMBER DEFAULT NULL,
        postal_code VARCHAR2(25) DEFAULT NULL,
        other_details NVARCHAR2(250) DEFAULT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_user_addresses_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_idnt_user_addresses_idnt_cities FOREIGN KEY (city_id) REFERENCES IDNT_Cities(id)
    ', 'muntenia_create_tables');

    -- Create IDNT_Invitation_Links_With_Role table
    TRY_CREATE_TABLE('IDNT_INVITATION_LINKS_WITH_ROLE', ' 
        value VARCHAR2(150) PRIMARY KEY,
        sender_id NUMBER NOT NULL,
        role_id NUMBER DEFAULT NULL,
        expires_on DATE NOT NULL,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_invitation_links_with_role_idnt_users FOREIGN KEY (sender_id) REFERENCES IDNT_Users(id),
        CONSTRAINT fk_idnt_invitation_links_with_role_idnt_roles FOREIGN KEY (role_id) REFERENCES IDNT_Roles(id)
    ', 'muntenia_create_tables');

    -- Create IDNT_Notifications table
    TRY_CREATE_TABLE('IDNT_NOTIFICATIONS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        user_id NUMBER NOT NULL,
        message NVARCHAR2(850) NOT NULL,
        metadata CLOB,
        created_on DATE DEFAULT SYSDATE NOT NULL,
        last_updated_on DATE DEFAULT NULL,
        CONSTRAINT fk_idnt_notifications_idnt_users FOREIGN KEY (user_id) REFERENCES IDNT_Users(id)
    ', 'muntenia_create_tables');

    --LOG_INFORMATION('Finished creating additional muntenia tables.', 'muntenia_create_tables');
END;
/