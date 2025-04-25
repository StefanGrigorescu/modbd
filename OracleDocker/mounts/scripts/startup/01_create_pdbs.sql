SET SERVEROUTPUT ON;

-- Switch to the root container
ALTER SESSION SET CONTAINER = CDB$ROOT;

WHENEVER SQLERROR EXIT SQL.SQLCODE;

-- Create PDBs
DECLARE
    pdb_exists NUMBER := 0;
BEGIN
    -- Create ESHOP_OLTP PDB
    SELECT COUNT(*) INTO pdb_exists FROM V$PDBS WHERE NAME = 'ESHOP_OLTP';
    IF pdb_exists = 0 THEN
        EXECUTE IMMEDIATE 'CREATE PLUGGABLE DATABASE ESHOP_OLTP ADMIN USER eshop_oltp_admin IDENTIFIED BY "oltp_password" ROLES = (DBA) FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed'', ''/opt/oracle/oradata/ORCLCDB/ESHOP_OLTP'')';
        EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE ESHOP_OLTP OPEN';
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_OLTP was successfully created and opened.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_OLTP already exists.');
    END IF;

    -- Create ESHOP_DW PDB
    SELECT COUNT(*) INTO pdb_exists FROM V$PDBS WHERE NAME = 'ESHOP_DW';
    IF pdb_exists = 0 THEN
        EXECUTE IMMEDIATE 'CREATE PLUGGABLE DATABASE ESHOP_DW ADMIN USER eshop_dw_admin IDENTIFIED BY "dw_password" ROLES = (DBA) FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed'', ''/opt/oracle/oradata/ORCLCDB/ESHOP_DW'')';
        EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE ESHOP_DW OPEN';
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_DW was successfully created and opened.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_DW already exists.');
    END IF;

    -- Create ESHOP_GLOBAL PDB
    SELECT COUNT(*) INTO pdb_exists FROM V$PDBS WHERE NAME = 'ESHOP_GLOBAL';
    IF pdb_exists = 0 THEN
        EXECUTE IMMEDIATE 'CREATE PLUGGABLE DATABASE ESHOP_GLOBAL ADMIN USER global_admin IDENTIFIED BY "GlobalAdminPassword123!" FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed'', ''/opt/oracle/oradata/GLOBALCDB/ESHOP_GLOBAL'')';
        EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE ESHOP_GLOBAL OPEN';
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_GLOBAL was successfully created and opened.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_GLOBAL already exists.');
    END IF;

    -- Create ESHOP_MUNTENIA PDB
    SELECT COUNT(*) INTO pdb_exists FROM V$PDBS WHERE NAME = 'ESHOP_MUNTENIA';
    IF pdb_exists = 0 THEN
        EXECUTE IMMEDIATE 'CREATE PLUGGABLE DATABASE ESHOP_MUNTENIA ADMIN USER muntenia_admin IDENTIFIED BY "MunteniaAdminPassword123!" FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed'', ''/opt/oracle/oradata/MUNTENIACDB/ESHOP_MUNTENIA'')';
        EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE ESHOP_MUNTENIA OPEN';
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_MUNTENIA was successfully created and opened.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_MUNTENIA already exists.');
    END IF;

    -- Create ESHOP_ROMANIA PDB
    SELECT COUNT(*) INTO pdb_exists FROM V$PDBS WHERE NAME = 'ESHOP_ROMANIA';
    IF pdb_exists = 0 THEN
        EXECUTE IMMEDIATE 'CREATE PLUGGABLE DATABASE ESHOP_ROMANIA ADMIN USER romania_admin IDENTIFIED BY "RomaniaAdminPassword123!" FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed'', ''/opt/oracle/oradata/ROMANIACDB/ESHOP_ROMANIA'')';
        EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE ESHOP_ROMANIA OPEN';
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_ROMANIA was successfully created and opened.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB ESHOP_ROMANIA already exists.');
    END IF;
END;
/
