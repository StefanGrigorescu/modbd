SET SERVEROUTPUT ON;

ALTER SESSION SET CONTAINER = eshop_muntenia;

ALTER SESSION SET CURRENT_SCHEMA = eshop_muntenia_user;

-- Delete trigger for IDNT_USERS
CREATE OR REPLACE TRIGGER trg_delete_idnt_users_muntenia
BEFORE DELETE ON IDNT_USERS
FOR EACH ROW
BEGIN
  RAISE_APPLICATION_ERROR(-20002, 'Deleting users is not allowed.');
END;
/