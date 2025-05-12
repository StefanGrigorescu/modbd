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

-- Create view for SLS_ORDERS
CREATE OR REPLACE VIEW vw_sls_orders (
  id, customer_id, customer_region_id, address, status_id, created_on, last_updated_on
) AS
SELECT id, customer_id, customer_region_id, address, status_id, created_on, last_updated_on
  FROM SLS_ORDERS@eshop_romania_link
UNION
SELECT id, customer_id, customer_region_id, address, status_id, created_on, last_updated_on
  FROM SLS_ORDERS@eshop_muntenia_link;

CREATE OR REPLACE TRIGGER trg_sync_vw_sls_orders
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_orders
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    IF :NEW.customer_region_id <> 0 THEN
      v_sql := q'[
        INSERT INTO SLS_ORDERS@eshop_romania_link
        (id, customer_id, customer_region_id, address, status_id, created_on)
        VALUES (:1, :2, :3, :4, :5, SYSDATE)
      ]';
      EXECUTE IMMEDIATE v_sql
        USING :NEW.id, :NEW.customer_id, :NEW.customer_region_id, :NEW.address, :NEW.status_id;
    ELSE
      v_sql := q'[
        INSERT INTO SLS_ORDERS@eshop_muntenia_link
        (id, customer_id, customer_region_id, address, status_id, created_on)
        VALUES (:1, :2, :3, :4, :5, SYSDATE)
      ]';
      EXECUTE IMMEDIATE v_sql
        USING :NEW.id, :NEW.customer_id, :NEW.customer_region_id, :NEW.address, :NEW.status_id;
    END IF;

  ELSIF UPDATING THEN
    IF :NEW.customer_region_id <> 0 THEN
      v_sql := q'[
        UPDATE SLS_ORDERS@eshop_romania_link
        SET customer_id = :1, customer_region_id = :2, address = :3, status_id = :4, last_updated_on = SYSDATE
        WHERE id = :5
      ]';
      EXECUTE IMMEDIATE v_sql
        USING :NEW.customer_id, :NEW.customer_region_id, :NEW.address, :NEW.status_id, :NEW.id;
    ELSE
      v_sql := q'[
        UPDATE SLS_ORDERS@eshop_muntenia_link
        SET customer_id = :1, customer_region_id = :2, address = :3, status_id = :4, last_updated_on = SYSDATE
        WHERE id = :5
      ]';
      EXECUTE IMMEDIATE v_sql
        USING :NEW.customer_id, :NEW.customer_region_id, :NEW.address, :NEW.status_id, :NEW.id;
    END IF;

  ELSE  -- DELETING
    IF :OLD.customer_region_id <> 0 THEN
      v_sql := q'[
        DELETE FROM SLS_ORDERS@eshop_romania_link WHERE id = :1
      ]';
      EXECUTE IMMEDIATE v_sql USING :OLD.id;
    ELSE
      v_sql := q'[
        DELETE FROM SLS_ORDERS@eshop_muntenia_link WHERE id = :1
      ]';
      EXECUTE IMMEDIATE v_sql USING :OLD.id;
    END IF;
  END IF;
EXCEPTION
  WHEN OTHERS THEN
    LOG_ERROR(
      'trg_sync_vw_sls_orders failed: action='
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


-- Create view for SLS_ORDER_ITEMS
CREATE OR REPLACE VIEW vw_sls_order_items (
  order_id, product_id, quantity, created_on, last_updated_on
) AS
SELECT order_id, product_id, quantity, created_on, last_updated_on
  FROM SLS_ORDER_ITEMS@eshop_romania_link
UNION
SELECT order_id, product_id, quantity, created_on, last_updated_on
  FROM SLS_ORDER_ITEMS@eshop_muntenia_link;

-- Create trigger for vw_sls_order_items
CREATE OR REPLACE TRIGGER trg_sync_vw_sls_order_items
INSTEAD OF INSERT OR UPDATE OR DELETE ON vw_sls_order_items
FOR EACH ROW
DECLARE
  v_sql VARCHAR2(4000);
BEGIN
  IF INSERTING THEN
    v_sql := q'[
      INSERT INTO SLS_ORDER_ITEMS@eshop_romania_link
      (order_id, product_id, quantity, created_on)
      VALUES (:1, :2, :3, SYSDATE)
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.order_id, :NEW.product_id, :NEW.quantity;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.order_id, :NEW.product_id, :NEW.quantity;

  ELSIF UPDATING THEN
    v_sql := q'[
      UPDATE SLS_ORDER_ITEMS@eshop_romania_link
      SET quantity = :1, last_updated_on = SYSDATE
      WHERE order_id = :2 AND product_id = :3
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :NEW.quantity, :NEW.order_id, :NEW.product_id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :NEW.quantity, :NEW.order_id, :NEW.product_id;

  ELSE  -- DELETING
    v_sql := q'[
      DELETE FROM SLS_ORDER_ITEMS@eshop_romania_link
      WHERE order_id = :1 AND product_id = :2
    ]';
    EXECUTE IMMEDIATE v_sql
      USING :OLD.order_id, :OLD.product_id;
    EXECUTE IMMEDIATE REPLACE(v_sql, 'eshop_romania_link', 'eshop_muntenia_link')
      USING :OLD.order_id, :OLD.product_id;
  END IF;
EXCEPTION
  WHEN OTHERS THEN
    LOG_ERROR(
      'trg_sync_vw_sls_order_items failed: action='
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


CREATE OR REPLACE PROCEDURE PLACE_ORDER (
    p_order_id IN NUMBER,
    p_customer_id IN NUMBER,
    p_address IN NVARCHAR2 DEFAULT 'Bucuresti Sector 2 Straga Pacii numarul 14 Bloc 34 Scara A',
    p_items_csv IN NVARCHAR2,
    is_success OUT NUMBER,
    error_message OUT NVARCHAR2,
    p_created_on IN TIMESTAMP DEFAULT SYSTIMESTAMP
) IS
    customer_region_id NUMBER;
    final_address NVARCHAR2(850);
    order_exists NUMBER := 0;
BEGIN
    -- Check if the order_id already exists
    SELECT COUNT(*) INTO order_exists
    FROM vw_sls_orders
    WHERE id = p_order_id;

    IF order_exists > 0 THEN
        -- If order_id exists, set is_success to false (0)
        is_success := 0;
        error_message := 'Could not place order ' || p_order_id || ' for customer ID ' || p_customer_id || '. Order ID already exists.';
        LOG_DEBUG('Place_Order: Could not place order ' || p_order_id || ' for customer ID ' || p_customer_id || '. Order ID already exists.');
    ELSE
        -- Query the customer's region ID and address if not provided
        BEGIN
                SELECT region_id INTO customer_region_id
                FROM vw_idnt_users
                WHERE id = p_customer_id;
                final_address := p_address;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                is_success := 0;
                error_message := 'No address found for customer ID ' || p_customer_id || '.';
                LOG_WARNING('Place_Order: No address found for customer ID ' || p_customer_id || '.');
                RETURN;
        END;

        -- Insert the order into SLS_Orders
        INSERT INTO vw_sls_orders (
            id, customer_id, customer_region_id, address, status_id, created_on
        ) VALUES (
                                                                        -- Default status ID for "Pending"
            p_order_id, p_customer_id, customer_region_id, final_address, 0, p_created_on
        );

        -- Process the items CSV
        FOR item IN (
            SELECT REGEXP_SUBSTR(p_items_csv, '[^,]+', 1, LEVEL) AS item
            FROM DUAL
            CONNECT BY REGEXP_SUBSTR(p_items_csv, '[^,]+', 1, LEVEL) IS NOT NULL
        ) LOOP
            DECLARE
                product_id NUMBER;
                quantity NUMBER;
            BEGIN
                -- Parse product_id and quantity from the item string
                SELECT TO_NUMBER(REGEXP_SUBSTR(item.item, '^[^x]+')),
                    TO_NUMBER(REGEXP_SUBSTR(item.item, '[^x]+$'))
                INTO product_id, quantity
                FROM DUAL;

                -- Insert the item into SLS_Order_Items
                INSERT INTO vw_sls_order_items (
                    order_id, product_id, quantity
                ) VALUES (
                    p_order_id, product_id, quantity
                );
            EXCEPTION
                WHEN OTHERS THEN
                    ROLLBACK;
                    is_success := 0;
                    error_message := 'Error placing order ' || p_order_id || ' for customer ID ' || p_customer_id || '. Could not add item ' || item.item || ': ' || SQLERRM;
                    LOG_ERROR('PLACE_ORDER: Error placing order ' || p_order_id || ' for customer ID ' || p_customer_id || '. Could not add item ' || item.item || ': ' || SQLERRM);
                    RETURN;
            END;
        END LOOP;
        
        COMMIT;

        -- Set is_success to true (1)
        is_success := 1;
        LOG_INFORMATION('Place_Order: Order ' || p_order_id || ' placed successfully for customer ID ' || p_customer_id || '.');
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        is_success := 0;
        error_message := 'Error placing order ' || p_order_id || ' for customer ID ' || p_customer_id || ': ' || SQLERRM;
        LOG_ERROR('PLACE_ORDER: Error placing order ' || p_order_id || ' for customer ID ' || p_customer_id || ': ' || SQLERRM);
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

-- Test: Update a record in the view
BEGIN
    LOG_INFORMATION('Testing UPDATE on vw_sls_orders...');

    -- Insert a test record into the view
    INSERT INTO ESHOP_GLOBAL_USER.VW_SLS_ORDERS (id, customer_id, customer_region_id, address, status_id, created_on)
    VALUES (1001, 2001, 1, 'Test Address Romania', 1, SYSDATE);

    COMMIT;

    -- Update the test record in the view
    UPDATE ESHOP_GLOBAL_USER.VW_SLS_ORDERS
    SET address = 'Updated Address Romania', status_id = 2
    WHERE id = 1001;

    COMMIT;

    -- Verify: Check if the record was updated in Romania
    DECLARE
        v_count NUMBER;
    BEGIN
        LOG_INFORMATION('Verifying update in SLS_ORDERS@eshop_romania_link...');
        SELECT COUNT(*) INTO v_count
        FROM SLS_ORDERS@ESHOP_ROMANIA_LINK
        WHERE id = 1001 AND address = 'Updated Address Romania' AND status_id = 2;

        IF v_count != 1 THEN
            RAISE_APPLICATION_ERROR(-20001,
                'Expected 1 updated row in Romania, but found ' || v_count);
        END IF;

        LOG_INFORMATION('Update verification in Romania passed.');
    END;

    -- Cleanup: Delete the test record
    DELETE FROM ESHOP_GLOBAL_USER.VW_SLS_ORDERS WHERE id = 1001;

    COMMIT;
    LOG_INFORMATION('Cleanup completed.');
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('Error during UPDATE test: ' || SQLERRM);
        ROLLBACK;
        RAISE;
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

-- Update trigger for IDNT_USERS
CREATE OR REPLACE TRIGGER trg_update_vw_idnt_users
INSTEAD OF UPDATE ON vw_idnt_users
FOR EACH ROW
DECLARE
  v_region_id NUMBER;
  v_sql VARCHAR2(4000);
BEGIN
  UPDATE IDNT_USERS
  SET email = :NEW.email,
      password = :NEW.password,
      salt = :NEW.salt
  WHERE id = :NEW.id;

  BEGIN
    SELECT region_id
    INTO v_region_id
    FROM IDNT_USERS@ESHOP_MUNTENIA_LINK
    WHERE id = :NEW.id;

    v_region_id := 0;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      BEGIN
        SELECT region_id
        INTO v_region_id
        FROM IDNT_USERS@ESHOP_ROMANIA_LINK
        WHERE id = :NEW.id;
      EXCEPTION
        WHEN NO_DATA_FOUND THEN
          RAISE_APPLICATION_ERROR(-20001, 'Record not found in local tables for ID: ' || :NEW.id);
      END;
  END;

  IF v_region_id = 0 THEN
    v_sql := q'[
      UPDATE IDNT_USERS@ESHOP_MUNTENIA_LINK
      SET username = :1, first_name = :2, last_name = :3, date_of_birth = :4, phone_number = :5
      WHERE id = :6
    ]';
  ELSE
    v_sql := q'[
      UPDATE IDNT_USERS@ESHOP_ROMANIA_LINK
      SET username = :1, first_name = :2, last_name = :3, date_of_birth = :4, phone_number = :5
      WHERE id = :6
    ]';
  END IF;

  EXECUTE IMMEDIATE v_sql
    USING :NEW.username, :NEW.first_name, :NEW.last_name, :NEW.date_of_birth, :NEW.phone_number, :NEW.id;
END;
/

-- Delete trigger for IDNT_USERS
CREATE OR REPLACE TRIGGER trg_delete_idnt_users
BEFORE DELETE ON IDNT_USERS
FOR EACH ROW
BEGIN
  RAISE_APPLICATION_ERROR(-20002, 'Deleting users is not allowed.');
END;
/
