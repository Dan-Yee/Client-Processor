CREATE TABLE Clients (
	client_ID serial PRIMARY KEY,
	first_name varchar(50) NOT NULL,
	last_name varchar(50) NOT NULL,
	phone_number varchar(15),
	email_address varchar(50)
);