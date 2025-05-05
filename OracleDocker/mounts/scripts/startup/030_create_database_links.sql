-- Switch to ESHOP_GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

DECLARE
    link_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*) INTO link_exists
    FROM ALL_DB_LINKS
    WHERE DB_LINK = 'LINK_TO_MUNTENIA';

    IF link_exists = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE PUBLIC DATABASE LINK link_to_muntenia
            CONNECT TO "muntenia_admin" IDENTIFIED BY "MunteniaAdminPassword123!"
            USING ''(DESCRIPTION =
                (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                (CONNECT_DATA = (SERVICE_NAME = ESHOP_MUNTENIA)))''';
    END IF;
END;
/

DECLARE
    link_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*) INTO link_exists
    FROM ALL_DB_LINKS
    WHERE DB_LINK = 'LINK_TO_ROMANIA';

    IF link_exists = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE PUBLIC DATABASE LINK link_to_romania
            CONNECT TO "romania_admin" IDENTIFIED BY "RomaniaAdminPassword123!"
            USING ''(DESCRIPTION =
                (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                (CONNECT_DATA = (SERVICE_NAME = ESHOP_ROMANIA)))''';
    END IF;
END;
/


-- Switch to ESHOP_MUNTENIA PDB
ALTER SESSION SET CONTAINER = eshop_muntenia;

DECLARE
    link_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*) INTO link_exists
    FROM ALL_DB_LINKS
    WHERE DB_LINK = 'LINK_TO_GLOBAL';

    IF link_exists = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE PUBLIC DATABASE LINK link_to_global
            CONNECT TO "global_admin" IDENTIFIED BY "GlobalAdminPassword123!"
            USING ''(DESCRIPTION =
                (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                (CONNECT_DATA = (SERVICE_NAME = ESHOP_GLOBAL)))''';
    END IF;
END;
/


-- Switch to ESHOP_ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

DECLARE
    link_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*) INTO link_exists
    FROM ALL_DB_LINKS
    WHERE DB_LINK = 'LINK_TO_GLOBAL';

    IF link_exists = 0 THEN
        EXECUTE IMMEDIATE '
            CREATE PUBLIC DATABASE LINK link_to_global
            CONNECT TO "global_admin" IDENTIFIED BY "GlobalAdminPassword123!"
            USING ''(DESCRIPTION =
                (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                (CONNECT_DATA = (SERVICE_NAME = ESHOP_GLOBAL)))''';
    END IF;
END;
/
