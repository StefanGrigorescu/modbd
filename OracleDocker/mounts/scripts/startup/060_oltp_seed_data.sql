SET SERVEROUTPUT ON;

-- Switch to OLTP PDB
ALTER SESSION SET CONTAINER = eshop_oltp;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_OLTP_USER;


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

CREATE OR REPLACE PROCEDURE INSERT_REGION_AND_CITIES (
    p_region_id IN NUMBER,
    p_region_name IN NVARCHAR2,
    p_city_names_csv IN NVARCHAR2
) IS
    region_exists_by_id NUMBER := 0;
    region_exists_by_name NUMBER := 0;
BEGIN
    SELECT 
        COUNT(CASE WHEN id = p_region_id THEN 1 END) INTO region_exists_by_id,
        COUNT(CASE WHEN UPPER(name) = UPPER(p_region_name) THEN 1 END) INTO region_exists_by_name
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

BEGIN
    INSERT_REGION_AND_CITIES(0, 'Muntenia', 'Bucuresti,Ploiesti,Giurgiu,Targoviste,Alexandria,Slobozia');
    INSERT_REGION_AND_CITIES(1, 'Transilvania', 'Cluj-Napoca,Sibiu,Brasov,Alba Iulia,Targu Mures,Oradea');
    INSERT_REGION_AND_CITIES(2, 'Moldova', 'Iasi,Bacau,Suceava,Botosani,Piatra Neamt,Vaslui');
    INSERT_REGION_AND_CITIES(3, 'Dobrogea', 'Constanta,Tulcea,Mangalia,Medgidia,Navodari');
    INSERT_REGION_AND_CITIES(4, 'Banat', 'Timisoara,Resita,Lugoj,Caransebes');
    INSERT_REGION_AND_CITIES(5, 'Crisana', 'Oradea,Arad,Salonta,Beius,Marghita');
    INSERT_REGION_AND_CITIES(6, 'Maramures', 'Baia Mare,Sighetu Marmatiei,Borsa,Viseu de Sus');
    INSERT_REGION_AND_CITIES(7, 'Oltenia', 'Craiova,Targu Jiu,Slatina,Ramnicu Valcea,Drobeta-Turnu Severin');
    INSERT_REGION_AND_CITIES(8, 'Bucovina', 'Radauti,Campulung Moldovenesc,Vatra Dornei');
END;
/


CREATE OR REPLACE PROCEDURE INSERT_ROLE (
    p_role_id IN NUMBER,
    p_role_name IN VARCHAR2
) IS
    role_exists_by_id NUMBER := 0;
    role_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the role ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_role_id THEN 1 END) INTO role_exists_by_id,
        COUNT(CASE WHEN UPPER(name) = UPPER(p_role_name) THEN 1 END) INTO role_exists_by_name
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

BEGIN
    INSERT_ROLE(0, 'System Admin');
    INSERT_ROLE(1, 'Roles Admin');
    INSERT_ROLE(2, 'Purchases Rep');
    INSERT_ROLE(3, 'Sales Rep');
END;
/


-- Create procedure to insert a user role
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
    END;

    IF role_id IS NOT NULL THEN
        -- Insert the user role if the role exists
        INSERT INTO IDNT_USER_ROLES (user_id, role_id)
        VALUES (p_user_id, role_id);
        LOG_INFORMATION('INSERT_USER_ROLE: Role ' || p_role_name || ' assigned to user ID ' || p_user_id || '.');
    ELSE
        LOG_DEBUG('INSERT_USER_ROLE: Role ' || p_role_name || ' does not exist. No role assigned to user ID ' || p_user_id || '.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_USER_ROLE: Error assigning role ' || p_role_name || ' to user ID ' || p_user_id || ': ' || SQLERRM);
END;
/

-- Create procedure to insert a user (staff, not customer)
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
    p_user_created OUT NUMBER -- New OUT parameter
) IS
    user_exists NUMBER := 0;
    user_id NUMBER;
BEGIN
    -- Check if the user already exists by username or email
    SELECT COUNT(*) INTO user_exists
    FROM IDNT_USERS
    WHERE UPPER(username) = UPPER(p_username)
    OR UPPER(email) = UPPER(p_email);
 
    IF user_exists = 0 THEN
        -- Insert the user if they don't exist
        INSERT INTO IDNT_USERS (
            username, first_name, last_name, date_of_birth, email, phone_number, password, salt
        ) VALUES (
            p_username, p_first_name, p_last_name, p_date_of_birth, p_email, p_phone_number, p_password, p_salt
        ) RETURNING id INTO user_id;
 
        -- Log success
        LOG_INFORMATION('INSERT_USER: User ' || p_username || ' created.');
 
        -- Split the roles CSV and insert each role
        FOR role_name IN (SELECT REGEXP_SUBSTR(p_roles_csv, '[^,]+', 1, LEVEL) AS role_name
                          FROM DUAL
                          CONNECT BY REGEXP_SUBSTR(p_roles_csv, '[^,]+', 1, LEVEL) IS NOT NULL) LOOP
            INSERT_USER_ROLE(user_id, role_name.role_name);
        END LOOP;
 
        -- Set OUT parameter
        p_user_created := 1; -- User was successfully created
    ELSE
        -- Log user exists
        LOG_DEBUG('INSERT_USER: User with username ' || p_username || ' or email ' || p_email || ' already exists.');
        p_user_created := 0; -- User already exists
    END IF;
 
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_USER: Error creating user ' || p_username || ': ' || SQLERRM);
        p_user_created := -1; -- Error occurred
END;
/

DECLARE
    p_user_created NUMBER;
BEGIN
    -- System Admins
    INSERT_USER(
        p_username => 'IonPopescu',
        p_first_name => 'Ion',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1980-01-01', 'YYYY-MM-DD'),
        p_email => 'Ion.Popescu@email.com',
        p_phone_number => '0700000001',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'System Admin',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'MariaIonescu',
        p_first_name => 'Maria',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1985-02-02', 'YYYY-MM-DD'),
        p_email => 'Maria.Ionescu@email.com',
        p_phone_number => '0700000002',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'System Admin',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'VasilePopa',
        p_first_name => 'Vasile',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1990-03-03', 'YYYY-MM-DD'),
        p_email => 'Vasile.Popa@email.com',
        p_phone_number => '0700000003',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'System Admin',
        p_user_created => p_user_created
    );

    -- Roles Admins
    INSERT_USER(
        p_username => 'ElenaPopescu',
        p_first_name => 'Elena',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1982-04-04', 'YYYY-MM-DD'),
        p_email => 'Elena.Popescu@email.com',
        p_phone_number => '0700000004',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Roles Admin',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'GeorgeIonescu',
        p_first_name => 'George',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1987-05-05', 'YYYY-MM-DD'),
        p_email => 'George.Ionescu@email.com',
        p_phone_number => '0700000005',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Roles Admin',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'AnaPopa',
        p_first_name => 'Ana',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1992-06-06', 'YYYY-MM-DD'),
        p_email => 'Ana.Popa@email.com',
        p_phone_number => '0700000006',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Roles Admin',
        p_user_created => p_user_created
    );
    
    -- Purchases reps
    INSERT_USER(
        p_username => 'MihaiPopescu',
        p_first_name => 'Mihai',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1983-07-07', 'YYYY-MM-DD'),
        p_email => 'Mihai.Popescu@email.com',
        p_phone_number => '0700000007',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Purchases Rep',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'IoanaIonescu',
        p_first_name => 'Ioana',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1988-08-08', 'YYYY-MM-DD'),
        p_email => 'Ioana.Ionescu@email.com',
        p_phone_number => '0700000008',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Purchases Rep',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'FlorinPopa',
        p_first_name => 'Florin',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1993-09-09', 'YYYY-MM-DD'),
        p_email => 'Florin.Popa@email.com',
        p_phone_number => '0700000009',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Purchases Rep',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'AndreeaPopescu',
        p_first_name => 'Andreea',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1984-10-10', 'YYYY-MM-DD'),
        p_email => 'Andreea.Popescu@email.com',
        p_phone_number => '0700000010',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Purchases Rep',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'CristinaIonescu',
        p_first_name => 'Cristina',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1989-11-11', 'YYYY-MM-DD'),
        p_email => 'Cristina.Ionescu@email.com',
        p_phone_number => '0700000011',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Purchases Rep',
        p_user_created => p_user_created
    );
    
    -- Sales reps
    INSERT_USER(
        p_username => 'AlexandruPopescu',
        p_first_name => 'Alexandru',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1985-12-12', 'YYYY-MM-DD'),
        p_email => 'Alexandru.Popescu@email.com',
        p_phone_number => '0700000012',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Sales Rep',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'GabrielaIonescu',
        p_first_name => 'Gabriela',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1990-01-13', 'YYYY-MM-DD'),
        p_email => 'Gabriela.Ionescu@email.com',
        p_phone_number => '0700000013',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Sales Rep',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'StefanPopa',
        p_first_name => 'Stefan',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1995-02-14', 'YYYY-MM-DD'),
        p_email => 'Stefan.Popa@email.com',
        p_phone_number => '0700000014',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Sales Rep',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'DianaPopescu',
        p_first_name => 'Diana',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1986-03-15', 'YYYY-MM-DD'),
        p_email => 'Diana.Popescu@email.com',
        p_phone_number => '0700000015',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Sales Rep',
        p_user_created => p_user_created
    );

    INSERT_USER(
        p_username => 'RaduIonescu',
        p_first_name => 'Radu',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1991-04-16', 'YYYY-MM-DD'),
        p_email => 'Radu.Ionescu@email.com',
        p_phone_number => '0700000016',
        p_password => 'Password1!',
        p_salt => '123abc',
        p_roles_csv => 'Sales Rep',
        p_user_created => p_user_created
    );
END;
/


-- Create procedure to insert a user address
CREATE OR REPLACE FUNCTION INSERT_USER_ADDRESS (
    p_user_id IN NUMBER,
    p_region_name IN NVARCHAR2,
    p_city_name IN NVARCHAR2,
    p_street IN NVARCHAR2,
    p_str_number IN NUMBER,
    p_postal_code IN VARCHAR2,
    p_other_details IN NVARCHAR2 DEFAULT NULL,
    p_region_id OUT NUMBER
) RETURN NUMBER IS
    city_id NUMBER;
BEGIN
    -- Check if the city exists in the specified region
    BEGIN
        SELECT c.id, r.id 
        INTO city_id, p_region_id
        FROM IDNT_CITIES c
        JOIN IDNT_REGIONS r ON c.region_id = r.id
        WHERE UPPER(c.name) = UPPER(p_city_name)
        AND UPPER(r.name) = UPPER(p_region_name);
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            city_id := NULL;
    END;

    IF city_id IS NOT NULL THEN
        -- Insert the user address if the city exists
        INSERT INTO IDNT_USER_ADDRESSES (
            user_id, city_id, street, str_number, postal_code, other_details
        ) VALUES (
            p_user_id, city_id, p_street, p_str_number, p_postal_code, p_other_details
        );
        LOG_INFORMATION('INSERT_USER_ADDRESS: Address created for user ID ' || p_user_id || '.');
        RETURN 1;
    ELSE
        LOG_DEBUG('INSERT_USER_ADDRESS: City ' || p_city_name || ' in region ' || p_region_name || ' does not exist. No address created for user ID ' || p_user_id || '.');
        RETURN 0;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_USER_ADDRESS: Error creating address for user ID ' || p_user_id || ': ' || SQLERRM);
    RETURN 0;
END;
/

-- Create procedure to insert a customer
CREATE OR REPLACE PROCEDURE INSERT_CUSTOMER (
    p_username IN NVARCHAR2,
    p_first_name IN NVARCHAR2,
    p_last_name IN NVARCHAR2,
    p_date_of_birth IN DATE,
    p_email IN VARCHAR2,
    p_phone_number IN VARCHAR2,
    p_password IN VARCHAR2,
    p_salt IN VARCHAR2,
    p_region_name IN NVARCHAR2,
    p_city_name IN NVARCHAR2,
    p_street IN NVARCHAR2,
    p_str_number IN NUMBER,
    p_postal_code IN VARCHAR2,
    p_other_details IN NVARCHAR2 DEFAULT NULL
) IS
    user_exists NUMBER := 0;
    user_id NUMBER;
    address_created NUMBER;
    customer_region_id NUMBER;
BEGIN
    -- Check if the user already exists by username or email
    SELECT COUNT(*) INTO user_exists
    FROM IDNT_USERS
    WHERE UPPER(username) = UPPER(p_username)
    OR UPPER(email) = UPPER(p_email);

    IF user_exists = 0 THEN
        -- Insert the user if they don't exist
        INSERT INTO IDNT_USERS (
            username, first_name, last_name, date_of_birth, email, phone_number, password, salt
        ) VALUES (
            p_username, p_first_name, p_last_name, p_date_of_birth, p_email, p_phone_number, p_password, p_salt
        ) RETURNING id INTO user_id;
        LOG_INFORMATION('INSERT_CUSTOMER: User ' || p_username || ' created.');

        -- Insert the user address
        address_created := INSERT_USER_ADDRESS(
            p_user_id => user_id,
            p_region_name => p_region_name,
            p_city_name => p_city_name,
            p_street => p_street,
            p_str_number => p_str_number,
            p_postal_code => p_postal_code,
            p_other_details => p_other_details,
            p_region_id => customer_region_id
        );

        IF address_created = 1 THEN
            -- Update the user's region_id
            UPDATE IDNT_USERS
            SET region_id = customer_region_id
            WHERE id = user_id;

            LOG_INFORMATION('INSERT_CUSTOMER: Address created for user ID ' || user_id || '.');
            COMMIT;
        ELSE
            ROLLBACK;
            LOG_ERROR('INSERT_CUSTOMER: Address creation failed for user ID ' || user_id || '.');
        END IF;
    ELSE
        LOG_DEBUG('INSERT_CUSTOMER: User with username ' || p_username || ' or email ' || p_email || ' already exists.');
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        LOG_ERROR('INSERT_CUSTOMER: Error creating customer ' || p_username || ': ' || SQLERRM);
END;
/

-- Customers
BEGIN
    INSERT_CUSTOMER(
        p_username => 'AdrianPopescu',
        p_first_name => 'Adrian',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1981-01-01', 'YYYY-MM-DD'),
        p_email => 'Adrian.Popescu@email.com',
        p_phone_number => '0700000017',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Muntenia',
        p_city_name => 'Bucuresti',
        p_street => 'Strada Principala',
        p_str_number => 1,
        p_postal_code => '010101',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'DanielaIonescu',
        p_first_name => 'Daniela',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1982-02-02', 'YYYY-MM-DD'),
        p_email => 'Daniela.Ionescu@email.com',
        p_phone_number => '0700000018',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Transilvania',
        p_city_name => 'Cluj-Napoca',
        p_street => 'Strada Secundara',
        p_str_number => 2,
        p_postal_code => '020202',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'BogdanPopa',
        p_first_name => 'Bogdan',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1983-03-03', 'YYYY-MM-DD'),
        p_email => 'Bogdan.Popa@email.com',
        p_phone_number => '0700000019',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Moldova',
        p_city_name => 'Iasi',
        p_street => 'Strada Tertiar',
        p_str_number => 3,
        p_postal_code => '030303',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'CarmenPopescu',
        p_first_name => 'Carmen',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1984-04-04', 'YYYY-MM-DD'),
        p_email => 'Carmen.Popescu@email.com',
        p_phone_number => '0700000020',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Dobrogea',
        p_city_name => 'Constanta',
        p_street => 'Strada Quaternar',
        p_str_number => 4,
        p_postal_code => '040404',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'DoruIonescu',
        p_first_name => 'Doru',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1985-05-05', 'YYYY-MM-DD'),
        p_email => 'Doru.Ionescu@email.com',
        p_phone_number => '0700000021',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Banat',
        p_city_name => 'Timisoara',
        p_street => 'Strada Quintenar',
        p_str_number => 5,
        p_postal_code => '050505',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'EmiliaPopa',
        p_first_name => 'Emilia',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1986-06-06', 'YYYY-MM-DD'),
        p_email => 'Emilia.Popa@email.com',
        p_phone_number => '0700000022',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Crisana',
        p_city_name => 'Oradea',
        p_street => 'Strada Sextenar',
        p_str_number => 6,
        p_postal_code => '060606',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'FlorentinaPopescu',
        p_first_name => 'Florentina',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1987-07-07', 'YYYY-MM-DD'),
        p_email => 'Florentina.Popescu@email.com',
        p_phone_number => '0700000023',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Maramures',
        p_city_name => 'Baia Mare',
        p_street => 'Strada Septenar',
        p_str_number => 7,
        p_postal_code => '070707',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'GheorgheIonescu',
        p_first_name => 'Gheorghe',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1988-08-08', 'YYYY-MM-DD'),
        p_email => 'Gheorghe.Ionescu@email.com',
        p_phone_number => '0700000024',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Oltenia',
        p_city_name => 'Craiova',
        p_street => 'Strada Octenar',
        p_str_number => 8,
        p_postal_code => '080808',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'HoriaPopa',
        p_first_name => 'Horia',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1989-09-09', 'YYYY-MM-DD'),
        p_email => 'Horia.Popa@email.com',
        p_phone_number => '0700000025',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Bucovina',
        p_city_name => 'Radauti',
        p_street => 'Strada Nona',
        p_str_number => 9,
        p_postal_code => '090909',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'IrinaPopescu',
        p_first_name => 'Irina',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1990-10-10', 'YYYY-MM-DD'),
        p_email => 'Irina.Popescu@email.com',
        p_phone_number => '0700000026',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Muntenia',
        p_city_name => 'Ploiesti',
        p_street => 'Strada Decenar',
        p_str_number => 10,
        p_postal_code => '101010',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'IulianIonescu',
        p_first_name => 'Iulian',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1991-11-11', 'YYYY-MM-DD'),
        p_email => 'Iulian.Ionescu@email.com',
        p_phone_number => '0700000027',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Transilvania',
        p_city_name => 'Sibiu',
        p_street => 'Strada Undecenar',
        p_str_number => 11,
        p_postal_code => '111111',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'JanaPopa',
        p_first_name => 'Jana',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1992-12-12', 'YYYY-MM-DD'),
        p_email => 'Jana.Popa@email.com',
        p_phone_number => '0700000028',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Moldova',
        p_city_name => 'Bacau',
        p_street => 'Strada Dodecenar',
        p_str_number => 12,
        p_postal_code => '121212',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'KlausPopescu',
        p_first_name => 'Klaus',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1993-01-13', 'YYYY-MM-DD'),
        p_email => 'Klaus.Popescu@email.com',
        p_phone_number => '0700000029',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Dobrogea',
        p_city_name => 'Tulcea',
        p_street => 'Strada Tredecenar',
        p_str_number => 13,
        p_postal_code => '131313',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'LauraIonescu',
        p_first_name => 'Laura',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1994-02-14', 'YYYY-MM-DD'),
        p_email => 'Laura.Ionescu@email.com',
        p_phone_number => '0700000030',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Banat',
        p_city_name => 'Resita',
        p_street => 'Strada Quattuordecenar',
        p_str_number => 14,
        p_postal_code => '141414',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'MariusPopa',
        p_first_name => 'Marius',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1995-03-15', 'YYYY-MM-DD'),
        p_email => 'Marius.Popa@email.com',
        p_phone_number => '0700000031',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Crisana',
        p_city_name => 'Arad',
        p_street => 'Strada Quindecenar',
        p_str_number => 15,
        p_postal_code => '151515',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'NicoletaPopescu',
        p_first_name => 'Nicoleta',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1996-04-16', 'YYYY-MM-DD'),
        p_email => 'Nicoleta.Popescu@email.com',
        p_phone_number => '0700000032',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Maramures',
        p_city_name => 'Sighetu Marmatiei',
        p_street => 'Strada Sedecenar',
        p_str_number => 16,
        p_postal_code => '161616',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'OvidiuIonescu',
        p_first_name => 'Ovidiu',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('1997-05-17', 'YYYY-MM-DD'),
        p_email => 'Ovidiu.Ionescu@email.com',
        p_phone_number => '0700000033',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Oltenia',
        p_city_name => 'Targu Jiu',
        p_street => 'Strada Septendecenar',
        p_str_number => 17,
        p_postal_code => '171717',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'PaulaPopa',
        p_first_name => 'Paula',
        p_last_name => 'Popa',
        p_date_of_birth => TO_DATE('1998-06-18', 'YYYY-MM-DD'),
        p_email => 'Paula.Popa@email.com',
        p_phone_number => '0700000034',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Bucovina',
        p_city_name => 'Campulung Moldovenesc',
        p_street => 'Strada Octodecenar',
        p_str_number => 18,
        p_postal_code => '181818',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'RoxanaPopescu',
        p_first_name => 'Roxana',
        p_last_name => 'Popescu',
        p_date_of_birth => TO_DATE('1999-07-19', 'YYYY-MM-DD'),
        p_email => 'Roxana.Popescu@email.com',
        p_phone_number => '0700000035',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Muntenia',
        p_city_name => 'Giurgiu',
        p_street => 'Strada Novendecenar',
        p_str_number => 19,
        p_postal_code => '191919',
        p_other_details => NULL
    );

    INSERT_CUSTOMER(
        p_username => 'SorinIonescu',
        p_first_name => 'Sorin',
        p_last_name => 'Ionescu',
        p_date_of_birth => TO_DATE('2000-08-20', 'YYYY-MM-DD'),
        p_email => 'Sorin.Ionescu@email.com',
        p_phone_number => '0700000036',
        p_password => 'password1!',
        p_salt => '123abc',
        p_region_name => 'Transilvania',
        p_city_name => 'Brasov',
        p_street => 'Strada Vicesimus',
        p_str_number => 20,
        p_postal_code => '202020',
        p_other_details => NULL
    );
END;
/


-- Create procedure to insert a subcategory
CREATE OR REPLACE PROCEDURE INSERT_SUBCATEGORY (
    p_category_id IN NUMBER,
    p_subcategory_name IN NVARCHAR2
) IS
    subcategory_exists NUMBER := 0;
BEGIN
    -- Check if the subcategory already exists
    SELECT COUNT(*) INTO subcategory_exists
    FROM SLS_PRODUCT_SUBCATEGORIES
    WHERE UPPER(name) = UPPER(p_subcategory_name)
    AND category_id = p_category_id;

    IF subcategory_exists = 0 THEN
        -- Insert the subcategory if it doesn't exist
        INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name)
        VALUES (p_category_id, p_subcategory_name);
        LOG_INFORMATION('INSERT_SUBCATEGORY: Subcategory ' || p_subcategory_name || ' created.');
    ELSE
        LOG_DEBUG('INSERT_SUBCATEGORY: Subcategory ' || p_subcategory_name || ' already exists.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_SUBCATEGORY: Error creating subcategory ' || p_subcategory_name || ': ' || SQLERRM);
END;
/

CREATE OR REPLACE PROCEDURE INSERT_CATEGORY_AND_SUBCATEGORIES (
    p_category_id IN NUMBER,
    p_category_name IN NVARCHAR2,
    p_subcategory_names_csv IN NVARCHAR2
) IS
    category_exists_by_id NUMBER := 0;
    category_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the category ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_category_id THEN 1 END) INTO category_exists_by_id,
        COUNT(CASE WHEN UPPER(name) = UPPER(p_category_name) THEN 1 END) INTO category_exists_by_name
    FROM SLS_PRODUCT_CATEGORIES;

    -- Ensure both ID and name are unique
    IF category_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_CATEGORY_AND_SUBCATEGORIES: Category ID ' || p_category_id || ' already exists.');
    ELSIF category_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_CATEGORY_AND_SUBCATEGORIES: Category name ' || p_category_name || ' already exists.');
    ELSE
        -- Insert the category if both ID and name are unique
        INSERT INTO SLS_PRODUCT_CATEGORIES (id, name)
        VALUES (p_category_id, p_category_name);
        LOG_INFORMATION('INSERT_CATEGORY_AND_SUBCATEGORIES: Category ' || p_category_name || ' with ID ' || p_category_id || ' created.');
    END IF;

    -- Split the subcategory names CSV and insert each subcategory
    FOR subcategory_name IN (SELECT REGEXP_SUBSTR(p_subcategory_names_csv, '[^,]+', 1, LEVEL) AS subcategory_name
                             FROM DUAL
                             CONNECT BY REGEXP_SUBSTR(p_subcategory_names_csv, '[^,]+', 1, LEVEL) IS NOT NULL) LOOP
        INSERT_SUBCATEGORY(p_category_id, subcategory_name.subcategory_name);
    END LOOP;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_CATEGORY_AND_SUBCATEGORIES: Error creating category ' || p_category_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

BEGIN
    INSERT_CATEGORY_AND_SUBCATEGORIES(0, 'Sport', 'Football,Basketball,Tennis,Running,Swimming,Fitness,Yoga,Hiking');
    INSERT_CATEGORY_AND_SUBCATEGORIES(1, 'Music', 'Instruments,Sheet Music,Accessories,Recording Equipment,Amps,Speakers,Headphones,Turntables');
    INSERT_CATEGORY_AND_SUBCATEGORIES(2, 'Art', 'Painting,Sculpture,Drawing,Photography,Printmaking,Pencil Sketching,Watercolor,Acrylic,Ceramics');
    INSERT_CATEGORY_AND_SUBCATEGORIES(3, 'Electronics', 'Mobile Phones,Laptops,Tablets,Cameras,Accessories');
    INSERT_CATEGORY_AND_SUBCATEGORIES(4, 'Home Appliances', 'Refrigerators,Washing Machines,Microwaves,Vacuum Cleaners,Air Conditioners');
    INSERT_CATEGORY_AND_SUBCATEGORIES(5, 'Furniture', 'Living Room,Bedroom,Office,Outdoor,Storage');
    INSERT_CATEGORY_AND_SUBCATEGORIES(6, 'Clothing', 'Men,Women,Kids,Accessories,Shoes');
    INSERT_CATEGORY_AND_SUBCATEGORIES(7, 'Books', 'Fiction,Non-Fiction,Children,Educational,Comics,Hystorical,Science Fiction');
    INSERT_CATEGORY_AND_SUBCATEGORIES(8, 'Toys', 'Action Figures,Dolls,Puzzles,Educational Toys,Outdoor Toys');
    INSERT_CATEGORY_AND_SUBCATEGORIES(9, 'Beauty', 'Skincare,Makeup,Haircare,Fragrances,Tools');
    INSERT_CATEGORY_AND_SUBCATEGORIES(10, 'Automotive', 'Car Accessories,Motorcycle Accessories,Tools,Parts,Electronics');
    INSERT_CATEGORY_AND_SUBCATEGORIES(11, 'Garden', 'Plants,Tools,Outdoor Furniture,Decorations,Watering Equipment');
    INSERT_CATEGORY_AND_SUBCATEGORIES(12, 'Health', 'Supplements,Medical Equipment,Personal Care,Fitness Equipment');
    INSERT_CATEGORY_AND_SUBCATEGORIES(13, 'Office Supplies', 'Stationery,Printers,Office Furniture,Storage,Electronics');
    INSERT_CATEGORY_AND_SUBCATEGORIES(14, 'Pet Supplies', 'Food,Toys,Accessories,Grooming,Health');
END;
/


-- Create function to attach product tags
CREATE OR REPLACE PROCEDURE ATTACH_PRODUCT_TAGS (
    p_product_id IN NUMBER,
    p_tags_csv IN NVARCHAR2
) IS
    tag_id NUMBER;
    tag_exists NUMBER;
BEGIN
    -- Split the tags CSV and process each tag
    FOR tag_name IN (SELECT REGEXP_SUBSTR(p_tags_csv, '[^,]+', 1, LEVEL) AS tag_name
                     FROM DUAL
                     CONNECT BY REGEXP_SUBSTR(p_tags_csv, '[^,]+', 1, LEVEL) IS NOT NULL) LOOP
        -- Check if the tag exists
        BEGIN
            SELECT id INTO tag_id
            FROM SLS_PRODUCT_TAGS
            WHERE UPPER(name) = UPPER(tag_name.tag_name);
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                -- Insert the tag if it doesn't exist
                INSERT INTO SLS_PRODUCT_TAGS (name)
                VALUES (tag_name.tag_name)
                RETURNING id INTO tag_id;
                LOG_INFORMATION('ATTACH_PRODUCT_TAGS: Tag ' || tag_name.tag_name || ' created.');
        END;

        -- Check if the product already has the tag
        SELECT COUNT(*) INTO tag_exists
        FROM SLS_PRODUCT_PRODUCT_TAGS
        WHERE product_id = p_product_id AND tag_id = tag_id;

        IF tag_exists = 0 THEN
            -- Attach the tag to the product
            INSERT INTO SLS_PRODUCT_PRODUCT_TAGS (product_id, tag_id)
            VALUES (p_product_id, tag_id);
            LOG_INFORMATION('ATTACH_PRODUCT_TAGS: Tag ' || tag_name.tag_name || ' attached to product ID ' || p_product_id || '.');
        ELSE
            LOG_DEBUG('ATTACH_PRODUCT_TAGS: Product ID ' || p_product_id || ' already has tag ' || tag_name.tag_name || '.');
        END IF;
    END LOOP;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('ATTACH_PRODUCT_TAGS: Error attaching tags to product ID ' || p_product_id || ': ' || SQLERRM);
END;
/

-- Create procedure to create a product within a subcategory
CREATE OR REPLACE PROCEDURE SLS_CREATE_PRODUCT (
    p_product_name IN NVARCHAR2,
    p_description IN NVARCHAR2,
    p_price_in_eur IN NUMBER,
    p_category_name IN NVARCHAR2,
    p_subcategory_name IN NVARCHAR2,
    p_tags_csv IN NVARCHAR2
) IS
    subcategory_id NUMBER;
    product_id NUMBER;
    product_exists NUMBER;
BEGIN
    -- Check if the subcategory exists within the specified category
    BEGIN
        SELECT s.id INTO subcategory_id
        FROM SLS_PRODUCT_SUBCATEGORIES s
        INNER JOIN SLS_PRODUCT_CATEGORIES c ON s.category_id = c.id
        WHERE UPPER(s.name) = UPPER(p_subcategory_name)
        AND UPPER(c.name) = UPPER(p_category_name);
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            subcategory_id := NULL;
    END;

    IF subcategory_id IS NOT NULL THEN
        -- Check if a product with the same name already exists in the subcategory
        SELECT COUNT(*) INTO product_exists
        FROM SLS_PRODUCTS p
        INNER JOIN SLS_PRODUCT_PRODUCT_SUBCATEGORIES ps ON p.id = ps.product_id
        WHERE UPPER(p.name) = UPPER(p_product_name)
        AND ps.subcategory_id = subcategory_id;

        IF product_exists = 0 THEN
            -- Insert the product
            INSERT INTO SLS_PRODUCTS (name, description, price_in_eur)
            VALUES (p_product_name, p_description, p_price_in_eur)
            RETURNING id INTO product_id;

            -- Link the product to the subcategory
            INSERT INTO SLS_PRODUCT_PRODUCT_SUBCATEGORIES (product_id, subcategory_id)
            VALUES (product_id, subcategory_id);

            -- Attach tags to the product
            ATTACH_PRODUCT_TAGS(product_id, p_tags_csv);

            LOG_INFORMATION('SLS_CREATE_PRODUCT: Product ' || p_product_name || ' created ' || 'in ' || p_category_name || ' > ' || p_subcategory_name || '.');

            COMMIT;
        ELSE
            LOG_DEBUG('SLS_CREATE_PRODUCT: Product ' || p_product_name || ' already exists in subcategory ' || p_subcategory_name || '.');
        END IF;
    ELSE
        LOG_DEBUG('SLS_CREATE_PRODUCT: Subcategory ' || p_subcategory_name || ' in category ' || p_category_name || ' does not exist.');
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('SLS_CREATE_PRODUCT: Error creating product ' || p_product_name || ': ' || SQLERRM);
END;
/

-- Create products
BEGIN
    -- Sport
    -- Football
    SLS_CREATE_PRODUCT('Football Ball', 'Standard size football ball', 25, 'Sport', 'Football', 'ball,sport');
    SLS_CREATE_PRODUCT('Football Shoes', 'High-quality football shoes', 50, 'Sport', 'Football', 'shoes,sport');
    -- Basketball
    SLS_CREATE_PRODUCT('Basketball Ball', 'Standard size basketball ball', 30, 'Sport', 'Basketball', 'ball,sport');
    SLS_CREATE_PRODUCT('Basketball Hoop', 'Durable basketball hoop', 100, 'Sport', 'Basketball', 'hoop,sport');
    -- Tennis
    SLS_CREATE_PRODUCT('Tennis Racket', 'Professional tennis racket', 75, 'Sport', 'Tennis', 'racket,sport');
    SLS_CREATE_PRODUCT('Tennis Balls', 'Pack of 3 tennis balls', 10, 'Sport', 'Tennis', 'balls,sport');
    -- Running
    SLS_CREATE_PRODUCT('Running Shoes', 'Comfortable running shoes', 60, 'Sport', 'Running', 'shoes,sport');
    SLS_CREATE_PRODUCT('Running Shorts', 'Lightweight running shorts', 20, 'Sport', 'Running', 'shorts,sport');
    -- Swimming
    SLS_CREATE_PRODUCT('Swimming Goggles', 'Anti-fog swimming goggles', 15, 'Sport', 'Swimming', 'goggles,sport');
    SLS_CREATE_PRODUCT('Swimming Cap', 'Silicone swimming cap', 10, 'Sport', 'Swimming', 'cap,sport');
    -- Fitness
    SLS_CREATE_PRODUCT('Dumbbells', 'Set of 2 dumbbells', 40, 'Sport', 'Fitness', 'dumbbells,fitness');
    SLS_CREATE_PRODUCT('Yoga Mat', 'Non-slip yoga mat', 25, 'Sport', 'Fitness', 'mat,fitness');
    -- Yoga
    SLS_CREATE_PRODUCT('Yoga Blocks', 'Set of 2 yoga blocks', 20, 'Sport', 'Yoga', 'blocks,yoga');
    SLS_CREATE_PRODUCT('Yoga Strap', 'Durable yoga strap', 15, 'Sport', 'Yoga', 'strap,yoga');
    -- Hiking
    SLS_CREATE_PRODUCT('Hiking Boots', 'Waterproof hiking boots', 80, 'Sport', 'Hiking', 'boots,hiking');
    SLS_CREATE_PRODUCT('Hiking Backpack', 'Spacious hiking backpack', 60, 'Sport', 'Hiking', 'backpack,hiking');

    -- Music
    -- Instruments
    SLS_CREATE_PRODUCT('Guitar', 'Acoustic guitar', 150, 'Music', 'Instruments', 'guitar,music');
    SLS_CREATE_PRODUCT('Piano', 'Digital piano', 500, 'Music', 'Instruments', 'piano,music');
    -- Sheet Music
    SLS_CREATE_PRODUCT('Piano Sheet Music', 'Classical piano sheet music', 20, 'Music', 'Sheet Music', 'sheet music,music');
    SLS_CREATE_PRODUCT('Guitar Sheet Music', 'Rock guitar sheet music', 15, 'Music', 'Sheet Music', 'sheet music,music');
    -- Accessories
    SLS_CREATE_PRODUCT('Guitar Picks', 'Pack of 10 guitar picks', 5, 'Music', 'Accessories', 'picks,music');
    SLS_CREATE_PRODUCT('Piano Bench', 'Adjustable piano bench', 50, 'Music', 'Accessories', 'bench,music');
    -- Recording Equipment
    SLS_CREATE_PRODUCT('Microphone', 'Studio microphone', 100, 'Music', 'Recording Equipment', 'microphone,music');
    SLS_CREATE_PRODUCT('Audio Interface', 'USB audio interface', 150, 'Music', 'Recording Equipment', 'interface,music');
    -- Amps
    SLS_CREATE_PRODUCT('Guitar Amp', 'Electric guitar amplifier', 200, 'Music', 'Amps', 'amp,music');
    SLS_CREATE_PRODUCT('Bass Amp', 'Electric bass amplifier', 250, 'Music', 'Amps', 'amp,music');
    -- Speakers
    SLS_CREATE_PRODUCT('Studio Monitors', 'Pair of studio monitors', 300, 'Music', 'Speakers', 'monitors,music');
    SLS_CREATE_PRODUCT('PA System', 'Portable PA system', 400, 'Music', 'Speakers', 'pa system,music');
    -- Headphones
    SLS_CREATE_PRODUCT('Studio Headphones', 'Over-ear studio headphones', 100, 'Music', 'Headphones', 'headphones,music');
    SLS_CREATE_PRODUCT('In-Ear Monitors', 'Professional in-ear monitors', 150, 'Music', 'Headphones', 'monitors,music');
    -- Turntables
    SLS_CREATE_PRODUCT('DJ Turntable', 'Professional DJ turntable', 350, 'Music', 'Turntables', 'turntable,music');
    SLS_CREATE_PRODUCT('Vinyl Record Player', 'Vintage vinyl record player', 200, 'Music', 'Turntables', 'record player,music');

    -- Art
    -- Painting
    SLS_CREATE_PRODUCT('Acrylic Paint Set', 'Set of 24 acrylic paints', 30, 'Art', 'Painting', 'paint,art');
    SLS_CREATE_PRODUCT('Canvas', 'Stretched canvas', 20, 'Art', 'Painting', 'canvas,art');
    -- Sculpture
    SLS_CREATE_PRODUCT('Clay', 'Modeling clay', 15, 'Art', 'Sculpture', 'clay,art');
    SLS_CREATE_PRODUCT('Sculpting Tools', 'Set of sculpting tools', 25, 'Art', 'Sculpture', 'tools,art');
    -- Drawing
    SLS_CREATE_PRODUCT('Sketchbook', 'Hardcover sketchbook', 10, 'Art', 'Drawing', 'sketchbook,art');
    SLS_CREATE_PRODUCT('Pencils', 'Set of 12 drawing pencils', 15, 'Art', 'Drawing', 'pencils,art');
    -- Photography
    SLS_CREATE_PRODUCT('Camera', 'Digital SLR camera', 500, 'Art', 'Photography', 'camera,art');
    SLS_CREATE_PRODUCT('Tripod', 'Adjustable tripod', 50, 'Art', 'Photography', 'tripod,art');
    -- Printmaking
    SLS_CREATE_PRODUCT('Printmaking Ink', 'Set of printmaking inks', 40, 'Art', 'Printmaking', 'ink,art');
    SLS_CREATE_PRODUCT('Brayer', 'Rubber brayer', 20, 'Art', 'Printmaking', 'brayer,art');
    -- Pencil Sketching
    SLS_CREATE_PRODUCT('Graphite Pencils', 'Set of 6 graphite pencils', 10, 'Art', 'Pencil Sketching', 'pencils,art');
    SLS_CREATE_PRODUCT('Eraser', 'Kneaded eraser', 5, 'Art', 'Pencil Sketching', 'eraser,art');
    -- Watercolor
    SLS_CREATE_PRODUCT('Watercolor Paint Set', 'Set of 12 watercolor paints', 25, 'Art', 'Watercolor', 'paint,art');
    SLS_CREATE_PRODUCT('Watercolor Paper', 'Pad of watercolor paper', 15, 'Art', 'Watercolor', 'paper,art');
    -- Acrylic
    SLS_CREATE_PRODUCT('Acrylic Paint Tubes', 'Set of 12 acrylic paint tubes', 30, 'Art', 'Acrylic', 'paint,art');
    SLS_CREATE_PRODUCT('Palette', 'Plastic palette', 10, 'Art', 'Acrylic', 'palette,art');
    -- Ceramics
    SLS_CREATE_PRODUCT('Pottery Wheel', 'Electric pottery wheel', 300, 'Art', 'Ceramics', 'wheel,art');
    SLS_CREATE_PRODUCT('Ceramic Glaze', 'Set of ceramic glazes', 40, 'Art', 'Ceramics', 'glaze,art');

    -- Electronics
    -- Mobile Phones
    SLS_CREATE_PRODUCT('Smartphone', 'Latest model smartphone', 700, 'Electronics', 'Mobile Phones', 'phone,electronics');
    SLS_CREATE_PRODUCT('Phone Case', 'Protective phone case', 20, 'Electronics', 'Mobile Phones', 'case,electronics');
    -- Laptops
    SLS_CREATE_PRODUCT('Laptop', 'High-performance laptop', 1000, 'Electronics', 'Laptops', 'laptop,electronics');
    SLS_CREATE_PRODUCT('Laptop Bag', 'Durable laptop bag', 50, 'Electronics', 'Laptops', 'bag,electronics');
    -- Tablets
    SLS_CREATE_PRODUCT('Tablet', 'Latest model tablet', 500, 'Electronics', 'Tablets', 'tablet,electronics');
    SLS_CREATE_PRODUCT('Tablet Stand', 'Adjustable tablet stand', 30, 'Electronics', 'Tablets', 'stand,electronics');
    -- Cameras
    SLS_CREATE_PRODUCT('Digital Camera', 'High-resolution digital camera', 600, 'Electronics', 'Cameras', 'camera,electronics');
    SLS_CREATE_PRODUCT('Camera Lens', 'Zoom camera lens', 300, 'Electronics', 'Cameras', 'lens,electronics');
    -- Accessories
    SLS_CREATE_PRODUCT('USB Cable', 'High-speed USB cable', 10, 'Electronics', 'Accessories', 'cable,electronics');
    SLS_CREATE_PRODUCT('Power Bank', 'Portable power bank', 25, 'Electronics', 'Accessories', 'power bank,electronics');

    -- Home Appliances
    -- Refrigerators
    SLS_CREATE_PRODUCT('Refrigerator', 'Energy-efficient refrigerator', 800, 'Home Appliances', 'Refrigerators', 'refrigerator,appliances');
    SLS_CREATE_PRODUCT('Mini Fridge', 'Compact mini fridge', 200, 'Home Appliances', 'Refrigerators', 'fridge,appliances');
    -- Washing Machines
    SLS_CREATE_PRODUCT('Washing Machine', 'Front-load washing machine', 600, 'Home Appliances', 'Washing Machines', 'washing machine,appliances');
    SLS_CREATE_PRODUCT('Dryer', 'Electric dryer', 500, 'Home Appliances', 'Washing Machines', 'dryer,appliances');
    -- Microwaves
    SLS_CREATE_PRODUCT('Microwave Oven', 'Countertop microwave oven', 150, 'Home Appliances', 'Microwaves', 'microwave,appliances');
    SLS_CREATE_PRODUCT('Convection Oven', 'Convection microwave oven', 250, 'Home Appliances', 'Microwaves', 'oven,appliances');
    -- Vacuum Cleaners
    SLS_CREATE_PRODUCT('Vacuum Cleaner', 'Bagless vacuum cleaner', 200, 'Home Appliances', 'Vacuum Cleaners', 'vacuum,appliances');
    SLS_CREATE_PRODUCT('Robot Vacuum', 'Smart robot vacuum', 300, 'Home Appliances', 'Vacuum Cleaners', 'robot vacuum,appliances');
    -- Air Conditioners
    SLS_CREATE_PRODUCT('Window AC', 'Window air conditioner', 400, 'Home Appliances', 'Air Conditioners', 'ac,appliances');
    SLS_CREATE_PRODUCT('Portable AC', 'Portable air conditioner', 350, 'Home Appliances', 'Air Conditioners', 'ac,appliances');

    -- Furniture
    -- Living Room
    SLS_CREATE_PRODUCT('Sofa', 'Comfortable living room sofa', 700, 'Furniture', 'Living Room', 'sofa,furniture');
    SLS_CREATE_PRODUCT('Coffee Table', 'Wooden coffee table', 150, 'Furniture', 'Living Room', 'table,furniture');
    -- Bedroom
    SLS_CREATE_PRODUCT('Bed Frame', 'Queen size bed frame', 400, 'Furniture', 'Bedroom', 'bed,furniture');
    SLS_CREATE_PRODUCT('Nightstand', 'Wooden nightstand', 100, 'Furniture', 'Bedroom', 'nightstand,furniture');
    -- Office
    SLS_CREATE_PRODUCT('Office Chair', 'Ergonomic office chair', 200, 'Furniture', 'Office', 'chair,furniture');
    SLS_CREATE_PRODUCT('Desk', 'Adjustable office desk', 300, 'Furniture', 'Office', 'desk,furniture');
    -- Outdoor
    SLS_CREATE_PRODUCT('Patio Set', 'Outdoor patio set', 500, 'Furniture', 'Outdoor', 'patio,furniture');
    SLS_CREATE_PRODUCT('Hammock', 'Comfortable outdoor hammock', 100, 'Furniture', 'Outdoor', 'hammock,furniture');
    -- Storage
    SLS_CREATE_PRODUCT('Bookshelf', 'Wooden bookshelf', 150, 'Furniture', 'Storage', 'bookshelf,furniture');
    SLS_CREATE_PRODUCT('Storage Box', 'Plastic storage box', 20, 'Furniture', 'Storage', 'box,furniture');

    -- Clothing
    -- Men
    SLS_CREATE_PRODUCT('Men T-Shirt', 'Cotton men t-shirt', 20, 'Clothing', 'Men', 't-shirt,clothing');
    SLS_CREATE_PRODUCT('Men Jeans', 'Denim men jeans', 40, 'Clothing', 'Men', 'jeans,clothing');
    -- Women
    SLS_CREATE_PRODUCT('Women Dress', 'Summer women dress', 30, 'Clothing', 'Women', 'dress,clothing');
    SLS_CREATE_PRODUCT('Women Blouse', 'Silk women blouse', 25, 'Clothing', 'Women', 'blouse,clothing');
    -- Kids
    SLS_CREATE_PRODUCT('Kids T-Shirt', 'Cotton kids t-shirt', 15, 'Clothing', 'Kids', 't-shirt,clothing');
    SLS_CREATE_PRODUCT('Kids Shorts', 'Denim kids shorts', 20, 'Clothing', 'Kids', 'shorts,clothing');
    -- Accessories
    SLS_CREATE_PRODUCT('Sunglasses', 'Polarized sunglasses', 50, 'Clothing', 'Accessories', 'sunglasses,clothing');
    SLS_CREATE_PRODUCT('Belt', 'Leather belt', 30, 'Clothing', 'Accessories', 'belt,clothing');
    -- Shoes
    SLS_CREATE_PRODUCT('Running Shoes', 'Comfortable running shoes', 60, 'Clothing', 'Shoes', 'shoes,clothing');
    SLS_CREATE_PRODUCT('Sandals', 'Leather sandals', 40, 'Clothing', 'Shoes', 'sandals,clothing');

    -- Books
    -- Fiction
    SLS_CREATE_PRODUCT('Mystery Novel', 'Bestselling mystery novel', 15, 'Books', 'Fiction', 'novel,books');
    SLS_CREATE_PRODUCT('Fantasy Novel', 'Epic fantasy novel', 20, 'Books', 'Fiction', 'novel,books');
    -- Non-Fiction
    SLS_CREATE_PRODUCT('Biography', 'Inspirational biography', 25, 'Books', 'Non-Fiction', 'biography,books');
    SLS_CREATE_PRODUCT('Self-Help Book', 'Popular self-help book', 20, 'Books', 'Non-Fiction', 'self-help,books');
    -- Children
    SLS_CREATE_PRODUCT('Picture Book', 'Colorful picture book', 10, 'Books', 'Children', 'book,books');
    SLS_CREATE_PRODUCT('Storybook', 'Classic children storybook', 15, 'Books', 'Children', 'book,books');
    -- Educational
    SLS_CREATE_PRODUCT('Math Textbook', 'Comprehensive math textbook', 30, 'Books', 'Educational', 'textbook,books');
    SLS_CREATE_PRODUCT('Science Textbook', 'Detailed science textbook', 35, 'Books', 'Educational', 'textbook,books');
    -- Comics
    SLS_CREATE_PRODUCT('Superhero Comic', 'Action-packed superhero comic', 5, 'Books', 'Comics', 'comic,books');
    SLS_CREATE_PRODUCT('Graphic Novel', 'Critically acclaimed graphic novel', 20, 'Books', 'Comics', 'novel,books');
    -- Historical
    SLS_CREATE_PRODUCT('History Book', 'In-depth history book', 25, 'Books', 'Historical', 'book,books');
    SLS_CREATE_PRODUCT('Historical Fiction', 'Engaging historical fiction', 20, 'Books', 'Historical', 'fiction,books');
    -- Science Fiction
    SLS_CREATE_PRODUCT('Sci-Fi Novel', 'Exciting sci-fi novel', 15, 'Books', 'Science Fiction', 'novel,books');
    SLS_CREATE_PRODUCT('Dystopian Novel', 'Gripping dystopian novel', 20, 'Books', 'Science Fiction', 'novel,books');

    -- Toys
    -- Action Figures
    SLS_CREATE_PRODUCT('Superhero Action Figure', 'Detailed superhero action figure', 20, 'Toys', 'Action Figures', 'figure,toys');
    SLS_CREATE_PRODUCT('Robot Action Figure', 'Interactive robot action figure', 30, 'Toys', 'Action Figures', 'figure,toys');
    -- Dolls
    SLS_CREATE_PRODUCT('Fashion Doll', 'Stylish fashion doll', 25, 'Toys', 'Dolls', 'doll,toys');
    SLS_CREATE_PRODUCT('Baby Doll', 'Realistic baby doll', 20, 'Toys', 'Dolls', 'doll,toys');
    -- Puzzles
    SLS_CREATE_PRODUCT('Jigsaw Puzzle', '1000-piece jigsaw puzzle', 15, 'Toys', 'Puzzles', 'puzzle,toys');
    SLS_CREATE_PRODUCT('3D Puzzle', 'Challenging 3D puzzle', 25, 'Toys', 'Puzzles', 'puzzle,toys');
    -- Educational Toys
    SLS_CREATE_PRODUCT('Building Blocks', 'Set of building blocks', 30, 'Toys', 'Educational Toys', 'blocks,toys');
    SLS_CREATE_PRODUCT('Science Kit', 'Fun science experiment kit', 40, 'Toys', 'Educational Toys', 'kit,toys');
    -- Outdoor Toys
    SLS_CREATE_PRODUCT('Swing Set', 'Backyard swing set', 200, 'Toys', 'Outdoor Toys', 'swing,toys');
    SLS_CREATE_PRODUCT('Trampoline', 'Large outdoor trampoline', 300, 'Toys', 'Outdoor Toys', 'trampoline,toys');

    -- Beauty
    -- Skincare
    SLS_CREATE_PRODUCT('Moisturizing Cream', 'Hydrating face cream', 25, 'Beauty', 'Skincare', 'cream,skincare');
    SLS_CREATE_PRODUCT('Anti-Aging Serum', 'Serum to reduce wrinkles', 40, 'Beauty', 'Skincare', 'serum,skincare');
    -- Makeup
    SLS_CREATE_PRODUCT('Foundation', 'Liquid foundation for all skin types', 30, 'Beauty', 'Makeup', 'foundation,makeup');
    SLS_CREATE_PRODUCT('Mascara', 'Volumizing mascara', 20, 'Beauty', 'Makeup', 'mascara,makeup');
    -- Haircare
    SLS_CREATE_PRODUCT('Shampoo', 'Nourishing shampoo', 15, 'Beauty', 'Haircare', 'shampoo,haircare');
    SLS_CREATE_PRODUCT('Conditioner', 'Moisturizing conditioner', 15, 'Beauty', 'Haircare', 'conditioner,haircare');
    -- Fragrances
    SLS_CREATE_PRODUCT('Eau de Parfum', 'Long-lasting perfume', 50, 'Beauty', 'Fragrances', 'perfume,fragrances');
    SLS_CREATE_PRODUCT('Body Mist', 'Refreshing body mist', 20, 'Beauty', 'Fragrances', 'mist,fragrances');
    -- Tools
    SLS_CREATE_PRODUCT('Makeup Brushes', 'Set of makeup brushes', 25, 'Beauty', 'Tools', 'brushes,tools');
    SLS_CREATE_PRODUCT('Hair Dryer', 'Professional hair dryer', 60, 'Beauty', 'Tools', 'dryer,tools');

    -- Automotive
    -- Car Accessories
    SLS_CREATE_PRODUCT('Car Seat Cover', 'Comfortable car seat cover', 30, 'Automotive', 'Car Accessories', 'cover,car');
    SLS_CREATE_PRODUCT('Car Phone Holder', 'Adjustable car phone holder', 15, 'Automotive', 'Car Accessories', 'holder,car');
    -- Motorcycle Accessories
    SLS_CREATE_PRODUCT('Motorcycle Helmet', 'Safety helmet for motorcycle', 80, 'Automotive', 'Motorcycle Accessories', 'helmet,motorcycle');
    SLS_CREATE_PRODUCT('Motorcycle Gloves', 'Protective gloves for motorcycle', 25, 'Automotive', 'Motorcycle Accessories', 'gloves,motorcycle');
    -- Tools
    SLS_CREATE_PRODUCT('Wrench Set', 'Set of wrenches', 40, 'Automotive', 'Tools', 'wrench,tools');
    SLS_CREATE_PRODUCT('Screwdriver Set', 'Set of screwdrivers', 20, 'Automotive', 'Tools', 'screwdriver,tools');
    -- Parts
    SLS_CREATE_PRODUCT('Brake Pads', 'High-quality brake pads', 50, 'Automotive', 'Parts', 'brake,parts');
    SLS_CREATE_PRODUCT('Air Filter', 'Durable air filter', 20, 'Automotive', 'Parts', 'filter,parts');
    -- Electronics
    SLS_CREATE_PRODUCT('Car Stereo', 'High-performance car stereo', 100, 'Automotive', 'Electronics', 'stereo,car');
    SLS_CREATE_PRODUCT('GPS Navigator', 'Reliable GPS navigator', 120, 'Automotive', 'Electronics', 'gps,car');

    -- Garden
    -- Plants
    SLS_CREATE_PRODUCT('Rose Plant', 'Beautiful rose plant', 15, 'Garden', 'Plants', 'rose,plants');
    SLS_CREATE_PRODUCT('Cactus Plant', 'Low-maintenance cactus plant', 10, 'Garden', 'Plants', 'cactus,plants');
    -- Tools
    SLS_CREATE_PRODUCT('Garden Shovel', 'Durable garden shovel', 20, 'Garden', 'Tools', 'shovel,tools');
    SLS_CREATE_PRODUCT('Pruning Shears', 'Sharp pruning shears', 15, 'Garden', 'Tools', 'shears,tools');
    -- Outdoor Furniture
    SLS_CREATE_PRODUCT('Garden Bench', 'Comfortable garden bench', 100, 'Garden', 'Outdoor Furniture', 'bench,furniture');
    SLS_CREATE_PRODUCT('Patio Table', 'Stylish patio table', 80, 'Garden', 'Outdoor Furniture', 'table,furniture');
    -- Decorations
    SLS_CREATE_PRODUCT('Garden Gnome', 'Cute garden gnome', 25, 'Garden', 'Decorations', 'gnome,decorations');
    SLS_CREATE_PRODUCT('Wind Chime', 'Relaxing wind chime', 15, 'Garden', 'Decorations', 'chime,decorations');
    -- Watering Equipment
    SLS_CREATE_PRODUCT('Garden Hose', 'Flexible garden hose', 30, 'Garden', 'Watering Equipment', 'hose,watering');
    SLS_CREATE_PRODUCT('Sprinkler', 'Efficient garden sprinkler', 25, 'Garden', 'Watering Equipment', 'sprinkler,watering');

    -- Health
    -- Supplements
    SLS_CREATE_PRODUCT('Vitamin C', 'Immune support supplement', 20, 'Health', 'Supplements', 'vitamin,supplements');
    SLS_CREATE_PRODUCT('Protein Powder', 'High-quality protein powder', 40, 'Health', 'Supplements', 'protein,supplements');
    -- Medical Equipment
    SLS_CREATE_PRODUCT('Blood Pressure Monitor', 'Accurate blood pressure monitor', 50, 'Health', 'Medical Equipment', 'monitor,medical');
    SLS_CREATE_PRODUCT('Thermometer', 'Digital thermometer', 15, 'Health', 'Medical Equipment', 'thermometer,medical');
    -- Personal Care
    SLS_CREATE_PRODUCT('Electric Toothbrush', 'Rechargeable electric toothbrush', 30, 'Health', 'Personal Care', 'toothbrush,personal');
    SLS_CREATE_PRODUCT('Hair Clippers', 'Professional hair clippers', 40, 'Health', 'Personal Care', 'clippers,personal');
    -- Fitness Equipment
    SLS_CREATE_PRODUCT('Yoga Mat', 'Non-slip yoga mat', 25, 'Health', 'Fitness Equipment', 'mat,fitness');
    SLS_CREATE_PRODUCT('Dumbbells', 'Set of dumbbells', 50, 'Health', 'Fitness Equipment', 'dumbbells,fitness');

    -- Office Supplies
    -- Stationery
    SLS_CREATE_PRODUCT('Notebook', 'Lined notebook', 5, 'Office Supplies', 'Stationery', 'notebook,stationery');
    SLS_CREATE_PRODUCT('Pen Set', 'Set of ballpoint pens', 10, 'Office Supplies', 'Stationery', 'pen,stationery');
    -- Printers
    SLS_CREATE_PRODUCT('Laser Printer', 'High-speed laser printer', 150, 'Office Supplies', 'Printers', 'printer,office');
    SLS_CREATE_PRODUCT('Inkjet Printer', 'Color inkjet printer', 100, 'Office Supplies', 'Printers', 'printer,office');
    -- Office Furniture
    SLS_CREATE_PRODUCT('Office Chair', 'Ergonomic office chair', 120, 'Office Supplies', 'Office Furniture', 'chair,furniture');
    SLS_CREATE_PRODUCT('Desk', 'Spacious office desk', 200, 'Office Supplies', 'Office Furniture', 'desk,furniture');
    -- Storage
    SLS_CREATE_PRODUCT('Filing Cabinet', 'Metal filing cabinet', 80, 'Office Supplies', 'Storage', 'cabinet,storage');
    SLS_CREATE_PRODUCT('Bookshelf', 'Wooden bookshelf', 100, 'Office Supplies', 'Storage', 'bookshelf,storage');
    -- Electronics
    SLS_CREATE_PRODUCT('Monitor', '24-inch computer monitor', 150, 'Office Supplies', 'Electronics', 'monitor,office');
    SLS_CREATE_PRODUCT('Keyboard', 'Mechanical keyboard', 50, 'Office Supplies', 'Electronics', 'keyboard,office');

    -- Pet Supplies
    -- Food
    SLS_CREATE_PRODUCT('Dog Food', 'Nutritious dog food', 30, 'Pet Supplies', 'Food', 'food,pet');
    SLS_CREATE_PRODUCT('Cat Food', 'Healthy cat food', 25, 'Pet Supplies', 'Food', 'food,pet');
    -- Toys
    SLS_CREATE_PRODUCT('Dog Toy', 'Chew toy for dogs', 10, 'Pet Supplies', 'Toys', 'toy,pet');
    SLS_CREATE_PRODUCT('Cat Toy', 'Interactive toy for cats', 15, 'Pet Supplies', 'Toys', 'toy,pet');
    -- Accessories
    SLS_CREATE_PRODUCT('Dog Leash', 'Durable dog leash', 20, 'Pet Supplies', 'Accessories', 'leash,pet');
    SLS_CREATE_PRODUCT('Cat Collar', 'Adjustable cat collar', 15, 'Pet Supplies', 'Accessories', 'collar,pet');
    -- Grooming
    SLS_CREATE_PRODUCT('Dog Shampoo', 'Gentle dog shampoo', 15, 'Pet Supplies', 'Grooming', 'shampoo,pet');
    SLS_CREATE_PRODUCT('Cat Brush', 'Soft brush for cats', 10, 'Pet Supplies', 'Grooming', 'brush,pet');
    -- Health
    SLS_CREATE_PRODUCT('Flea Treatment', 'Effective flea treatment', 25, 'Pet Supplies', 'Health', 'flea,pet');
    SLS_CREATE_PRODUCT('Pet Vitamins', 'Vitamins for pets', 20, 'Pet Supplies', 'Health', 'vitamins,pet');
END;
/


CREATE OR REPLACE PROCEDURE INSERT_DISCOUNT_TYPE (
    p_discount_type_id IN NUMBER,
    p_discount_type_name IN VARCHAR2
) IS
    discount_type_exists_by_id NUMBER := 0;
    discount_type_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the discount type ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_discount_type_id THEN 1 END) INTO discount_type_exists_by_id,
        COUNT(CASE WHEN UPPER(name) = UPPER(p_discount_type_name) THEN 1 END) INTO discount_type_exists_by_name
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

BEGIN
    INSERT_DISCOUNT_TYPE(0, 'Absolute value');
    INSERT_DISCOUNT_TYPE(1, 'Percentage');
END;
/


CREATE OR REPLACE PROCEDURE INSERT_DISCOUNT_REASON (
    p_discount_reason_id IN NUMBER,
    p_discount_reason_name IN VARCHAR2
) IS
    discount_reason_exists_by_id NUMBER := 0;
    discount_reason_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the discount reason ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_discount_reason_id THEN 1 END) INTO discount_reason_exists_by_id,
        COUNT(CASE WHEN UPPER(name) = UPPER(p_discount_reason_name) THEN 1 END) INTO discount_reason_exists_by_name
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


CREATE OR REPLACE PROCEDURE INSERT_ORDER_STATUS (
    p_status_id IN NUMBER,
    p_status_name IN VARCHAR2
) IS
    status_exists_by_id NUMBER := 0;
    status_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the order status ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_status_id THEN 1 END) INTO status_exists_by_id,
        COUNT(CASE WHEN UPPER(name) = UPPER(p_status_name) THEN 1 END) INTO status_exists_by_name
    FROM SLS_ORDER_STATUSES;

    -- Ensure both ID and name are unique
    IF status_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_ORDER_STATUS: Status ID ' || p_status_id || ' already exists.');
    ELSIF status_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_ORDER_STATUS: Status name ' || p_status_name || ' already exists.');
    ELSE
        -- Insert the order status if both ID and name are unique
        INSERT INTO SLS_ORDER_STATUSES (id, name)
        VALUES (p_status_id, p_status_name);
        LOG_INFORMATION('INSERT_ORDER_STATUS: Status ' || p_status_name || ' with ID ' || p_status_id || ' created.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_ORDER_STATUS: Error creating status ' || p_status_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

BEGIN
    INSERT_ORDER_STATUS(0, 'Pending');
    INSERT_ORDER_STATUS(1, 'Completed');
    INSERT_ORDER_STATUS(2, 'Canceled');
END;
/


CREATE OR REPLACE PROCEDURE INSERT_INVOICE_STATUS (
    p_status_id IN NUMBER,
    p_status_name IN VARCHAR2
) IS
    status_exists_by_id NUMBER := 0;
    status_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the invoice status ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_status_id THEN 1 END) INTO status_exists_by_id,
        COUNT(CASE WHEN UPPER(name) = UPPER(p_status_name) THEN 1 END) INTO status_exists_by_name
    FROM BLG_INVOICE_STATUSES;

    -- Ensure both ID and name are unique
    IF status_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_INVOICE_STATUS: Status ID ' || p_status_id || ' already exists.');
    ELSIF status_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_INVOICE_STATUS: Status name ' || p_status_name || ' already exists.');
    ELSE
        -- Insert the invoice status if both ID and name are unique
        INSERT INTO BLG_INVOICE_STATUSES (id, name)
        VALUES (p_status_id, p_status_name);
        LOG_INFORMATION('INSERT_INVOICE_STATUS: Status ' || p_status_name || ' with ID ' || p_status_id || ' created.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_INVOICE_STATUS: Error creating status ' || p_status_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

BEGIN
    INSERT_INVOICE_STATUS(0, 'Pending');
    INSERT_INVOICE_STATUS(1, 'Paid');
    INSERT_INVOICE_STATUS(2, 'Overdue');
    INSERT_INVOICE_STATUS(3, 'Canceled');
END;
/
