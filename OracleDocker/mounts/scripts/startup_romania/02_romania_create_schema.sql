SET SERVEROUTPUT ON;

-- Switch to ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

-- Check if user eshop_romania_user exists
DECLARE
    user_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*)
    INTO user_exists
    FROM dba_users
    WHERE username = 'ESHOP_ROMANIA_USER';

    IF user_exists = 0 THEN
        -- Create ROMANIA schema
        EXECUTE IMMEDIATE '
            CREATE USER eshop_romania_user IDENTIFIED BY "RomaniaUserPassword123!"
            ACCOUNT UNLOCK
        ';
        DBMS_OUTPUT.PUT_LINE('romania_create_schema: User eshop_romania_user was successfully created.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('romania_create_schema: User eshop_romania_user already exists.');
    END IF;
END;
/

-- Grant necessary privileges directly to the user
BEGIN
    EXECUTE IMMEDIATE 'GRANT CONNECT TO ESHOP_ROMANIA_USER';
    EXECUTE IMMEDIATE 'GRANT RESOURCE TO ESHOP_ROMANIA_USER';
    EXECUTE IMMEDIATE 'GRANT CREATE TABLE TO ESHOP_ROMANIA_USER';
    EXECUTE IMMEDIATE 'GRANT CREATE VIEW TO ESHOP_ROMANIA_USER';
    EXECUTE IMMEDIATE 'GRANT CREATE SEQUENCE TO ESHOP_ROMANIA_USER';
    EXECUTE IMMEDIATE 'GRANT CREATE TRIGGER TO ESHOP_ROMANIA_USER';
    EXECUTE IMMEDIATE 'GRANT UNLIMITED TABLESPACE TO ESHOP_ROMANIA_USER';

    DBMS_OUTPUT.PUT_LINE('romania_create_schema: Necessary privileges were granted to eshop_romania_user.');
END;
/