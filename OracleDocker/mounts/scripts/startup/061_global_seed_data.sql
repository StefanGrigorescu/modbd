SET SERVEROUTPUT ON;

-- Switch to GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER;

-- Create procedure to insert a city
CREATE OR REPLACE PROCEDURE INSERT_CITY (
    p_region_id IN NUMBER,
    p_city_name IN NVARCHAR2
) IS
    city_exists NUMBER := 0;
BEGIN
    -- Check if the city already exists
    SELECT COUNT(*) INTO city_exists
    FROM IDNT_CITIES
    WHERE UPPER(name) = UPPER(p_city_name)
    AND region_id = p_region_id;

    IF city_exists = 0 THEN
        -- Insert the city if it doesn't exist
        INSERT INTO IDNT_CITIES (region_id, name)
        VALUES (p_region_id, p_city_name);
        LOG_INFORMATION('INSERT_CITY: City ' || p_city_name || ' created.');
    ELSE
        LOG_DEBUG('INSERT_CITY: City ' || p_city_name || ' already exists.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_CITY: Error creating city ' || p_city_name || ': ' || SQLERRM);
END;
/

-- Create procedure to insert a region and its cities
CREATE OR REPLACE PROCEDURE INSERT_REGION_AND_CITIES (
    p_region_id IN NUMBER,
    p_region_name IN NVARCHAR2,
    p_city_names_csv IN NVARCHAR2
) IS
    region_exists_by_id NUMBER := 0;
    region_exists_by_name NUMBER := 0;
BEGIN
    SELECT 
        COUNT(CASE WHEN id = p_region_id THEN 1 END), COUNT(CASE WHEN UPPER(name) = UPPER(p_region_name) THEN 1 END) 
        INTO region_exists_by_id, region_exists_by_name
    FROM IDNT_REGIONS;

    -- Ensure both ID and name are unique
    IF region_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_REGION_AND_CITIES: Region ID ' || p_region_id || ' already exists.');
    ELSIF region_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_REGION_AND_CITIES: Region name ' || p_region_name || ' already exists.');
    ELSE
        -- Insert the region if both ID and name are unique
        INSERT INTO IDNT_REGIONS (id, name)
        VALUES (p_region_id, p_region_name);
        LOG_INFORMATION('INSERT_REGION_AND_CITIES: Region ' || p_region_name || ' with ID ' || p_region_id || ' created.');

        -- Split the city names CSV and insert each city
        FOR city_name IN (SELECT REGEXP_SUBSTR(p_city_names_csv, '[^,]+', 1, LEVEL) AS city_name
                          FROM DUAL
                          CONNECT BY REGEXP_SUBSTR(p_city_names_csv, '[^,]+', 1, LEVEL) IS NOT NULL) LOOP
            INSERT_CITY(p_region_id, city_name.city_name);
        END LOOP;

        COMMIT;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_REGION_AND_CITIES: Error creating region ' || p_region_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

-- Insert regions and cities
BEGIN
    INSERT_REGION_AND_CITIES(0, 'Muntenia', 'Bucuresti,Ploiesti,Giurgiu');
    INSERT_REGION_AND_CITIES(1, 'Transilvania', 'Cluj-Napoca,Sibiu');
    INSERT_REGION_AND_CITIES(2, 'Moldova', 'Iasi,Bacau');
    INSERT_REGION_AND_CITIES(3, 'Dobrogea', 'Constanta,Tulcea');
    INSERT_REGION_AND_CITIES(4, 'Banat', 'Timisoara,Resita');
    INSERT_REGION_AND_CITIES(5, 'Crisana', 'Oradea,Arad');
    INSERT_REGION_AND_CITIES(6, 'Maramures', 'Baia Mare,Sighetu Marmatiei');
    INSERT_REGION_AND_CITIES(7, 'Oltenia', 'Craiova,Targu Jiu');
    INSERT_REGION_AND_CITIES(8, 'Bucovina', 'Radauti,Campulung Moldovenesc');
END;
/

-- Create procedure to insert a role
CREATE OR REPLACE PROCEDURE INSERT_ROLE (
    p_role_id IN NUMBER,
    p_role_name IN VARCHAR2
) IS
    role_exists_by_id NUMBER := 0;
    role_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the role ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_role_id THEN 1 END), COUNT(CASE WHEN UPPER(name) = UPPER(p_role_name) THEN 1 END) 
        INTO role_exists_by_id, role_exists_by_name
    FROM IDNT_ROLES;

    -- Ensure both ID and name are unique
    IF role_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_ROLE: Role ID ' || p_role_id || ' already exists.');
    ELSIF role_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_ROLE: Role name ' || p_role_name || ' already exists.');
    ELSE
        -- Insert the role if both ID and name are unique
        INSERT INTO IDNT_ROLES (id, name)
        VALUES (p_role_id, p_role_name);
        LOG_INFORMATION('INSERT_ROLE: Role ' || p_role_name || ' with ID ' || p_role_id || ' created.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_ROLE: Error creating role ' || p_role_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

-- Insert roles
BEGIN
    INSERT_ROLE(0, 'System Admin');
    INSERT_ROLE(1, 'Roles Admin');
    INSERT_ROLE(2, 'Purchases Rep');
    INSERT_ROLE(3, 'Sales Rep');
END;
/

-- Create procedure to insert a discount type
CREATE OR REPLACE PROCEDURE INSERT_DISCOUNT_TYPE (
    p_discount_type_id IN NUMBER,
    p_discount_type_name IN VARCHAR2
) IS
    discount_type_exists_by_id NUMBER := 0;
    discount_type_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the discount type ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_discount_type_id THEN 1 END), COUNT(CASE WHEN UPPER(name) = UPPER(p_discount_type_name) THEN 1 END) 
        INTO discount_type_exists_by_id, discount_type_exists_by_name
    FROM SLS_DISCOUNT_TYPES;

    -- Ensure both ID and name are unique
    IF discount_type_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_DISCOUNT_TYPE: Discount type ID ' || p_discount_type_id || ' already exists.');
    ELSIF discount_type_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_DISCOUNT_TYPE: Discount type name ' || p_discount_type_name || ' already exists.');
    ELSE
        -- Insert the discount type if both ID and name are unique
        INSERT INTO SLS_DISCOUNT_TYPES (id, name)
        VALUES (p_discount_type_id, p_discount_type_name);
        LOG_INFORMATION('INSERT_DISCOUNT_TYPE: Discount type ' || p_discount_type_name || ' with ID ' || p_discount_type_id || ' created.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_DISCOUNT_TYPE: Error creating discount type ' || p_discount_type_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

-- Insert discount types
BEGIN
    INSERT_DISCOUNT_TYPE(0, 'Absolute value');
    INSERT_DISCOUNT_TYPE(1, 'Percentage');
END;
/

-- Create procedure to insert a discount reason
CREATE OR REPLACE PROCEDURE INSERT_DISCOUNT_REASON (
    p_discount_reason_id IN NUMBER,
    p_discount_reason_name IN VARCHAR2
) IS
    discount_reason_exists_by_id NUMBER := 0;
    discount_reason_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the discount reason ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_discount_reason_id THEN 1 END), COUNT(CASE WHEN UPPER(name) = UPPER(p_discount_reason_name) THEN 1 END) 
        INTO discount_reason_exists_by_id, discount_reason_exists_by_name
    FROM SLS_DISCOUNT_REASONS;

    -- Ensure both ID and name are unique
    IF discount_reason_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_DISCOUNT_REASON: Discount reason ID ' || p_discount_reason_id || ' already exists.');
    ELSIF discount_reason_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_DISCOUNT_REASON: Discount reason name ' || p_discount_reason_name || ' already exists.');
    ELSE
        -- Insert the discount reason if both ID and name are unique
        INSERT INTO SLS_DISCOUNT_REASONS (id, name)
        VALUES (p_discount_reason_id, p_discount_reason_name);
        LOG_INFORMATION('INSERT_DISCOUNT_REASON: Discount reason ' || p_discount_reason_name || ' with ID ' || p_discount_reason_id || ' created.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_DISCOUNT_REASON: Error creating discount reason ' || p_discount_reason_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

-- Insert discount reasons
BEGIN
    INSERT_DISCOUNT_REASON(0, 'Fidelity');
    INSERT_DISCOUNT_REASON(1, 'Re-engagement');
    INSERT_DISCOUNT_REASON(2, 'Seasonal Sale');
    INSERT_DISCOUNT_REASON(3, 'Clearance');
    INSERT_DISCOUNT_REASON(4, 'Referral');
    INSERT_DISCOUNT_REASON(5, 'First Purchase');
    INSERT_DISCOUNT_REASON(6, 'Bulk Purchase');
    INSERT_DISCOUNT_REASON(7, 'Holiday Sale');
END;
/