-- Enable DBMS_OUTPUT
SET SERVEROUTPUT ON;

-- Switch to ESHOP_GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

-- Set schema
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER;

-- Log schema
BEGIN
    DBMS_OUTPUT.PUT_LINE('Creating database link in schema: ' || SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA'));
END;
/

-- Drop and recreate LINK_TO_MUNTENIA
BEGIN
    EXECUTE IMMEDIATE 'DROP DATABASE LINK link_to_muntenia';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -942 THEN
            RAISE;
        END IF;
END;
/

BEGIN
    EXECUTE IMMEDIATE '
        CREATE DATABASE LINK link_to_muntenia
        CONNECT TO eshop_muntenia_user IDENTIFIED BY ''MunteniaUserPassword123!''
        USING ''(DESCRIPTION =
                    (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                    (CONNECT_DATA = (SERVICE_NAME = ESHOP_MUNTENIA)))''';
END;
/

-- Test LINK_TO_MUNTENIA
BEGIN
    FOR rec IN (SELECT 'LINK_TO_MUNTENIA is working' AS status FROM dual@link_to_muntenia) LOOP
        DBMS_OUTPUT.PUT_LINE(rec.status);
    END LOOP;
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Error testing LINK_TO_MUNTENIA: ' || SQLERRM);
END;
/

-- Drop and recreate LINK_TO_ROMANIA
BEGIN
    EXECUTE IMMEDIATE 'DROP DATABASE LINK link_to_romania';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -942 THEN
            RAISE;
        END IF;
END;
/

BEGIN
    EXECUTE IMMEDIATE '
        CREATE DATABASE LINK link_to_romania
        CONNECT TO eshop_romania_user IDENTIFIED BY ''RomaniaUserPassword123!''
        USING ''(DESCRIPTION =
                    (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                    (CONNECT_DATA = (SERVICE_NAME = ESHOP_ROMANIA)))''';
END;
/

-- Test LINK_TO_ROMANIA
BEGIN
    FOR rec IN (SELECT 'LINK_TO_ROMANIA is working' AS status FROM dual@link_to_romania) LOOP
        DBMS_OUTPUT.PUT_LINE(rec.status);
    END LOOP;
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Error testing LINK_TO_ROMANIA: ' || SQLERRM);
END;
/

-- Switch to ESHOP_MUNTENIA PDB
ALTER SESSION SET CONTAINER = eshop_muntenia;

ALTER SESSION SET CURRENT_SCHEMA = ESHOP_MUNTENIA_USER;

-- Log schema
BEGIN
    DBMS_OUTPUT.PUT_LINE('Creating database link in schema: ' || SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA'));
END;
/

-- Drop and recreate LINK_TO_GLOBAL
BEGIN
    EXECUTE IMMEDIATE 'DROP DATABASE LINK link_to_global';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -942 THEN
            RAISE;
        END IF;
END;
/

BEGIN
    EXECUTE IMMEDIATE '
        CREATE DATABASE LINK link_to_global
        CONNECT TO eshop_global_user IDENTIFIED BY ''GlobalUserPassword123!''
        USING ''(DESCRIPTION =
                    (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                    (CONNECT_DATA = (SERVICE_NAME = ESHOP_GLOBAL)))''';
END;
/

-- Test LINK_TO_GLOBAL from ESHOP_MUNTENIA
BEGIN
    FOR rec IN (SELECT 'LINK_TO_GLOBAL from ESHOP_MUNTENIA is working' AS status FROM dual@link_to_global) LOOP
        DBMS_OUTPUT.PUT_LINE(rec.status);
    END LOOP;
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Error testing LINK_TO_GLOBAL from ESHOP_MUNTENIA: ' || SQLERRM);
END;
/

-- Switch to ESHOP_ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

ALTER SESSION SET CURRENT_SCHEMA = ESHOP_ROMANIA_USER;

-- Log schema
BEGIN
    DBMS_OUTPUT.PUT_LINE('Creating database link in schema: ' || SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA'));
END;
/

-- Drop and recreate LINK_TO_GLOBAL
BEGIN
    EXECUTE IMMEDIATE 'DROP DATABASE LINK link_to_global';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -942 THEN
            RAISE;
        END IF;
END;
/

BEGIN
    EXECUTE IMMEDIATE '
        CREATE DATABASE LINK link_to_global
        CONNECT TO eshop_global_user IDENTIFIED BY ''GlobalUserPassword123!''
        USING ''(DESCRIPTION =
                    (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                    (CONNECT_DATA = (SERVICE_NAME = ESHOP_GLOBAL)))''';
END;
/

-- Test LINK_TO_GLOBAL from ESHOP_ROMANIA
BEGIN
    FOR rec IN (SELECT 'LINK_TO_GLOBAL from ESHOP_ROMANIA is working' AS status FROM dual@link_to_global) LOOP
        DBMS_OUTPUT.PUT_LINE(rec.status);
    END LOOP;
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Error testing LINK_TO_GLOBAL from ESHOP_ROMANIA: ' || SQLERRM);
END;
/
