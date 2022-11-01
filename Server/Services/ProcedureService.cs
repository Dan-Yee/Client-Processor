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
            return Task.FromResult(insertProcedure(request));
        }

        /// <summary>
        /// Method <c>InsertProcedure</c> inserts the procedure into the database. This method updates three tables: Client_Procedures, Client_Forms, Client_Images
        /// </summary>
        /// <param name="info">An object containing the client id and employee id associated with the procedure, and forms/images associated with the procedure</param>
        /// <returns>An object containing the status of the RPC and a status message.</returns>
        private static ServiceStatus insertProcedure(ProcedureInfo info)
        {
            ServiceStatus status = new();
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
            return procedures;
        }
    }
}