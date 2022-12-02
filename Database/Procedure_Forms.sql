CREATE TABLE Procedure_Forms (
	form_id serial PRIMARY KEY,											-- ID used to uniquely identify each form in this table
	procedure_id integer,												-- The procedure this form is associated with
	filename varchar(255),												-- The name of the file.
	file_extension varchar(255),										-- The extension of the file.
	file_bytes bytea,													-- The content of the file stored as bytes.
	created_by integer,													-- The employee that completed the form for the Client
	create_datetime timestamp DEFAULT (NOW() AT TIME ZONE 'UTC'),		-- The date and time when the Form was first submitted
	last_edited_by integer,												-- The employee that updated (deleted) the form
	last_edited_datetime timestamp DEFAULT (NOW() AT TIME ZONE 'UTC'),	-- The date and time when the form was updated (deleted)
	isDeleted bool DEFAULT false,
	FOREIGN KEY (procedure_ID) REFERENCES Client_Procedures,
	FOREIGN KEY (created_by) REFERENCES Employees,
	FOREIGN KEY (last_edited_by) REFERENCES Employees
);