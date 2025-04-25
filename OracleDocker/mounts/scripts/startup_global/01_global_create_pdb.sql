WHENEVER SQLERROR EXIT SQL.SQLCODE;

-- Check if the ESHOP_GLOBAL PDB already exists
DECLARE
  pdb_exists NUMBER := 0;
BEGIN
  SELECT COUNT(*) INTO pdb_exists FROM V$PDBS WHERE NAME = 'ESHOP_GLOBAL';

  IF pdb_exists = 0 THEN
    -- Create the ESHOP_GLOBAL PDB
    EXECUTE IMMEDIATE 'CREATE PLUGGABLE DATABASE ESHOP_GLOBAL ADMIN USER global_admin IDENTIFIED BY "GlobalAdminPassword123!" FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed'', ''/opt/oracle/oradata/GLOBALCDB/ESHOP_GLOBAL'')';
    EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE ESHOP_GLOBAL OPEN';
    DBMS_OUTPUT.PUT_LINE('ESHOP_GLOBAL PDB was successfully created and opened.');
  ELSE
    DBMS_OUTPUT.PUT_LINE('ESHOP_GLOBAL PDB already exists.');
  END IF;
END;
/

-- Ensure the GLOBAL_ADMIN user exists and has the DBA role
BEGIN
  EXECUTE IMMEDIATE 'ALTER SESSION SET CONTAINER = ESHOP_GLOBAL';

  -- Check if the GLOBAL_ADMIN user exists
  DECLARE
    user_exists NUMBER := 0;
  BEGIN
    SELECT COUNT(*) INTO user_exists FROM DBA_USERS WHERE USERNAME = 'GLOBAL_ADMIN';

    IF user_exists = 0 THEN
      -- Create the GLOBAL_ADMIN user
      EXECUTE IMMEDIATE 'CREATE USER global_admin IDENTIFIED BY "GlobalAdminPassword123!"';
      DBMS_OUTPUT.PUT_LINE('GLOBAL_ADMIN user was successfully created.');

      -- Grant the DBA role to GLOBAL_ADMIN
      EXECUTE IMMEDIATE 'GRANT DBA TO global_admin';
      DBMS_OUTPUT.PUT_LINE('GLOBAL_ADMIN user was granted the DBA role.');
    ELSE
      DBMS_OUTPUT.PUT_LINE('GLOBAL_ADMIN user already exists.');
    END IF;
  END;
END;
/