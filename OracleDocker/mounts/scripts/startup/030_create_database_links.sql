-- Switch to GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

-- Drop the database link if it already exists
BEGIN
    EXECUTE IMMEDIATE 'DROP PUBLIC DATABASE LINK eshop_romania_link';
    DBMS_OUTPUT.PUT_LINE('Dropped database link: eshop_romania_link');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1031 THEN -- Ignore "link does not exist" error
            RAISE;
        ELSE
            DBMS_OUTPUT.PUT_LINE('Database link eshop_romania_link does not exist.');
        END IF;
END;
/

BEGIN
    EXECUTE IMMEDIATE 'DROP PUBLIC DATABASE LINK eshop_muntenia_link';
    DBMS_OUTPUT.PUT_LINE('Dropped database link: eshop_muntenia_link');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1031 THEN -- Ignore "link does not exist" error
            RAISE;
        ELSE
            DBMS_OUTPUT.PUT_LINE('Database link eshop_muntenia_link does not exist.');
        END IF;
END;
/

-- Create a public database link to ESHOP_ROMANIA
CREATE PUBLIC DATABASE LINK eshop_romania_link
CONNECT TO eshop_romania_user IDENTIFIED BY "RomaniaUserPassword123!"
USING '(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ESHOP_ROMANIA)))';

-- Create a public database link to ESHOP_MUNTENIA
CREATE PUBLIC DATABASE LINK eshop_muntenia_link
CONNECT TO eshop_muntenia_user IDENTIFIED BY "MunteniaUserPassword123!"
USING '(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ESHOP_MUNTENIA)))';


-- drop the database link if it already exists
BEGIN
    EXECUTE IMMEDIATE 'DROP PUBLIC DATABASE LINK eshop_oltp_link';
    DBMS_OUTPUT.PUT_LINE('Dropped database link: eshop_oltp_link');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1031 THEN -- Ignore "link does not exist" error
            RAISE;
        ELSE
            DBMS_OUTPUT.PUT_LINE('Database link eshop_oltp_link does not exist.');
        END IF;
END;
/

-- create a public database link to oltp
CREATE PUBLIC DATABASE LINK eshop_oltp_link
CONNECT TO eshop_oltp_user IDENTIFIED BY "oltp_password"
USING '(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ESHOP_OLTP)))';


-- Switch to ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

-- Drop the database link if it already exists
BEGIN
    EXECUTE IMMEDIATE 'DROP PUBLIC DATABASE LINK eshop_romania_global_link';
    DBMS_OUTPUT.PUT_LINE('Dropped database link: eshop_romania_global_link');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1031 THEN -- Ignore "link does not exist" error
            RAISE;
        ELSE
            DBMS_OUTPUT.PUT_LINE('Database link eshop_romania_link does not exist.');
        END IF;
END;
/

-- Create a public database link to ESHOP_ROMANIA_GLOBAL_LINK
CREATE PUBLIC DATABASE LINK eshop_romania_global_link
CONNECT TO eshop_global_user IDENTIFIED BY "GlobalUserPassword123!"
USING '(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ESHOP_GLOBAL)))';

-- drop the database link if it already exists
BEGIN
    EXECUTE IMMEDIATE 'DROP PUBLIC DATABASE LINK eshop_oltp_link';
    DBMS_OUTPUT.PUT_LINE('Dropped database link: eshop_oltp_link');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1031 THEN -- Ignore "link does not exist" error
            RAISE;
        ELSE
            DBMS_OUTPUT.PUT_LINE('Database link eshop_oltp_link does not exist.');
        END IF;
END;
/

-- create a public database link to oltp
CREATE PUBLIC DATABASE LINK eshop_oltp_link
CONNECT TO eshop_oltp_user IDENTIFIED BY "oltp_password"
USING '(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ESHOP_OLTP)))';

-- Switch to MUNTENIA PDB
ALTER SESSION SET CONTAINER = eshop_muntenia;

-- Drop the database link if it already exists
BEGIN
    EXECUTE IMMEDIATE 'DROP PUBLIC DATABASE LINK eshop_muntenia_global_link';
    DBMS_OUTPUT.PUT_LINE('Dropped database link: eshop_muntenia_global_link');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1031 THEN -- Ignore "link does not exist" error
            RAISE;
        ELSE
            DBMS_OUTPUT.PUT_LINE('Database link eshop_muntenia_link does not exist.');
        END IF;
END;
/

-- Create a public database link to ESHOP_MUNTENIA_GLOBAL_LINK
CREATE PUBLIC DATABASE LINK eshop_muntenia_global_link
CONNECT TO eshop_global_user IDENTIFIED BY "GlobalUserPassword123!"
USING '(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ESHOP_GLOBAL)))';

-- drop the database link if it already exists
BEGIN
    EXECUTE IMMEDIATE 'DROP PUBLIC DATABASE LINK eshop_oltp_link';
    DBMS_OUTPUT.PUT_LINE('Dropped database link: eshop_oltp_link');
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1031 THEN -- Ignore "link does not exist" error
            RAISE;
        ELSE
            DBMS_OUTPUT.PUT_LINE('Database link eshop_oltp_link does not exist.');
        END IF;
END;
/

-- create a public database link to oltp
CREATE PUBLIC DATABASE LINK eshop_oltp_link
CONNECT TO eshop_oltp_user IDENTIFIED BY "oltp_password"
USING '(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ESHOP_OLTP)))';
