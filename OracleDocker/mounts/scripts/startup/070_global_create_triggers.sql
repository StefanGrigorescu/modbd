SET SERVEROUTPUT ON;

ALTER SESSION SET CONTAINER = eshop_global;

ALTER SESSION SET CURRENT_SCHEMA = eshop_global_user;

CREATE OR REPLACE VIEW vw_sls_products (
  id, name, description, price_in_eur, created_on, last_updated_on
) AS
SELECT id, name, description, price_in_eur, created_on, last_updated_on
  FROM SLS_PRODUCTS@eshop_romania_link
UNION
SELECT id, name, description, price_in_eur, created_on, last_updated_on
  FROM SLS_PRODUCTS@eshop_muntenia_link;

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
      VALUES (:1, :2, :3, :4, sysdate)
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.id, :NEW.name, :NEW.description, :NEW.price_in_eur;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.id, :NEW.name, :NEW.description, :NEW.price_in_eur;

  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_PRODUCTS@eshop_romania_link
      SET name = :1, description = :2, price_in_eur = :3
      WHERE id = :4
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.name, :NEW.description, :NEW.price_in_eur, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.name, :NEW.description, :NEW.price_in_eur, :NEW.id;

  ELSE  -- DELETING
    v_sql := q'[
      DELETE FROM SLS_PRODUCTS@eshop_romania_link WHERE id = :1
    ]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
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

-- Create view for SLS_PRODUCT_CATEGORIES
CREATE OR REPLACE VIEW vw_sls_product_categories (
  id, name, created_on, last_updated_on
) AS
SELECT id, name, created_on, last_updated_on
  FROM SLS_PRODUCT_CATEGORIES@eshop_romania_link
UNION
SELECT id, name, created_on, last_updated_on
  FROM SLS_PRODUCT_CATEGORIES@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_product_categories
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_product_categories
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_PRODUCT_CATEGORIES@eshop_romania_link
      (id, name, created_on)
      VALUES (:1, :2, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.id, :NEW.name;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.id, :NEW.name;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_PRODUCT_CATEGORIES@eshop_romania_link
      SET name = :1
      WHERE id = :2
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.name, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.name, :NEW.id;
  ELSE
    v_sql := q'[DELETE FROM SLS_PRODUCT_CATEGORIES@eshop_romania_link WHERE id = :1]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :OLD.id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_sls_product_categories failed: ' || SQLERRM);
  RAISE;
END;
/

-- Create view for SLS_PRODUCT_SUBCATEGORIES
CREATE OR REPLACE VIEW vw_sls_product_subcategories (
  id, category_id, name, created_on, last_updated_on
) AS
SELECT id, category_id, name, created_on, last_updated_on
  FROM SLS_PRODUCT_SUBCATEGORIES@eshop_romania_link
UNION
SELECT id, category_id, name, created_on, last_updated_on
  FROM SLS_PRODUCT_SUBCATEGORIES@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_product_subcategories
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_product_subcategories
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_PRODUCT_SUBCATEGORIES@eshop_romania_link
      (id, category_id, name, created_on)
      VALUES (:1, :2, :3, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.id, :NEW.category_id, :NEW.name;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.id, :NEW.category_id, :NEW.name;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_PRODUCT_SUBCATEGORIES@eshop_romania_link
      SET category_id = :1, name = :2
      WHERE id = :3
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.category_id, :NEW.name, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.category_id, :NEW.name, :NEW.id;
  ELSE
    v_sql := q'[DELETE FROM SLS_PRODUCT_SUBCATEGORIES@eshop_romania_link WHERE id = :1]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :OLD.id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_sls_product_subcategories failed: ' || SQLERRM);
  RAISE;
END;
/

-- Create view for SLS_PRODUCT_PRODUCT_SUBCATEGORIES
CREATE OR REPLACE VIEW vw_sls_product_product_subcategories (
  product_id, subcategory_id, created_on
) AS
SELECT product_id, subcategory_id, created_on
  FROM SLS_PRODUCT_PRODUCT_SUBCATEGORIES@eshop_romania_link
UNION
SELECT product_id, subcategory_id, created_on
  FROM SLS_PRODUCT_PRODUCT_SUBCATEGORIES@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_product_product_subcategories
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_product_product_subcategories
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_PRODUCT_PRODUCT_SUBCATEGORIES@eshop_romania_link
      (product_id, subcategory_id, created_on)
      VALUES (:1, :2, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.product_id, :NEW.subcategory_id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.product_id, :NEW.subcategory_id;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_PRODUCT_PRODUCT_SUBCATEGORIES@eshop_romania_link
      SET subcategory_id = :1
      WHERE product_id = :2
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.subcategory_id, :NEW.product_id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.subcategory_id, :NEW.product_id;
  ELSE
    v_sql := q'[DELETE FROM SLS_PRODUCT_PRODUCT_SUBCATEGORIES@eshop_romania_link 
               WHERE product_id = :1 AND subcategory_id = :2]';
    EXECUTE IMMEDIATE v_sql USING :OLD.product_id, :OLD.subcategory_id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') 
      USING :OLD.product_id, :OLD.subcategory_id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_sls_product_product_subcategories failed: ' || SQLERRM);
  RAISE;
END;
/

-- Create view for SLS_PRODUCT_TAGS
CREATE OR REPLACE VIEW vw_sls_product_tags (
  id, name, created_on, last_updated_on
) AS
SELECT id, name, created_on, last_updated_on
  FROM SLS_PRODUCT_TAGS@eshop_romania_link
UNION
SELECT id, name, created_on, last_updated_on
  FROM SLS_PRODUCT_TAGS@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_product_tags
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_product_tags
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_PRODUCT_TAGS@eshop_romania_link
      (id, name, created_on)
      VALUES (:1, :2, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.id, :NEW.name;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.id, :NEW.name;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_PRODUCT_TAGS@eshop_romania_link
      SET name = :1
      WHERE id = :2
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.name, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.name, :NEW.id;
  ELSE
    v_sql := q'[DELETE FROM SLS_PRODUCT_TAGS@eshop_romania_link WHERE id = :1]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :OLD.id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_sls_product_tags failed: ' || SQLERRM);
  RAISE;
END;
/

-- Create view for SLS_PRODUCT_PRODUCT_TAGS
CREATE OR REPLACE VIEW vw_sls_product_product_tags (
  product_id, tag_id, created_on
) AS
SELECT product_id, tag_id, created_on
  FROM SLS_PRODUCT_PRODUCT_TAGS@eshop_romania_link
UNION
SELECT product_id, tag_id, created_on
  FROM SLS_PRODUCT_PRODUCT_TAGS@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_product_product_tags
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_product_product_tags
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_PRODUCT_PRODUCT_TAGS@eshop_romania_link
      (product_id, tag_id, created_on)
      VALUES (:1, :2, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.product_id, :NEW.tag_id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') 
      USING :NEW.product_id, :NEW.tag_id;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_PRODUCT_PRODUCT_TAGS@eshop_romania_link
      SET tag_id = :1
      WHERE product_id = :2
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.tag_id, :NEW.product_id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') 
      USING :NEW.tag_id, :NEW.product_id;
  ELSE
    v_sql := q'[DELETE FROM SLS_PRODUCT_PRODUCT_TAGS@eshop_romania_link 
               WHERE product_id = :1 AND tag_id = :2]';
    EXECUTE IMMEDIATE v_sql USING :OLD.product_id, :OLD.tag_id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') 
      USING :OLD.product_id, :OLD.tag_id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_sls_product_product_tags failed: ' || SQLERRM);
  RAISE;
END;
/

-- Create view for SLS_ORDER_STATUSES
CREATE OR REPLACE VIEW vw_sls_order_statuses (
  id, name, created_on, last_updated_on
) AS
SELECT id, name, created_on, last_updated_on
  FROM SLS_ORDER_STATUSES@eshop_romania_link
UNION
SELECT id, name, created_on, last_updated_on
  FROM SLS_ORDER_STATUSES@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_order_statuses
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_order_statuses
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_ORDER_STATUSES@eshop_romania_link
      (id, name, created_on)
      VALUES (:1, :2, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.id, :NEW.name;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.id, :NEW.name;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_ORDER_STATUSES@eshop_romania_link
      SET name = :1
      WHERE id = :2
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.name, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.name, :NEW.id;
  ELSE
    v_sql := q'[DELETE FROM SLS_ORDER_STATUSES@eshop_romania_link WHERE id = :1]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :OLD.id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_sls_order_statuses failed: ' || SQLERRM);
  RAISE;
END;
/

-- Create view for SLS_DISCOUNT_TYPES
CREATE OR REPLACE VIEW vw_sls_discount_types (
  id, name, created_on, last_updated_on
) AS
SELECT id, name, created_on, last_updated_on
  FROM SLS_DISCOUNT_TYPES@eshop_romania_link
UNION
SELECT id, name, created_on, last_updated_on
  FROM SLS_DISCOUNT_TYPES@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_discount_types
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_discount_types
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_DISCOUNT_TYPES@eshop_romania_link
      (id, name, created_on)
      VALUES (:1, :2, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.id, :NEW.name;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.id, :NEW.name;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_DISCOUNT_TYPES@eshop_romania_link
      SET name = :1
      WHERE id = :2
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.name, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.name, :NEW.id;
  ELSE
    v_sql := q'[DELETE FROM SLS_DISCOUNT_TYPES@eshop_romania_link WHERE id = :1]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :OLD.id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_sls_discount_types failed: ' || SQLERRM);
  RAISE;
END;
/

-- Create view for SLS_DISCOUNT_REASONS
CREATE OR REPLACE VIEW vw_sls_discount_reasons (
  id, name, created_on, last_updated_on
) AS
SELECT id, name, created_on, last_updated_on
  FROM SLS_DISCOUNT_REASONS@eshop_romania_link
UNION
SELECT id, name, created_on, last_updated_on
  FROM SLS_DISCOUNT_REASONS@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_discount_reasons
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_discount_reasons
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_DISCOUNT_REASONS@eshop_romania_link
      (id, name, created_on)
      VALUES (:1, :2, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.id, :NEW.name;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.id, :NEW.name;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_DISCOUNT_REASONS@eshop_romania_link
      SET name = :1
      WHERE id = :2
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.name, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.name, :NEW.id;
  ELSE
    v_sql := q'[DELETE FROM SLS_DISCOUNT_REASONS@eshop_romania_link WHERE id = :1]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :OLD.id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_sls_discount_reasons failed: ' || SQLERRM);
  RAISE;
END;
/

-- Create view for BLG_INVOICE_STATUSES
CREATE OR REPLACE VIEW vw_blg_invoice_statuses (
  id, name, created_on, last_updated_on
) AS
SELECT id, name, created_on, last_updated_on
  FROM BLG_INVOICE_STATUSES@eshop_romania_link
UNION
SELECT id, name, created_on, last_updated_on
  FROM BLG_INVOICE_STATUSES@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_blg_invoice_statuses
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_blg_invoice_statuses
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO BLG_INVOICE_STATUSES@eshop_romania_link
      (id, name, created_on)
      VALUES (:1, :2, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.id, :NEW.name;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.id, :NEW.name;
  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE BLG_INVOICE_STATUSES@eshop_romania_link
      SET name = :1
      WHERE id = :2
    ]';
    EXECUTE IMMEDIATE v_sql USING :NEW.name, :NEW.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :NEW.name, :NEW.id;
  ELSE
    v_sql := q'[DELETE FROM BLG_INVOICE_STATUSES@eshop_romania_link WHERE id = :1]';
    EXECUTE IMMEDIATE v_sql USING :OLD.id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link') USING :OLD.id;
  END IF;
EXCEPTION WHEN OTHERS THEN
  LOG_ERROR('trg_sync_vw_blg_invoice_statuses failed: ' || SQLERRM);
  RAISE;
END;
/

-----------------------------------------------
-------------- TESTING ------------------------
-----------------------------------------------

-- Test: Insert into the view
BEGIN
    LOG_INFORMATION('Testing INSERT into vw_sls_products...');
    INSERT INTO ESHOP_GLOBAL_USER.VW_SLS_PRODUCTS (id, name, description, price_in_eur, created_on)
    VALUES (999, 'Test Product', 'This is a test product', 99.99, sysdate);

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

--Create view for IDNT_USERS
CREATE OR REPLACE VIEW vw_idnt_users AS
SELECT 
    g.id, g.email, g.password, g.salt,
    l.username, l.first_name, l.last_name, l.date_of_birth, l.phone_number, l.region_id, l.created_on, l.last_updated_on
FROM IDNT_USERS g
LEFT JOIN (
    SELECT 
        id, username, first_name, last_name, date_of_birth, phone_number, region_id, created_on, last_updated_on
    FROM IDNT_USERS@ESHOP_MUNTENIA_LINK
    UNION ALL
    SELECT 
        id, username, first_name, last_name, date_of_birth, phone_number, region_id, created_on, last_updated_on
    FROM IDNT_USERS@ESHOP_ROMANIA_LINK
) l ON g.id = l.id;

--Trigger that inserts / deletes data from local if changes are made to global.
--Due to vertical sharding, login related info is saved in global and profile related info
--is saved into global
CREATE OR REPLACE TRIGGER trg_sync_vw_idnt_users
INSTEAD OF INSERT OR DELETE ON vw_idnt_users
FOR EACH ROW
DECLARE
    v_region VARCHAR2(50); -- Use region name
    v_generated_id NUMBER; -- Store the generated ID
BEGIN
    -- Determine the region
    v_region := CASE
        WHEN INSERTING THEN :NEW.region_id
        ELSE :OLD.region_id
    END;

    IF INSERTING THEN
        INSERT INTO IDNT_USERS (
            email, password, salt
        ) VALUES (
            :NEW.email, :NEW.password, :NEW.salt
        ) RETURNING id INTO v_generated_id;

        IF v_region = 'MUNTENIA' THEN
            INSERT INTO IDNT_USERS@ESHOP_MUNTENIA_LINK (
                id, username, first_name, last_name, date_of_birth, phone_number, created_on, last_updated_on
            ) VALUES (
                v_generated_id, :NEW.username, :NEW.first_name, :NEW.last_name, :NEW.date_of_birth, :NEW.phone_number, SYSDATE, SYSDATE
            );
        ELSIF v_region = 'ROMANIA' THEN
            INSERT INTO IDNT_USERS@ESHOP_ROMANIA_LINK (
                id, username, first_name, last_name, date_of_birth, phone_number, created_on, last_updated_on
            ) VALUES (
                v_generated_id, :NEW.username, :NEW.first_name, :NEW.last_name, :NEW.date_of_birth, :NEW.phone_number, SYSDATE, SYSDATE
            );
        ELSE
            RAISE_APPLICATION_ERROR(-20001, 'Invalid region. Cannot route data to local databases.');
        END IF;

    ELSE
        DELETE FROM IDNT_USERS
        WHERE id = :OLD.id;

        IF v_region = 'MUNTENIA' THEN
            DELETE FROM IDNT_USERS@ESHOP_MUNTENIA_LINK
            WHERE id = :OLD.id;
        ELSIF v_region = 'ROMANIA' THEN
            DELETE FROM IDNT_USERS@ESHOP_ROMANIA_LINK
            WHERE id = :OLD.id;
        ELSE
            RAISE_APPLICATION_ERROR(-20002, 'Invalid region. Cannot delete data from local databases.');
        END IF;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR(
            'trg_sync_vw_idnt_users failed: action='
            || CASE
                   WHEN INSERTING THEN 'INSERT'
                   ELSE 'DELETE'
               END
            || ', err=' || SQLERRM
        );
        RAISE;
END;
/