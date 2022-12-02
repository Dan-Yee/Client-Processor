CREATE TABLE Procedure_Photos (
    procedure_ID int,
    photo_name varchar(255),
    photo_extension varchar(255),
    photo_bytes bytea,
	isBefore bool,
	created_by integer,													-- The employee that uploaded this photo
	create_datetime timestamp DEFAULT (NOW() AT TIME ZONE 'UTC'),		-- The date and time this photo was uploaded
	last_edited_by integer,												-- The employee that last edited (deleted) this photo
	last_edited_datetime timestamp DEFAULT (NOW() AT TIME ZONE 'UTC'),	-- The date and time this photo was edited (deleted)
	isDeleted bool DEFAULT false,
	PRIMARY KEY (procedure_ID, photo_name),
	FOREIGN KEY (procedure_ID) REFERENCES Client_Procedures,
	FOREIGN KEY (created_by) REFERENCES Employees,
	FOREIGN KEY (last_edited_by) REFERENCES Employees
);