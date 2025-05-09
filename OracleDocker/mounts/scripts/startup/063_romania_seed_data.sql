SET SERVEROUTPUT ON;

ALTER SESSION SET CONTAINER = eshop_romania;

ALTER SESSION SET CURRENT_SCHEMA = ESHOP_ROMANIA_USER;

CREATE OR REPLACE PROCEDURE PLACE_ORDER (
    p_order_id IN NUMBER,
    p_customer_id IN NUMBER,
    p_address IN NVARCHAR2 DEFAULT 'Bucuresti Sector 2 Straga Pacii numarul 14 Bloc 34 Scara A',
    p_items_csv IN NVARCHAR2,
    is_success OUT NUMBER,
    p_created_on IN TIMESTAMP DEFAULT SYSTIMESTAMP
) IS
    customer_region_id NUMBER;
    final_address NVARCHAR2(850);
    order_exists NUMBER := 0;
BEGIN
    -- Check if the order_id already exists
    SELECT COUNT(*) INTO order_exists
    FROM SLS_ORDERS
    WHERE id = p_order_id;

    IF order_exists > 0 THEN
        -- If order_id exists, set is_success to false (0)
        is_success := 0;
        LOG_DEBUG('Place_Order: Could not place order ' || p_order_id || ' for customer ID ' || p_customer_id || '. Order ID already exists.');
    ELSE
        -- Query the customer's region ID and address if not provided
        BEGIN
                SELECT region_id INTO customer_region_id
                FROM IDNT_USERS
                WHERE id = p_customer_id;
                final_address := p_address;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                is_success := 0;
                LOG_WARNING('Place_Order: No address found for customer ID ' || p_customer_id || '.');
                RETURN;
        END;

        -- Insert the order into SLS_Orders
        INSERT INTO SLS_ORDERS (
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
                INSERT INTO SLS_ORDER_ITEMS (
                    order_id, product_id, quantity
                ) VALUES (
                    p_order_id, product_id, quantity
                );
            EXCEPTION
                WHEN OTHERS THEN
                    LOG_ERROR('PLACE_ORDER: Error placing order ' || p_order_id || ' for customer ID ' || p_customer_id || '. Could not add item ' || item.item || ': ' || SQLERRM);
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
        LOG_ERROR('PLACE_ORDER: Error placing order ' || p_order_id || ' for customer ID ' || p_customer_id || ': ' || SQLERRM);
END;
/
