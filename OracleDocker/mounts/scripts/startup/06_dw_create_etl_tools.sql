SET SERVEROUTPUT ON;

-- Switch to DW PDB
ALTER SESSION SET CONTAINER = eshop_dw;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_DW_USER;


BEGIN
    -- Check and create DWH_ETL_RUNS table
    TRY_CREATE_TABLE('DWH_ETL_RUNS', ' 
        id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY, 
        
        target_table_name VARCHAR2(100) NOT NULL, -- The name of the target table

        source_data_from DATE NOT NULL, -- The date from which the data is being loaded
        source_data_to DATE NOT NULL,   -- The date to which the data is being loaded

        started_on DATE NOT NULL,       -- The date when the ETL run started
        ended_on DATE DEFAULT SYSDATE   -- The date when the ETL run ended
    ', 'dw_create_etl_tools');
  
    LOG_INFORMATION('Finished creating ETL runs table.', 'dw_create_etl_tools');
END;
/
