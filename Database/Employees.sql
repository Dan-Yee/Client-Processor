CREATE TABLE Employees (
	employee_ID serial PRIMARY KEY,			
	first_name varchar(50) NOT NULL,					-- Employee's first name
	last_name varchar(50) NOT NULL,						-- Employee's last name
	employee_username varchar(30) UNIQUE,				-- Username to login to the application
	employee_password char(88),							-- Hashed password to login to the database
	password_salt char(44),								-- The salt used for this password
	isAdministrator bool NOT NULL						-- Should this employee be able to access to the admin menu?
);