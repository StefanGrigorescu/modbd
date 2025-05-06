SET SERVEROUTPUT ON;

-- Switch to ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_ROMANIA_USER;

-- Insert users
BEGIN
    INSERT INTO IDNT_USERS (username, first_name, last_name, date_of_birth, phone_number) 
    VALUES ('IonPopescu', 'Ion', 'Popescu', TO_DATE('1980-01-01', 'YYYY-MM-DD'), '0700000001');
    INSERT INTO IDNT_USERS (username, first_name, last_name, date_of_birth, phone_number) 
    VALUES ('MariaIonescu', 'Maria', 'Ionescu', TO_DATE('1985-02-02', 'YYYY-MM-DD'), '0700000002');
    INSERT INTO IDNT_USERS (username, first_name, last_name, date_of_birth, phone_number) 
    VALUES ('VasilePopa', 'Vasile', 'Popa', TO_DATE('1990-03-03', 'YYYY-MM-DD'), '0700000003');
    COMMIT;
END;
/

-- Insert order statuses
BEGIN
    INSERT INTO SLS_ORDER_STATUSES (id, name) VALUES (0, 'Pending');
    INSERT INTO SLS_ORDER_STATUSES (id, name) VALUES (1, 'Completed');
    INSERT INTO SLS_ORDER_STATUSES (id, name) VALUES (2, 'Canceled');
    COMMIT;
END;
/

-- Insert orders
BEGIN
    INSERT INTO SLS_ORDERS (id, customer_id, customer_region_id, address, status_id) 
    VALUES (1, 1, 0, 'Strada Principala, Bucuresti', 0);
    INSERT INTO SLS_ORDERS (id, customer_id, customer_region_id, address, status_id) 
    VALUES (2, 2, 1, 'Strada Secundara, Cluj-Napoca', 1);
    INSERT INTO SLS_ORDERS (id, customer_id, customer_region_id, address, status_id) 
    VALUES (3, 3, 2, 'Strada Tertiar, Iasi', 2);
    COMMIT;
END;
/

-- Insert order items
BEGIN
    INSERT INTO SLS_ORDER_ITEMS (order_id, product_id, quantity) VALUES (1, 1, 2);
    INSERT INTO SLS_ORDER_ITEMS (order_id, product_id, quantity) VALUES (2, 2, 1);
    INSERT INTO SLS_ORDER_ITEMS (order_id, product_id, quantity) VALUES (3, 3, 5);
    COMMIT;
END;
/

-- Insert invoice statuses
BEGIN
    INSERT INTO BLG_INVOICE_STATUSES (id, name) VALUES (0, 'Pending');
    INSERT INTO BLG_INVOICE_STATUSES (id, name) VALUES (1, 'Paid');
    INSERT INTO BLG_INVOICE_STATUSES (id, name) VALUES (2, 'Overdue');
    INSERT INTO BLG_INVOICE_STATUSES (id, name) VALUES (3, 'Canceled');
    COMMIT;
END;
/

-- Insert invoices
BEGIN
    INSERT INTO BLG_INVOICES (id, customer_id, customer_region_id, total_discount_in_eur, status_id) 
    VALUES (1, 1, 0, 10, 1);
    INSERT INTO BLG_INVOICES (id, customer_id, customer_region_id, total_discount_in_eur, status_id) 
    VALUES (2, 2, 1, 5, 0);
    INSERT INTO BLG_INVOICES (id, customer_id, customer_region_id, total_discount_in_eur, status_id) 
    VALUES (3, 3, 2, 0, 2);
    COMMIT;
END;
/

-- Insert invoice items
BEGIN
    INSERT INTO BLG_INVOICE_ITEMS (invoice_id, product_id, product_name, product_description, product_price_in_eur, 
                                   products_applied_discount_in_eur, products_price_after_discount_in_eur, quantity) 
    VALUES (1, 1, 'Football Ball', 'Standard size football ball', 25, 5, 20, 2);
    INSERT INTO BLG_INVOICE_ITEMS (invoice_id, product_id, product_name, product_description, product_price_in_eur, 
                                   products_applied_discount_in_eur, products_price_after_discount_in_eur, quantity) 
    VALUES (2, 2, 'Basketball Ball', 'Standard size basketball ball', 30, 5, 25, 1);
    INSERT INTO BLG_INVOICE_ITEMS (invoice_id, product_id, product_name, product_description, product_price_in_eur, 
                                   products_applied_discount_in_eur, products_price_after_discount_in_eur, quantity) 
    VALUES (3, 3, 'Tennis Racket', 'Professional tennis racket', 75, 0, 75, 5);
    COMMIT;
END;
/

-- Insert invoice bundles
BEGIN
    INSERT INTO BLG_INVOICE_BUNDLES (invoice_id, bundle_id, bundle_price_in_eur, bundles_applied_discount_in_eur, 
                                     bundles_price_after_discount_in_eur, quantity) 
    VALUES (1, 1, 100, 10, 90, 1);
    INSERT INTO BLG_INVOICE_BUNDLES (invoice_id, bundle_id, bundle_price_in_eur, bundles_applied_discount_in_eur, 
                                     bundles_price_after_discount_in_eur, quantity) 
    VALUES (2, 2, 200, 20, 180, 2);
    INSERT INTO BLG_INVOICE_BUNDLES (invoice_id, bundle_id, bundle_price_in_eur, bundles_applied_discount_in_eur, 
                                     bundles_price_after_discount_in_eur, quantity) 
    VALUES (3, 3, 300, 30, 270, 3);
    COMMIT;
END;
/