CREATE TABLE Client_Procedures (
	procedure_id serial PRIMARY KEY,
	procedure_datetime timestamp,							-- the date and time of a specific procedure
	client_ID integer,										-- the Client this procedure is associated with
	employee_ID integer,									-- the Employee working on this procedure
	procedure_notes varchar(10000),							-- Notes/Comments left on the procedure
	FOREIGN KEY(client_ID) REFERENCES Clients,
	FOREIGN KEY(employee_ID) REFERENCES Employees
);