SET SERVEROUTPUT ON;

-- Switch to ROMANIA PDB
ALTER SESSION SET CONTAINER = eshop_romania;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_ROMANIA_USER;

-- Create procedure to insert an order status
CREATE OR REPLACE PROCEDURE INSERT_ORDER_STATUS (
    p_status_id IN NUMBER,
    p_status_name IN VARCHAR2
) IS
    status_exists_by_id NUMBER := 0;
    status_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the order status ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_status_id THEN 1 END), COUNT(CASE WHEN UPPER(name) = UPPER(p_status_name) THEN 1 END) 
        INTO status_exists_by_id, status_exists_by_name
    FROM SLS_ORDER_STATUSES;

    -- Ensure both ID and name are unique
    IF status_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_ORDER_STATUS: Status ID ' || p_status_id || ' already exists.');
    ELSIF status_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_ORDER_STATUS: Status name ' || p_status_name || ' already exists.');
    ELSE
        -- Insert the order status if both ID and name are unique
        INSERT INTO SLS_ORDER_STATUSES (id, name)
        VALUES (p_status_id, p_status_name);
        LOG_INFORMATION('INSERT_ORDER_STATUS: Status ' || p_status_name || ' with ID ' || p_status_id || ' created.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_ORDER_STATUS: Error creating status ' || p_status_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

BEGIN
    INSERT_ORDER_STATUS(0, 'Pending');
    INSERT_ORDER_STATUS(1, 'Completed');
    INSERT_ORDER_STATUS(2, 'Canceled');
END;
/

-- Create procedure to insert an invoice status
CREATE OR REPLACE PROCEDURE INSERT_INVOICE_STATUS (
    p_status_id IN NUMBER,
    p_status_name IN VARCHAR2
) IS
    status_exists_by_id NUMBER := 0;
    status_exists_by_name NUMBER := 0;
BEGIN
    -- Check if the invoice status ID or name already exists
    SELECT 
        COUNT(CASE WHEN id = p_status_id THEN 1 END), COUNT(CASE WHEN UPPER(name) = UPPER(p_status_name) THEN 1 END) 
        INTO status_exists_by_id, status_exists_by_name
    FROM BLG_INVOICE_STATUSES;

    -- Ensure both ID and name are unique
    IF status_exists_by_id > 0 THEN
        LOG_DEBUG('INSERT_INVOICE_STATUS: Status ID ' || p_status_id || ' already exists.');
    ELSIF status_exists_by_name > 0 THEN
        LOG_DEBUG('INSERT_INVOICE_STATUS: Status name ' || p_status_name || ' already exists.');
    ELSE
        -- Insert the invoice status if both ID and name are unique
        INSERT INTO BLG_INVOICE_STATUSES (id, name)
        VALUES (p_status_id, p_status_name);
        LOG_INFORMATION('INSERT_INVOICE_STATUS: Status ' || p_status_name || ' with ID ' || p_status_id || ' created.');
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        LOG_ERROR('INSERT_INVOICE_STATUS: Error creating status ' || p_status_name || ': ' || SQLERRM);
        ROLLBACK;
END;
/

BEGIN
    INSERT_INVOICE_STATUS(0, 'Pending');
    INSERT_INVOICE_STATUS(1, 'Paid');
    INSERT_INVOICE_STATUS(2, 'Overdue');
    INSERT_INVOICE_STATUS(3, 'Canceled');
END;
/