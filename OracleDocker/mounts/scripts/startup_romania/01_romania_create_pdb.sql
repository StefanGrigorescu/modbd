WHENEVER SQLERROR EXIT SQL.SQLCODE;

-- Check if the ESHOP_ROMANIA PDB already exists
DECLARE
  pdb_exists NUMBER := 0;
BEGIN
  SELECT COUNT(*) INTO pdb_exists FROM V$PDBS WHERE NAME = 'ESHOP_ROMANIA';

  IF pdb_exists = 0 THEN
    -- Create the ESHOP_ROMANIA PDB
    EXECUTE IMMEDIATE 'CREATE PLUGGABLE DATABASE ESHOP_ROMANIA ADMIN USER romania_admin IDENTIFIED BY "RomaniaAdminPassword123!" FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed'', ''/opt/oracle/oradata/ROMANIACDB/ESHOP_ROMANIA'')';
    EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE ESHOP_ROMANIA OPEN';
    DBMS_OUTPUT.PUT_LINE('ESHOP_ROMANIA PDB was successfully created and opened.');
  ELSE
    DBMS_OUTPUT.PUT_LINE('ESHOP_ROMANIA PDB already exists.');
  END IF;
END;
/

-- Ensure the ROMANIA_ADMIN user exists and has the DBA role
BEGIN
  EXECUTE IMMEDIATE 'ALTER SESSION SET CONTAINER = ESHOP_ROMANIA';

  -- Check if the ROMANIA_ADMIN user exists
  DECLARE
    user_exists NUMBER := 0;
  BEGIN
    SELECT COUNT(*) INTO user_exists FROM DBA_USERS WHERE USERNAME = 'ROMANIA_ADMIN';

    IF user_exists = 0 THEN
      -- Create the ROMANIA_ADMIN user
      EXECUTE IMMEDIATE 'CREATE USER romania_admin IDENTIFIED BY "RomaniaAdminPassword123!"';
      DBMS_OUTPUT.PUT_LINE('ROMANIA_ADMIN user was successfully created.');

      -- Grant the DBA role to ROMANIA_ADMIN
      EXECUTE IMMEDIATE 'GRANT DBA TO romania_admin';
      DBMS_OUTPUT.PUT_LINE('ROMANIA_ADMIN user was granted the DBA role.');
    ELSE
      DBMS_OUTPUT.PUT_LINE('ROMANIA_ADMIN user already exists.');
    END IF;
  END;
END;
/