SET SERVEROUTPUT ON;

-- Switch to the root container
ALTER SESSION SET CONTAINER = CDB$ROOT;


-- Check if PDB eshop_oltp exists
DECLARE
    pdb_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*)
    INTO pdb_exists
    FROM dba_pdbs
    WHERE pdb_name = 'ESHOP_OLTP';

    IF pdb_exists = 0 THEN
        -- Create PDB for OLTP
        EXECUTE IMMEDIATE '
            CREATE PLUGGABLE DATABASE eshop_oltp 
            ADMIN USER eshop_oltp_admin IDENTIFIED BY oltp_password 
            ROLES = (DBA) 
            FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed/'', ''/opt/oracle/oradata/ORCLCDB/eshop_oltp/'')
        ';
        EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE eshop_oltp OPEN';
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB eshop_oltp was successfully created and opened.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB eshop_oltp already exists.');
    END IF;
END;
/

-- Check if PDB eshop_dw exists
DECLARE
    pdb_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*)
    INTO pdb_exists
    FROM dba_pdbs
    WHERE pdb_name = 'ESHOP_DW';

    IF pdb_exists = 0 THEN
        -- Create PDB for Data Warehouse
        EXECUTE IMMEDIATE '
            CREATE PLUGGABLE DATABASE eshop_dw 
            ADMIN USER eshop_dw_admin IDENTIFIED BY dw_password 
            ROLES = (DBA) 
            FILE_NAME_CONVERT = (''/opt/oracle/oradata/ORCLCDB/pdbseed/'', ''/opt/oracle/oradata/ORCLCDB/eshop_dw/'') 
        ';
        EXECUTE IMMEDIATE 'ALTER PLUGGABLE DATABASE eshop_dw OPEN';
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB eshop_dw was successfully created and opened.');
    ELSE
        DBMS_OUTPUT.PUT_LINE('create_pdbs: PDB eshop_dw already exists.');
    END IF;
END;
/
