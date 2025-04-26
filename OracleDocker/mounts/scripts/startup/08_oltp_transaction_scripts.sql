CREATE OR REPLACE PROCEDURE PLACE_ORDER (
    p_customer_id IN NUMBER,
    p_address IN NVARCHAR2 DEFAULT NULL,
    p_items_csv IN NVARCHAR2
) IS
    customer_region_id NUMBER;
    order_id NUMBER;
    final_address NVARCHAR2(850);
    status_id NUMBER := 0; -- Default status ID for "Pending"
BEGIN
    -- Query the customer's region ID and address if not provided
    IF p_address IS NULL THEN
        SELECT region_id, 
               NVL((SELECT street || ', ' || str_number || ', ' || postal_code || ', ' || other_details
                    FROM IDNT_USER_ADDRESSES
                    WHERE user_id = p_customer_id
                    FETCH FIRST 1 ROWS ONLY), 'No Address') 
        INTO customer_region_id, final_address
        FROM IDNT_USERS
        WHERE id = p_customer_id;
    ELSE
        SELECT region_id INTO customer_region_id
        FROM IDNT_USERS
        WHERE id = p_customer_id;
        final_address := p_address;
    END IF;

    -- Insert the order into SLS_Orders
    INSERT INTO SLS_ORDERS (
        customer_id, customer_region_id, address, status_id
    ) VALUES (
        p_customer_id, customer_region_id, final_address, status_id
    ) RETURNING id INTO order_id;

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
            INSERT INTO SLS_ORDER_ITEMS (
                order_id, product_id, quantity
            ) VALUES (
                order_id, product_id, quantity
            );
        EXCEPTION
            WHEN OTHERS THEN
                LOG_ERROR('PLACE_ORDER: Error processing item ' || item.item || ': ' || SQLERRM);
        END;
    END LOOP;

    -- Commit the transaction
    COMMIT;

    LOG_INFORMATION('PLACE_ORDER: Order ' || order_id || ' placed successfully for customer ID ' || p_customer_id || '.');
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        LOG_ERROR('PLACE_ORDER: Error placing order for customer ID ' || p_customer_id || ': ' || SQLERRM);
END;
/
