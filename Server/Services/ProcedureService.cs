using Grpc.Core;
using DotNetEnv;
using Npgsql;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;

namespace Server.Services
{
    public class ProcedureService : Procedure.ProcedureBase
    {
        /// <summary>
        /// Method <c>GetConnection</c> creates a connection to a PostgreSQL Database.
        /// </summary>
        /// <returns>A connection to the PostgreSQL database using database credentials</returns>
        private static NpgsqlConnection GetConnection()
        {
            Env.TraversePath().Load();
            string? DB_HOST = Environment.GetEnvironmentVariable("HOST");
            string? DB_PORT = Environment.GetEnvironmentVariable("PORT");
            string? DB_NAME = Environment.GetEnvironmentVariable("DATABASE");
            string? DB_USERNAME = Environment.GetEnvironmentVariable("USERNAME");
            string? DB_PASSWORD = Environment.GetEnvironmentVariable("PASSWORD");
            string connectionString = "Server=" + DB_HOST + ";Port=" + DB_PORT + ";Database=" + DB_NAME + ";User Id=" + DB_USERNAME + ";Password=" + DB_PASSWORD;
            return new NpgsqlConnection(@connectionString);
        }

        /// <summary>
        /// Implementation of the RPC addProcedure for adding a completed procedure to the database.
        /// </summary>
        /// <param name="request">An object containing the client id and employee id associated with the procedure, and forms/images associated with the procedure</param>
        /// <param name="context"></param>
        /// <returns>An object containing the status of the RPC and a status message.</returns>
        public override Task<ProcedureID> addProcedure(ProcedureInfo request, ServerCallContext context)
        {
            return Task.FromResult(InsertProcedure(request));
        }

        /// <summary>
        /// Method <c>InsertProcedure</c> inserts the procedure into the database. This method updates three tables: Client_Procedures, Client_Forms, Client_Images
        /// </summary>
        /// <param name="info">An object containing the client id and employee id associated with the procedure, and forms/images associated with the procedure</param>
        /// <returns>An object containing the status of the RPC and a status message.</returns>
        private static ProcedureID InsertProcedure(ProcedureInfo info)
        {
            ProcedureID newPID = new();
            NpgsqlCommand command;
            string query;
            int procedureID;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "INSERT INTO Client_Procedures (procedure_name, procedure_notes, client_id, employee_id, last_edited_by) VALUES ($1, $2, $3, $4, $5) RETURNING procedure_id;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = info.ProcedureName},
                        new() {Value = info.ProcedureNotes},
                        new() {Value = info.ClientID},
                        new() {Value = info.EmployeeID},
                        new() {Value = info.EmployeeID},
                    }
                };
                conn.Open();
                try
                {
                    procedureID = Convert.ToInt32(command.ExecuteScalar());                    // This specific query returns the procedure_id of the newly inserted procedure.
                    newPID.PID = procedureID;
                } catch (NpgsqlException pgE)
                {
                    Console.WriteLine("ProcedureService.addProcedure RPC Postgresql Error State: " + pgE.SqlState);
                    newPID.PID = -1;
                }
                conn.Close();
            }
            return newPID;
        }

        /// <summary>
        /// Implementation of the RPC getProcedures for getting all procedures associated with a specific client by ID.
        /// </summary>
        /// <param name="request">An object containing the id of the client to get the procedures for</param>
        /// <param name="context"></param>
        /// <returns>An object containing zero or more procedures associated with a specific client</returns>
        public override Task<AllProcedures> getProcedures(ClientID request, ServerCallContext context)
        {
            return Task.FromResult(SelectAllProcedures(request));
        }

        /// <summary>
        /// Method <c>SelectAllProcedures</c> queries the database table Client_Procedures for any procedure associated with a specific client by ID.
        /// </summary>
        /// <param name="id">An object containing the id of the client to get the procedures for</param>
        /// <returns>An object containing zero or more procedures associated with a specific client</returns>
        private static AllProcedures SelectAllProcedures(ClientID id)
        {
            AllProcedures procedures = new();
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            DateTime createDT, editDT;
            string query;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT * FROM Client_Procedures WHERE client_id = $1 AND isDeleted = false ORDER BY create_datetime DESC;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = id.CID}
                    }
                };
                conn.Open();
                reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        createDT = Convert.ToDateTime(reader["create_datetime"]);
                        createDT = DateTime.SpecifyKind(createDT, DateTimeKind.Utc);
                        editDT = Convert.ToDateTime(reader["last_edited_datetime"]);
                        editDT = DateTime.SpecifyKind(editDT, DateTimeKind.Utc);
                        var currentProcedure = new ProcedureInfo
                        {
                            ProcedureID = Convert.ToInt32(reader["procedure_id"]),
                            ProcedureName = reader["procedure_name"].ToString(),
                            ProcedureDatetime = createDT.ToLocalTime().ToString(),
                            ClientID = Convert.ToInt32(reader["client_id"]),
                            EmployeeID = Convert.ToInt32(reader["employee_id"]),
                            ProcedureNotes = reader["procedure_notes"].ToString(),
                            ProcedureLastEditDatetime = editDT.ToLocalTime().ToString(),
                            LastEditEmployeeID = Convert.ToInt32(reader["last_edited_by"]),
                        };
                        procedures.Procedures.Add(currentProcedure);
                    }
                }
                reader.Close();
                conn.Close();
            }
            return procedures;
        }

        /// <summary>
        /// Implementation of RPC getFormFields for getting all the fillable fields from a PDF form, given the form name.
        /// </summary>
        /// <param name="request">An object containing the name of the form.</param>
        /// <param name="context"></param>
        /// <returns>An object of objects where each sub-object represents a field that was found on the specified form.</returns>
        public override Task<FormFields> getFormFields(FormName request, ServerCallContext context)
        {
            return Task.FromResult(GetFields(request));
        }

        /// <summary>
        /// Method <c>GetFields</c> queries the database for the form (stored as bytes) that is specified by the provided name. It accesses the form and finds all the fields that can be filled out, returning the names and types.
        /// </summary>
        /// <param name="fName">An object containing the name of the form.</param>
        /// <returns>An object containing zero or more fields, each representing an individual field from the original PDF.</returns>
        private static FormFields GetFields(FormName fName)
        {
            FormFields fields = new();
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT file_bytes FROM Empty_Forms WHERE filename = $1";
                command = new(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = fName.FormName_}
                    }
                };
                conn.Open();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    fields.FName = fName;
                    using (MemoryStream input = new((byte[])reader["file_bytes"]))
                    {
                        PdfReader pdfReader = new(input);
                        PdfDocument document = new(pdfReader);
                        PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, false);
                        IDictionary<String, PdfFormField> formFields = acroForm.GetFormFields();
                        foreach (KeyValuePair<String, PdfFormField> field in formFields)
                        {
                            if (field.Key.Contains(".1"))
                                continue;
                            Field currentField = new()
                            {
                                FieldName = field.Key,
                                FieldType = field.Value.ToString()
                            };
                            fields.Fields.Add(currentField);
                        }
                    }
                }
                reader.Close();
                conn.Close();
            }
            return fields;
        }

        /// <summary>
        /// Implementation of the deleteProcedure RPC that deletes a procedure and all relevant information such as forms and images.
        /// </summary>
        /// <param name="request">The procedure ID being referenced for deletion.</param>
        /// <param name="context"></param>
        /// <returns>An object that states whether or not the operation was successful.</returns>
        public override Task<ServiceStatus> deleteProcedure(ProcedureUpdateInfo request, ServerCallContext context)
        {
            return Task.FromResult(DeleteProcedure(request));
        }

        /// <summary>
        /// Method <c>DeleteProcedure</c> deletes a procedure from the Client_Procedures table and other tables that contain relevant information, given the ID.
        /// </summary>
        /// <param name="pID">The procedure ID being referenced for deletion.</param>
        /// <returns>An object that states whether or not the operation was successful.</returns>
        private static ServiceStatus DeleteProcedure(ProcedureUpdateInfo info)
        {
            ServiceStatus sStatus = new();
            NpgsqlCommand command;
            string query;
            int status;

            using (NpgsqlConnection conn = GetConnection())
            {
                try
                {
                    // Delete all foreign key references to this procedure from the Procedure_Photos table first.
                    query = "UPDATE Procedure_Photos SET " +
                            "isDeleted = true, " +
                            "last_edited_by = $1, " +
                            "last_edited_datetime = DEFAULT " +
                            "WHERE procedure_id = $2;";
                    command = new NpgsqlCommand(@query, conn)
                    {
                        Parameters =
                        {
                            new() {Value = info.EmployeeID},
                            new() {Value = info.PID},
                        }
                    };
                    conn.Open();
                    status = command.ExecuteNonQuery();
                    conn.Close();

                    // Delete all foreign key references to this procedure from the Procedure_Forms table first.
                    query = "UPDATE Procedure_Forms SET " +
                            "isDeleted = true, " +
                            "last_edited_by = $1, " +
                            "last_edited_datetime = DEFAULT " +
                            "WHERE procedure_id = $2;";
                    command = new NpgsqlCommand(@query, conn)
                    {
                        Parameters =
                        {
                            new() {Value = info.EmployeeID},
                            new() {Value = info.PID},
                        }
                    };
                    conn.Open();
                    status = command.ExecuteNonQuery();
                    conn.Close();

                    // Finally, delete from the Client_Procedures table once all foreign key references have been removed.
                    query = "UPDATE Client_Procedures SET " +
                            "isDeleted = true, " +
                            "last_edited_by = $1, " +
                            "last_edited_datetime = DEFAULT " +
                            "WHERE procedure_id = $2;";
                    command = new NpgsqlCommand(@query, conn)
                    {
                        Parameters =
                        {
                            new() {Value = info.EmployeeID},
                            new() {Value = info.PID}
                        }
                    };
                    conn.Open();
                    status = command.ExecuteNonQuery();
                    conn.Close();
                }
                catch (NpgsqlException pgE)
                {
                    Console.WriteLine("ProcedureService.deleteProcedure RPC Postgresql Error State: " + pgE.SqlState);
                    status = 0;
                }

                if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
            if(status > 0)
            {
                sStatus.IsSuccessfulOperation = true;
                sStatus.StatusMessage = "Success: Procedure deleted.";
            }
            else
            {
                sStatus.IsSuccessfulOperation = false;
                sStatus.StatusMessage = "Error: Unable to delete Procedure.";
            }
            return sStatus;
        }

        /// <summary>
        /// Implementation of the RPC completeForm that gets a form from the Empty_Forms table, fills in fields, and stores it in the Procedure_Forms table.
        /// </summary>
        /// <param name="request">An object containing the Procedure ID, Form name, Form fields and their respective values.</param>
        /// <param name="context"></param>
        /// <returns>An object stating whether or not this operation was successful.</returns>
        public override Task<ServiceStatus> completeForm(CompleteFormInfo request, ServerCallContext context)
        {
            return Task.FromResult(CompleteForm(request));
        }

        /// <summary>
        /// Method <c>CompleteForm</c> queries the Empty_Forms table for the form to be filled out. It then edit that form and stores it in the Procedure_Forms table.
        /// </summary>
        /// <param name="info">An object containing the Procedure ID, Form name, Form fields and their respective values.</param>
        /// <returns>An object stating whether or not this operation was successful.</returns>
        private static ServiceStatus CompleteForm(CompleteFormInfo info)
        {
            ServiceStatus sStatus = new();
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;
            byte[]? fileToStore = null;
            string? fileToStoreExt = string.Empty;
            int insertStatus = 0;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT file_extension, file_bytes FROM Empty_Forms WHERE filename = $1;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = info.Form.FName.FormName_}
                    }
                };
                conn.Open();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    fileToStoreExt = Convert.ToString(reader["file_extension"]);
                    using (MemoryStream fileInput = new((byte[])reader["file_bytes"]))
                    {
                        using (MemoryStream fileOutput = new())
                        {
                            PdfReader pdfReader = new(fileInput);
                            PdfWriter pdfWriter = new(fileOutput);
                            PdfDocument pdfDocument = new(pdfReader, pdfWriter);
                            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
                            IDictionary<string, PdfFormField> formFields = acroForm.GetFormFields();
                            PdfFormField? fieldToSet;

                            foreach (Field field in info.Form.Fields)
                            {
                                formFields.TryGetValue(field.FieldName, out fieldToSet);
                                if (fieldToSet != null)
                                {
                                    fieldToSet.SetValue(field.FieldValue);
                                }
                            }
                            acroForm.FlattenFields();
                            pdfDocument.Close();
                            fileToStore = fileOutput.ToArray();
                        }
                    }
                }
                conn.Close();
                reader.Close();

                // Only attempt to insert if the form that was edited exists and is not null
                if (fileToStore != null)
                {
                    // store the completed form (file) into the Procedure_Forms table
                    query = "INSERT INTO Procedure_Forms (procedure_id, filename, file_extension, file_bytes, created_by, last_edited_by) VALUES ($1, $2, $3, $4, $5, $6);";
                    command = new NpgsqlCommand(@query, conn)
                    {
                        Parameters =
                        {
                            new() {Value = info.ProcedureID},
                            new() {Value = info.Form.FName.FormName_},
                            new() {Value = fileToStoreExt},
                            new() {Value = fileToStore},
                            new() {Value = info.EmployeeID},
                            new() {Value = info.EmployeeID},
                        }
                    };
                    conn.Open();
                    try
                    {
                        insertStatus = command.ExecuteNonQuery();
                    }
                    catch (NpgsqlException pgE)
                    {
                        Console.WriteLine("ProcedureService.completeForm RPC Postgresql Error State: " + pgE.SqlState);
                        insertStatus = 0;
                    }
                    conn.Close();
                }

                // INSERT returns the total number of rows affected. We expect this value to be "1" for this operation to be considered successful.
                if (insertStatus == 1)
                {
                    sStatus.IsSuccessfulOperation = true;
                    sStatus.StatusMessage = "Success: Form was completed and saved.";
                }
                else
                {
                    sStatus.IsSuccessfulOperation = false;
                    sStatus.StatusMessage = "Error: Form was not saved.";
                }
            }
            return sStatus;
        }

        /// <summary>
        /// Implementation of the updateProcedure RPC that updates a procedures name and notes that are currently stored in the database.
        /// </summary>
        /// <param name="request">An object containing the (possibly) new procedure name and notes.</param>
        /// <param name="context"></param>
        /// <returns>An object that states whether or not the operation was successful.</returns>
        public override Task<ServiceStatus> updateProcedure(ProcedureInfo request, ServerCallContext context)
        {
            return Task.FromResult(UpdateProcedure(request));
        }

        /// <summary>
        /// Method <c>UpdateProcedure</c> updates a Procedure record in the database with the new name and notes.
        /// </summary>
        /// <param name="newInfo">An object containing the (possibly) new procedure name and notes.</param>
        /// <returns>An object that states whether or not the operation was successful.</returns>
        private static ServiceStatus UpdateProcedure(ProcedureInfo newInfo)
        {
            ServiceStatus sStatus = new();
            NpgsqlCommand command;
            string query;
            int status;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "UPDATE Client_Procedures SET " +
                        "procedure_name = $1, " +
                        "procedure_notes = $2, " +
                        "last_edited_by = $3, " +
                        "last_edited_datetime = DEFAULT " +
                        "WHERE procedure_id = $4;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = newInfo.ProcedureName},
                        new() {Value = newInfo.ProcedureNotes},
                        new() {Value = newInfo.EmployeeID},
                        new() {Value = newInfo.ProcedureID},
                    }
                };
                conn.Open();
                try
                {
                    status = command.ExecuteNonQuery();
                }
                catch (NpgsqlException pgE)
                {
                    Console.WriteLine("ProcedureService.updateProcedure RPC Postgresql Error State: " + pgE.SqlState);
                    status = 0;
                }
                conn.Close();

                if (status == 1)
                {
                    sStatus.IsSuccessfulOperation = true;
                    sStatus.StatusMessage = "Success: Procedure was updated.";
                }
                else
                {
                    sStatus.IsSuccessfulOperation = false;
                    sStatus.StatusMessage = "Error: Unable to update procedure.";
                }
            }
            return sStatus;
        }

        /// <summary>
        /// Implementation of the searchProcedure RPC that searches for procedures given a name.
        /// </summary>
        /// <param name="request">The name of the procedure being searched for.</param>
        /// <param name="context"></param>
        /// <returns>An object containing zero or more sub-objects where each sub-object represents a procedure that fit the search criteria.</returns>
        public override Task<AllProcedures> searchProcedure(ProcedureName request, ServerCallContext context)
        {
            return Task.FromResult(SearchProcedureByName(request));
        }

        /// <summary>
        /// Method <c>SearchProcedureByName</c> queries the Client_Procedures table for records that match the provided search term.
        /// </summary>
        /// <param name="procName">The name of the procedure being searched for.</param>
        /// <returns>An object containing zero or more sub-objects where each sub-object represents a procedure that fit the search criteria.</returns>
        private static AllProcedures SearchProcedureByName(ProcedureName procName)
        {
            AllProcedures allProcedures = new();
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;
            string searchTerm = procName.PName + "%";
            DateTime createDT, editDT;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT * FROM Client_Procedures WHERE LOWER(procedure_name) LIKE LOWER($1) AND isDeleted = false ORDER BY procedure_datetime DESC;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = searchTerm},
                    }
                };
                conn.Open();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        createDT = Convert.ToDateTime(reader["create_datetime"]);
                        createDT = DateTime.SpecifyKind(createDT, DateTimeKind.Utc);
                        editDT = Convert.ToDateTime(reader["last_edited_datetime"]);
                        editDT = DateTime.SpecifyKind(editDT, DateTimeKind.Utc);
                        ProcedureInfo current = new()
                        {
                            ProcedureID = Convert.ToInt32(reader["procedure_id"]),
                            ProcedureName = Convert.ToString(reader["procedure_name"]),
                            ProcedureDatetime = createDT.ToLocalTime().ToString(),
                            ClientID = Convert.ToInt32(reader["client_id"]),
                            EmployeeID = Convert.ToInt32(reader["employee_id"]),
                            ProcedureNotes = Convert.ToString(reader["procedure_notes"]),
                            ProcedureLastEditDatetime = editDT.ToLocalTime().ToString(),
                            LastEditEmployeeID = Convert.ToInt32(reader["last_edited_by"]),
                        };
                        allProcedures.Procedures.Add(current);
                    }
                }
                reader.Close();
                conn.Close();
            }
            return allProcedures;
        }
    }
}