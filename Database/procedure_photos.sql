CREATE TABLE Procedure_photos (
    procedure_ID int,
    photo_name varchar(255),
    photo_extension varchar(255),
    isBefore bool, 
    photo_bytes bytea,
    CONSTRAINT procedure_ID
      FOREIGN KEY(procedure_ID) 
      REFERENCES client_procedures(procedure_ID)
);