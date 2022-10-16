-- Table to store all Employee information
CREATE TABLE Employees (
	employee_ID serial PRIMARY KEY,
	first_name varchar(50) not null,				-- Employee's first name
	last_name varchar(50) not null,					-- Employee's last name
	employee_username varchar(30) not null,			-- Username to login to the application
	employee_password varchar(100) not null,		-- Password to login to the application; varchar(100) used for possible encryption
);

-- Table to store all Client information
CREATE TABLE Clients (
	client_ID serial PRIMARY KEY,
	first_name varchar(50) not null,				-- Client's first name
	last_name varchar(50) not null,					-- Client's last name
	last_edited timestamp,							-- Last time the information in this row was edited
	employee_ID integer,							-- Employee ID associated with the employee that made the changes
	FOREIGN KEY(employee_ID) REFERENCES Employees,
);

-- Table to store all procedures completed
CREATE TABLE Client_Procedures (
	procedure_ID serial PRIMARY KEY,
	procedure_type varchar(30),						-- Type of procedure being performed for this entry
	procedure_date timestamp,						-- Date and time this procedure was completed
	client_ID integer,								-- Client ID associated with the client this procedure is being performed on
	employee_ID integer,							-- Employee ID associated with the employee working on this procedure
	FOREIGN KEY(client_ID) REFERENCES Clients,
	FOREIGN KEY(employee_ID) REFERENCES Employees,
);

-- Table used to store templates of forms uploaded
CREATE TABLE Empty_Forms (
	form_ID serial PRIMARY KEY,
	form_name varchar(30) not null,					-- The name of the form stored
	form bytea,										-- The form that is stored as a PDF
);

-- Table to store completed forms for any procedures completed
CREATE TABLE Completed_Forms (
	form_ID serial PRIMARY KEY,
	form bytea,										-- The completed form stored as a PDF
	procedure_ID integer,							-- The form this procedure is associated with
	FOREIGN KEY(procedure_ID) REFERENCES Client_Procedures,
);

-- Table to store photos taken for each procedure completed
CREATE TABLE Procedure_Photos (
	photo_ID serial PRIMARY KEY,
	photo bytea,									-- The photo being stored as a JPG or PNG
	procedure_ID integer,							-- The procedure this photo is associated with
	isBefore bool,									-- Is the photo the one taken before the procedure?
	isAfter bool,									-- Is the photo the one taken after the procedure?
	FOREIGN KEY(procedure_ID) REFERENCES Client_Procedures,
);

-- Table to store payment receipts from each procedure
CREATE TABLE Receipts (
	procedure_ID PRIMARY KEY,
	receipt bytea,									-- The receipt of payment being stored as a PDF
	FOREIGN KEY(procedure_ID) REFERENCES Client_Procedures,
);