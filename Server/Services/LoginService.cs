using Grpc.Core;
using DotNetEnv;
using Npgsql;

namespace Server.Services
{
    public class LoginService : Login.LoginBase
    {
        private readonly ILogger<LoginService> _logger;

        // credentials for testing purposes.
        private readonly string username = "admin";
        private readonly string password = "password12222";
        public LoginService(ILogger<LoginService> logger)
        {
            _logger = logger;
        }

        /*
         * Method to verify the credentials entered from the client side of the RPC
         */ 
        public override Task<LoginStatus> doLogin(Credentials request, ServerCallContext context)
        {
            Env.TraversePath().Load();
            LoginStatus status = new();
            status.IsSuccessfulLogin = CheckCredentials(request.Username, request.Password);
            return Task.FromResult(status);
        }

        /**
         * Check the login credentials against what is stored in the database
         */ 
        private static bool CheckCredentials(string username, string password)
        {
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;
            string? expectedPassword = String.Empty;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT employee_password FROM Employees WHERE employee_username = '" + username + "';";
                command = new NpgsqlCommand(@query, conn);
                conn.Open();
                reader = command.ExecuteReader();

                if(reader.HasRows)
                {
                    reader.Read();
                    expectedPassword = reader["employee_password"].ToString();
                }
                conn.Close();
                return password == expectedPassword;
            }
        }

        /**
         * Get a connection to the database
         */
        private static NpgsqlConnection GetConnection()
        {
            string? DB_HOST = Environment.GetEnvironmentVariable("HOST");
            string? DB_PORT = Environment.GetEnvironmentVariable("PORT");
            string? DB_NAME = Environment.GetEnvironmentVariable("DATABASE");
            string? DB_USERNAME = Environment.GetEnvironmentVariable("USERNAME");
            string? DB_PASSWORD = Environment.GetEnvironmentVariable("PASSWORD");
            string connectionString = "Server=" + DB_HOST + ";Port=" + DB_PORT + ";Database=" + DB_NAME + ";User Id=" + DB_USERNAME + ";Password=" + DB_PASSWORD;
            return new NpgsqlConnection(@connectionString);
        }
    }
}
