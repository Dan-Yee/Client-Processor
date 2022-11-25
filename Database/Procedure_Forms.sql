CREATE TABLE Procedure_Forms (
	form_id serial PRIMARY KEY,									-- ID used to uniquely identify each form in this table
	procedure_id integer,										-- The procedure this form is associated with
	filename varchar(255),										-- The name of the file.
	file_extension varchar(255),								-- The extension of the file.
	file_bytes bytea,											-- The content of the file stored as bytes.
	FOREIGN KEY (procedure_ID) REFERENCES Client_Procedures
);