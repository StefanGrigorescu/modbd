SET SERVEROUTPUT ON;

-- Switch to the GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

-- Set the schema to the GLOBAL user
ALTER SESSION SET CURRENT_SCHEMA = eshop_global_user;

-- Create a view for SLS_PRODUCTS
CREATE OR REPLACE VIEW vw_sls_products (
  id, name, description, price_in_eur
) AS
SELECT id, name, description, price_in_eur
  FROM SLS_PRODUCTS@eshop_romania_link
UNION
SELECT id, name, description, price_in_eur
  FROM SLS_PRODUCTS@eshop_muntenia_link;

-- Log the creation of the view
BEGIN
    LOG_INFORMATION('vw_sls_products: View created successfully.');
END;
/

-- Create a trigger on the view to synchronize data between Romania and Muntenia
CREATE OR REPLACE TRIGGER trg_sync_vw_sls_products
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_products
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_PRODUCTS@eshop_romania_link
      (id, name, description, price_in_eur, created_on)
      VALUES (:1, :2, :3, :4, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.id, :NEW.name, :NEW.description, :NEW.price_in_eur;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'romania', 'muntenia')
      USING :NEW.id, :NEW.name, :NEW.description, :NEW.price_in_eur;

  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_PRODUCTS@eshop_romania_link
      SET name = :1, description = :2, price_in_eur = :3
      WHERE id = :4
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.name, :NEW.description, :NEW.price_in_eur, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'romania', 'muntenia')
      USING :NEW.name, :NEW.description, :NEW.price_in_eur, :NEW.id;

  ELSE  -- DELETING
    v_sql := q'[
      DELETE FROM SLS_PRODUCTS@eshop_romania_link WHERE id = :1
    ]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'romania', 'muntenia')
      USING :OLD.id;
  END IF;
EXCEPTION
  WHEN OTHERS THEN
    LOG_ERROR(
      'trg_sync_vw_sls_products failed: action='
      || CASE
           WHEN INSERTING THEN 'INSERT'
           WHEN UPDATING THEN 'UPDATE'
           ELSE 'DELETE'
         END
      || ', err=' || SQLERRM
    );
    RAISE;  -- propagate so caller sees failure
END;
/

-----------------------------------------------
-------------- TESTING ------------------------
-----------------------------------------------

-- Test: Insert into the view
BEGIN
    LOG_INFORMATION('Testing INSERT into vw_sls_products...');
    INSERT INTO ESHOP_GLOBAL_USER.VW_SLS_PRODUCTS (id, name, description, price_in_eur)
    VALUES (999, 'Test Product', 'This is a test product', 99.99);

    COMMIT;
    LOG_INFORMATION('Insert completed.');
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('Error during INSERT test: ' || SQLERRM);
        ROLLBACK;
        RAISE;
END;
/

-- Verify: Check if the record exists in Romania
DECLARE
    v_count NUMBER;
BEGIN
    LOG_INFORMATION('Verifying record in SLS_PRODUCTS@eshop_romania_link...');
    SELECT COUNT(*) INTO v_count
      FROM SLS_PRODUCTS@ESHOP_ROMANIA_LINK
     WHERE id = 999;

    IF v_count != 1 THEN
        RAISE_APPLICATION_ERROR(-20001,
            'Expected 1 row in Romania, but found ' || v_count);
    END IF;

    LOG_INFORMATION('Romania verification passed.');
END;
/

-- Verify: Check if the record exists in Muntenia
DECLARE
    v_count NUMBER;
BEGIN
    LOG_INFORMATION('Verifying record in SLS_PRODUCTS@eshop_muntenia_link...');
    SELECT COUNT(*) INTO v_count
      FROM SLS_PRODUCTS@ESHOP_MUNTENIA_LINK
     WHERE id = 999;

    IF v_count != 1 THEN
        RAISE_APPLICATION_ERROR(-20002,
            'Expected 1 row in Muntenia, but found ' || v_count);
    END IF;

    LOG_INFORMATION('Muntenia verification passed.');
END;
/

-- Cleanup: Delete the test record
BEGIN
    LOG_INFORMATION('Cleaning up test record via view...');
    DELETE FROM ESHOP_GLOBAL_USER.VW_SLS_PRODUCTS WHERE id = 999;
    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20003,
            'No row deleted from view for ID = 999');
    END IF;

    COMMIT;
    LOG_INFORMATION('Cleanup completed.');
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('Error during CLEANUP: ' || SQLERRM);
        ROLLBACK;
        RAISE;
END;
/

-- Verify: Check if the record was deleted from Romania
DECLARE
    v_count NUMBER;
BEGIN
    LOG_INFORMATION('Verifying deletion from SLS_PRODUCTS@eshop_romania_link...');
    SELECT COUNT(*) INTO v_count
      FROM SLS_PRODUCTS@ESHOP_ROMANIA_LINK
     WHERE id = 999;

    IF v_count != 0 THEN
        RAISE_APPLICATION_ERROR(-20004,
            'Expected 0 rows in Romania after delete, but found ' || v_count);
    END IF;

    LOG_INFORMATION('Deletion verified in Romania.');
END;
/

-- Verify: Check if the record was deleted from Muntenia
DECLARE
    v_count NUMBER;
BEGIN
    LOG_INFORMATION('Verifying deletion from SLS_PRODUCTS@eshop_muntenia_link...');
    SELECT COUNT(*) INTO v_count
      FROM SLS_PRODUCTS@ESHOP_MUNTENIA_LINK
     WHERE id = 999;

    IF v_count != 0 THEN
        RAISE_APPLICATION_ERROR(-20005,
            'Expected 0 rows in Muntenia after delete, but found ' || v_count);
    END IF;

    LOG_INFORMATION('Deletion verified in Muntenia.');
END;
/

-- Final message
BEGIN
    LOG_INFORMATION('All tests completed successfully.');
END;
/