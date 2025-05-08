SET SERVEROUTPUT ON;

-- Switch to MUNTENIA PDB
ALTER SESSION SET CONTAINER = eshop_muntenia;

ALTER SESSION SET CURRENT_SCHEMA = ESHOP_MUNTENIA_USER;

-- Create logs table
DECLARE
    table_exists NUMBER := 0;
BEGIN
    -- Check if table exists
    SELECT COUNT(*) INTO table_exists
    FROM USER_TABLES
    WHERE UPPER(TABLE_NAME) = 'ESHOP_MUNTENIA_LOGS';

    IF table_exists = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE TABLE ESHOP_MUNTENIA_LOGS (
                id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                message NVARCHAR2(255) NOT NULL,
                message_type VARCHAR2(1) NOT NULL,    -- D (Debug), I (Information), W (Warning), E (Error), C (Critical)
                created_by VARCHAR2(150) DEFAULT NULL,
                created_at DATE NOT NULL,
                current_user VARCHAR2(150) NOT NULL
            )';
        DBMS_OUTPUT.PUT_LINE('Table ESHOP_MUNTENIA_LOGS created.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Table ESHOP_MUNTENIA_LOGS already exists.');
    END IF;
END;
/


-- Create stored procedures for logging
CREATE OR REPLACE PROCEDURE LOG_DEBUG (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_MUNTENIA_LOGS (message, message_type, created_by, created_at, current_user)
    VALUES (SUBSTR(message, 1, 255), 'D', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] DEBUG: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/

CREATE OR REPLACE PROCEDURE LOG_INFORMATION (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_MUNTENIA_LOGS (message, message_type, created_by, created_at, current_user)
    VALUES (SUBSTR(message, 1, 255), 'I', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] INFO: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/

CREATE OR REPLACE PROCEDURE LOG_WARNING (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_MUNTENIA_LOGS (message, message_type, created_by, created_at, current_user)
    VALUES (SUBSTR(message, 1, 255), 'W', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] WARNING: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/

CREATE OR REPLACE PROCEDURE LOG_ERROR (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_MUNTENIA_LOGS (message, message_type, created_by, created_at, current_user)
    VALUES (SUBSTR(message, 1, 255), 'E', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] ERROR: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/

CREATE OR REPLACE PROCEDURE LOG_CRITICAL (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_MUNTENIA_LOGS (message, message_type, created_by, created_at, current_user)
    VALUES (SUBSTR(message, 1, 255), 'C', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] CRITICAL: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/
