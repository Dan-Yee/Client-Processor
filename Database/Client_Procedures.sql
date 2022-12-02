CREATE TABLE Client_Procedures (
	procedure_id serial PRIMARY KEY,
	procedure_name varchar(50),											-- the name of the procedure; user inputted
	procedure_notes varchar(10000),										-- Notes/Comments left on the procedure
	client_ID integer,													-- the Client this procedure is associated with
	employee_ID integer,												-- the Employee working on this procedure
	create_datetime timestamp DEFAULT (NOW() AT TIME ZONE 'UTC'),		-- the date and time of a specific procedure
	last_edited_by integer,												-- the Employee who last made changes to the procedure
	last_edited_datetime timestamp DEFAULT (NOW() AT TIME ZONE 'UTC'),	-- the date and time of the last time the procedure was updated.
	isDeleted bool DEFAULT false,
	FOREIGN KEY (client_ID) REFERENCES Clients,
	FOREIGN KEY (employee_ID) REFERENCES Employees,
	FOREIGN KEY (last_edited_by) REFERENCES Employees
);