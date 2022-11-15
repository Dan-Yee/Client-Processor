using Grpc.Core;
using DotNetEnv;
using Npgsql;

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
        public override Task<ServiceStatus> addProcedure(ProcedureInfo request, ServerCallContext context)
        {
            return Task.FromResult(InsertProcedure(request));
        }

        /// <summary>
        /// Method <c>InsertProcedure</c> inserts the procedure into the database. This method updates three tables: Client_Procedures, Client_Forms, Client_Images
        /// </summary>
        /// <param name="info">An object containing the client id and employee id associated with the procedure, and forms/images associated with the procedure</param>
        /// <returns>An object containing the status of the RPC and a status message.</returns>
        private static ServiceStatus InsertProcedure(ProcedureInfo info)
        {
            ServiceStatus status = new();
            NpgsqlCommand command;
            string query;
            int procedureID;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "INSERT INTO Client_Procedures (procedure_name, procedure_datetime, client_id, employee_id, procedure_notes) VALUES ($1, $2, $3, $4, $5) RETURNING procedure_id;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = info.ProcedureName},
                        new() {Value = DateTime.UtcNow},
                        new() {Value = info.ClientID},
                        new() {Value = info.EmployeeID},
                        new() {Value = info.ProcedureNotes}
                    }
                };
                conn.Open();
                try
                {
                    procedureID = command.ExecuteNonQuery();                    // This specific query returns the procedure_id of the newly inserted procedure.
                    status.IsSuccessfulOperation = true;
                    status.StatusMessage = "Success: Procedure was saved.";
                } catch (NpgsqlException pgE)
                {
                    status.IsSuccessfulOperation = false;
                    status.StatusMessage = "Error: Unable to save procedure.";
                }
                conn.Close();
            }
            return status;
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
            DateTime dt;
            string query;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT * FROM Client_Procedures WHERE client_id = $1;";
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
                        dt = Convert.ToDateTime(reader["procedure_datetime"]);
                        dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                        var currentProcedure = new ProcedureInfo
                        {
                            ProcedureID = Convert.ToInt32(reader["procedure_id"]),
                            ProcedureName = reader["procedure_name"].ToString(),
                            ProcedureDatetime = dt.ToLocalTime().ToString(),
                            ClientID = Convert.ToInt32(reader["client_id"]),
                            EmployeeID = Convert.ToInt32(reader["employee_id"]),
                            ProcedureNotes = reader["procedure_notes"].ToString()
                        };
                        procedures.Procedures.Add(currentProcedure);
                    }
                }
                reader.Close();
                conn.Close();
            }
            return procedures;
        }
    }
}