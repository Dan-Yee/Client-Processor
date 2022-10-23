CREATE TABLE Employees (
	employee_ID serial PRIMARY KEY,			
	first_name varchar(50) NOT NULL,			-- Employee's first name
	last_name varchar(50) NOT NULL,				-- Employee's last name
	employee_username varchar(30),				-- Username to login to the application
	employee_password varchar(100)				-- Password to login to the database.
);