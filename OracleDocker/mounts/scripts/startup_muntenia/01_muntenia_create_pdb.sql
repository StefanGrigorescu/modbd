WHENEVER SQLERROR EXIT SQL.SQLCODE;

-- Check if the ESHOP_MUNTENIA PDB already exists
DECLARE
  pdb_exists NUMBER := 0;
BEGIN
  SELECT COUNT(*) INTO pdb_exists FROM V$PDBS WHERE NAME = 'ESHOP_MUNTENIA';

  IF pdb_exists = 0 THEN
    -- Create the ESHOP_MUNTENIA PDB
    EXECUTE IMMEDIATE 'CREATE PLUGGABLE DATABASE ESHOP_MUNTENIA ADMIN USER muntenia_admin IDENTIFIED BY "MunteniaAdminPassword123!" FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed'', ''/opt/oracle/oradata/MUNTENIACDB/ESHOP_MUNTENIA'')';
    EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE ESHOP_MUNTENIA OPEN';
    DBMS_OUTPUT.PUT_LINE('ESHOP_MUNTENIA PDB was successfully created and opened.');
  ELSE
    DBMS_OUTPUT.PUT_LINE('ESHOP_MUNTENIA PDB already exists.');
  END IF;
END;
/

-- Ensure the MUNTENIA_ADMIN user exists and has the DBA role
BEGIN
  EXECUTE IMMEDIATE 'ALTER SESSION SET CONTAINER = ESHOP_MUNTENIA';

  -- Check if the MUNTENIA_ADMIN user exists
  DECLARE
    user_exists NUMBER := 0;
  BEGIN
    SELECT COUNT(*) INTO user_exists FROM DBA_USERS WHERE USERNAME = 'MUNTENIA_ADMIN';

    IF user_exists = 0 THEN
      -- Create the MUNTENIA_ADMIN user
      EXECUTE IMMEDIATE 'CREATE USER muntenia_admin IDENTIFIED BY "MunteniaAdminPassword123!"';
      DBMS_OUTPUT.PUT_LINE('MUNTENIA_ADMIN user was successfully created.');

      -- Grant the DBA role to MUNTENIA_ADMIN
      EXECUTE IMMEDIATE 'GRANT DBA TO muntenia_admin';
      DBMS_OUTPUT.PUT_LINE('MUNTENIA_ADMIN user was granted the DBA role.');
    ELSE
      DBMS_OUTPUT.PUT_LINE('MUNTENIA_ADMIN user already exists.');
    END IF;
  END;
END;
/