SET SERVEROUTPUT ON;

-- Switch to GLOBAL PDB
ALTER SESSION SET CONTAINER = eshop_global;

-- Set the schema to the desired user
ALTER SESSION SET CURRENT_SCHEMA = ESHOP_GLOBAL_USER;

-- Insert regions and cities
BEGIN
    INSERT INTO IDNT_REGIONS (id, name) VALUES (0, 'Muntenia');
    INSERT INTO IDNT_REGIONS (id, name) VALUES (1, 'Transilvania');
    INSERT INTO IDNT_REGIONS (id, name) VALUES (2, 'Moldova');
    INSERT INTO IDNT_REGIONS (id, name) VALUES (3, 'Dobrogea');
    INSERT INTO IDNT_REGIONS (id, name) VALUES (4, 'Banat');
    INSERT INTO IDNT_REGIONS (id, name) VALUES (5, 'Crisana');
    INSERT INTO IDNT_REGIONS (id, name) VALUES (6, 'Maramures');
    INSERT INTO IDNT_REGIONS (id, name) VALUES (7, 'Oltenia');
    INSERT INTO IDNT_REGIONS (id, name) VALUES (8, 'Bucovina');

    INSERT INTO IDNT_CITIES (region_id, name) VALUES (0, 'Bucuresti');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (0, 'Ploiesti');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (0, 'Giurgiu');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (1, 'Cluj-Napoca');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (1, 'Sibiu');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (2, 'Iasi');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (2, 'Bacau');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (3, 'Constanta');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (3, 'Tulcea');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (4, 'Timisoara');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (4, 'Resita');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (5, 'Oradea');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (5, 'Arad');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (6, 'Baia Mare');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (6, 'Sighetu Marmatiei');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (7, 'Craiova');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (7, 'Targu Jiu');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (8, 'Radauti');
    INSERT INTO IDNT_CITIES (region_id, name) VALUES (8, 'Campulung Moldovenesc');
    COMMIT;
END;
/

-- Insert roles
BEGIN
    INSERT INTO IDNT_ROLES (id, name) VALUES (0, 'System Admin');
    INSERT INTO IDNT_ROLES (id, name) VALUES (1, 'Roles Admin');
    INSERT INTO IDNT_ROLES (id, name) VALUES (2, 'Purchases Rep');
    INSERT INTO IDNT_ROLES (id, name) VALUES (3, 'Sales Rep');
    COMMIT;
END;
/

-- Insert users
BEGIN
    INSERT INTO IDNT_USERS (email, password, salt) VALUES ('Ion.Popescu@email.com', 'Password1!', '123abc');
    INSERT INTO IDNT_USERS (email, password, salt) VALUES ('Maria.Ionescu@email.com', 'Password1!', '123abc');
    INSERT INTO IDNT_USERS (email, password, salt) VALUES ('Vasile.Popa@email.com', 'Password1!', '123abc');
    COMMIT;
END;
/

-- Insert user roles
BEGIN
    INSERT INTO IDNT_USER_ROLES (user_id, role_id) VALUES (1, 0); -- Ion Popescu -> System Admin
    INSERT INTO IDNT_USER_ROLES (user_id, role_id) VALUES (2, 0); -- Maria Ionescu -> System Admin
    INSERT INTO IDNT_USER_ROLES (user_id, role_id) VALUES (3, 0); -- Vasile Popa -> System Admin
    COMMIT;
END;
/

-- Insert discount types
BEGIN
    INSERT INTO SLS_DISCOUNT_TYPES (id, name) VALUES (0, 'Absolute value');
    INSERT INTO SLS_DISCOUNT_TYPES (id, name) VALUES (1, 'Percentage');
    COMMIT;
END;
/

-- Insert discount reasons
BEGIN
    INSERT INTO SLS_DISCOUNT_REASONS (id, name) VALUES (0, 'Fidelity');
    INSERT INTO SLS_DISCOUNT_REASONS (id, name) VALUES (1, 'Re-engagement');
    INSERT INTO SLS_DISCOUNT_REASONS (id, name) VALUES (2, 'Seasonal Sale');
    INSERT INTO SLS_DISCOUNT_REASONS (id, name) VALUES (3, 'Clearance');
    COMMIT;
END;
/

-- Insert product categories and subcategories
BEGIN
    INSERT INTO SLS_PRODUCT_CATEGORIES (id, name) VALUES (0, 'Sport');
    INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name) VALUES (0, 'Football');
    INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name) VALUES (0, 'Basketball');
    INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name) VALUES (0, 'Tennis');
    INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name) VALUES (0, 'Running');
    INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name) VALUES (0, 'Swimming');
    INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name) VALUES (0, 'Fitness');
    INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name) VALUES (0, 'Yoga');
    INSERT INTO SLS_PRODUCT_SUBCATEGORIES (category_id, name) VALUES (0, 'Hiking');
    COMMIT;
END;
/

-- Insert products
BEGIN
    INSERT INTO SLS_PRODUCTS (name, description, price_in_eur) VALUES ('Football Ball', 'Standard size football ball', 25);
    INSERT INTO SLS_PRODUCTS (name, description, price_in_eur) VALUES ('Basketball Ball', 'Standard size basketball ball', 30);
    INSERT INTO SLS_PRODUCTS (name, description, price_in_eur) VALUES ('Tennis Racket', 'Professional tennis racket', 75);
    INSERT INTO SLS_PRODUCTS (name, description, price_in_eur) VALUES ('Running Shoes', 'Comfortable running shoes', 60);
    INSERT INTO SLS_PRODUCTS (name, description, price_in_eur) VALUES ('Swimming Goggles', 'Anti-fog swimming goggles', 15);
    COMMIT;
END;
/

-- Insert product tags
BEGIN
    INSERT INTO SLS_PRODUCT_TAGS (name) VALUES ('sport');
    INSERT INTO SLS_PRODUCT_TAGS (name) VALUES ('fitness');
    INSERT INTO SLS_PRODUCT_TAGS (name) VALUES ('outdoor');
    COMMIT;
END;
/

-- Link products to subcategories
BEGIN
    INSERT INTO SLS_PRODUCT_PRODUCT_SUBCATEGORIES (product_id, subcategory_id) VALUES (1, 0); -- Football Ball -> Football
    INSERT INTO SLS_PRODUCT_PRODUCT_SUBCATEGORIES (product_id, subcategory_id) VALUES (2, 1); -- Basketball Ball -> Basketball
    INSERT INTO SLS_PRODUCT_PRODUCT_SUBCATEGORIES (product_id, subcategory_id) VALUES (3, 2); -- Tennis Racket -> Tennis
    INSERT INTO SLS_PRODUCT_PRODUCT_SUBCATEGORIES (product_id, subcategory_id) VALUES (4, 3); -- Running Shoes -> Running
    INSERT INTO SLS_PRODUCT_PRODUCT_SUBCATEGORIES (product_id, subcategory_id) VALUES (5, 4); -- Swimming Goggles -> Swimming
    COMMIT;
END;
/

-- Link product tags
BEGIN
    INSERT INTO SLS_PRODUCT_PRODUCT_TAGS (product_id, tag_id) VALUES (1, 1); -- Football Ball -> sport
    INSERT INTO SLS_PRODUCT_PRODUCT_TAGS (product_id, tag_id) VALUES (2, 1); -- Basketball Ball -> sport
    INSERT INTO SLS_PRODUCT_PRODUCT_TAGS (product_id, tag_id) VALUES (3, 1); -- Tennis Racket -> sport
    INSERT INTO SLS_PRODUCT_PRODUCT_TAGS (product_id, tag_id) VALUES (4, 2); -- Running Shoes -> fitness
    INSERT INTO SLS_PRODUCT_PRODUCT_TAGS (product_id, tag_id) VALUES (5, 3); -- Swimming Goggles -> outdoor
    COMMIT;
END;
/