CREATE TABLE Clients (
	client_ID serial PRIMARY KEY,
	first_name varchar(50) NOT NULL,			-- Client's first name
	last_name varchar(50) NOT NULL,				-- Client's last name
	phone_number varchar(15),					-- Client's phone number
	email_address varchar(50)					-- (Optional) Client's email address
);