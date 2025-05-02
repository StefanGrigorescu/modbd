-- Switch to ESHOP_GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

-- Create database link to ESHOP_MUNTENIA
BEGIN
  EXECUTE IMMEDIATE '
    CREATE DATABASE LINK link_to_muntenia
    CONNECT TO "muntenia_admin" IDENTIFIED BY "MunteniaAdminPassword123!"
    USING ''(DESCRIPTION =
      (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
      (CONNECT_DATA = (SERVICE_NAME = ESHOP_MUNTENIA)))''';
END;
/

-- Create database link to ESHOP_ROMANIA
BEGIN
  EXECUTE IMMEDIATE '
    CREATE DATABASE LINK link_to_romania
    CONNECT TO "romania_admin" IDENTIFIED BY "RomaniaAdminPassword123!"
    USING ''(DESCRIPTION =
      (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
      (CONNECT_DATA = (SERVICE_NAME = ESHOP_ROMANIA)))''';
END;
/

-- Switch to ESHOP_MUNTENIA PDB
ALTER SESSION SET CONTAINER = eshop_muntenia;

-- Create database link to ESHOP_GLOBAL
BEGIN
  EXECUTE IMMEDIATE '
    CREATE DATABASE LINK link_to_global
    CONNECT TO "global_admin" IDENTIFIED BY "GlobalAdminPassword123!"
    USING ''(DESCRIPTION =
      (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
      (CONNECT_DATA = (SERVICE_NAME = ESHOP_GLOBAL)))''';
END;
/

-- Switch to ESHOP_ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

-- Create database link to ESHOP_GLOBAL
BEGIN
  EXECUTE IMMEDIATE '
    CREATE DATABASE LINK link_to_global
    CONNECT TO "global_admin" IDENTIFIED BY "GlobalAdminPassword123!"
    USING ''(DESCRIPTION =
      (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
      (CONNECT_DATA = (SERVICE_NAME = ESHOP_GLOBAL)))''';
END;
/
