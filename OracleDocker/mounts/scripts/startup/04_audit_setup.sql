SET SERVEROUTPUT ON;

-- Create sequence for id generation of logs table
DECLARE
    seq_exists NUMBER := 0;
BEGIN
    -- Check if sequence exists
    SELECT COUNT(*) INTO seq_exists
    FROM USER_SEQUENCES
    WHERE UPPER(SEQUENCE_NAME) = 'ESHOP_LOGS_SEQ';

    IF seq_exists = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE SEQUENCE ESHOP_LOGS_SEQ
            START WITH 1
            INCREMENT BY 1
            NOCACHE
            NOCYCLE';
        DBMS_OUTPUT.PUT_LINE('Sequence ESHOP_LOGS_SEQ created.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Sequence ESHOP_LOGS_SEQ already exists.');
    END IF;
END;
/


-- Create logs table
DECLARE
    table_exists NUMBER := 0;
BEGIN
    -- Check if table exists
    SELECT COUNT(*) INTO table_exists
    FROM USER_TABLES
    WHERE UPPER(TABLE_NAME) = 'ESHOP_LOGS';

    IF table_exists = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE TABLE ESHOP_LOGS (
                id INT PRIMARY KEY,
                message NVARCHAR2(255) NOT NULL,
                message_type VARCHAR2(1) NOT NULL,    -- D (Debug), I (Information), W (Warning), E (Error), C (Critical)
                created_by VARCHAR2(150) DEFAULT NULL,
                created_at DATE NOT NULL,
                current_user VARCHAR2(150) NOT NULL
            )';
        DBMS_OUTPUT.PUT_LINE('Table ESHOP_LOGS created.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('Table ESHOP_LOGS already exists.');
    END IF;
END;
/


-- Create stored procedures for logging
CREATE OR REPLACE PROCEDURE LOG_DEBUG (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_LOGS (id, message, message_type, created_by, created_at, current_user)
    VALUES (ESHOP_LOGS_SEQ.NEXTVAL, message, 'D', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] DEBUG: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/

CREATE OR REPLACE PROCEDURE LOG_INFORMATION (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_LOGS (id, message, message_type, created_by, created_at, current_user)
    VALUES (ESHOP_LOGS_SEQ.NEXTVAL, message, 'I', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] INFO: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/

CREATE OR REPLACE PROCEDURE LOG_WARNING (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_LOGS (id, message, message_type, created_by, created_at, current_user)
    VALUES (ESHOP_LOGS_SEQ.NEXTVAL, message, 'W', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] WARNING: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/

CREATE OR REPLACE PROCEDURE LOG_ERROR (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_LOGS (id, message, message_type, created_by, created_at, current_user)
    VALUES (ESHOP_LOGS_SEQ.NEXTVAL, message, 'E', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] ERROR: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/

CREATE OR REPLACE PROCEDURE LOG_CRITICAL (
    message IN NVARCHAR2,
    created_by IN VARCHAR2 DEFAULT NULL
) IS
BEGIN
    INSERT INTO ESHOP_LOGS (id, message, message_type, created_by, created_at, current_user)
    VALUES (ESHOP_LOGS_SEQ.NEXTVAL, message, 'C', created_by, SYSDATE, USER);
    DBMS_OUTPUT.PUT_LINE('[' || TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') || '] CRITICAL: ' || message || ' | Created by: ' || created_by || ' | Current user: ' || USER);
END;
/