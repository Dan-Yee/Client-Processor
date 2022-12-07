CREATE TABLE Empty_Forms (
	filename varchar(255) UNIQUE PRIMARY KEY,		-- the name of the file being uploaded
	file_extension varchar(255),					-- the extension of the file
	file_bytes bytea								-- the file stored as bytes
);