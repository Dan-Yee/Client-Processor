using Grpc.Core;
using DotNetEnv;
using Npgsql;
using Google.Protobuf.WellKnownTypes;

namespace Server.Services
{
    public class EmployeeService : Employee.EmployeeBase
    {
        private ILogger<EmployeeService> _logger;
        public EmployeeService(ILogger<EmployeeService> logger)
        {
            _logger = logger;
        }

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

        /*
         * Implementation of the newEmployee RPC for adding a new employee.
         * Request contains the first name, last name, username, and password for the new employee.
         */
        public override Task<ServiceStatus> newEmployee(EmployeeInfo request, ServerCallContext context)
        {
            ServiceStatus status = new();
            status.IsSuccessfulOperation = CreateNewEmployee(request);
            return Task.FromResult(status);
        }

        /// <summary>
        /// Method <c>CreateNewEmployee</c> inserts a new record into Employees table in the database
        /// </summary>
        /// <param name="employeeInfo">An object containing the first name, last name, username, and password of the new employee</param>
        /// <returns><c>true</c> if the INSERT operation was successful. <c>false</c> otherwise.</returns>
        private static bool CreateNewEmployee(EmployeeInfo employeeInfo)
        {
            NpgsqlCommand command;
            string query;
            int status;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "INSERT INTO Employees (first_name, last_name, employee_username, employee_password) VALUES ($1, $2, $3, $4);";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = employeeInfo.FirstName},
                        new() {Value = employeeInfo.LastName},
                        new() {Value = employeeInfo.Credentials.Username},
                        new() {Value = employeeInfo.Credentials.Password}
                    }
                };
                conn.Open();
                status = command.ExecuteNonQuery();
                conn.Close();
            }
            return status == 1;                                             // INSERT returns the number of rows affected. This operation expects 1 to be successful.
        }

        /*
         * Implementation of the updateEmployee RPC for updating information about an employee.
         * Request contains the employee's first name, last name, username and password - all of which can be changed as needed.
         */
        public override Task<ServiceStatus> updateEmployee(EmployeeInfo request, ServerCallContext context)
        {
            ServiceStatus status = new();
            status.IsSuccessfulOperation = UpdateEmployeeRecord(request);
            return Task.FromResult(status);
        }

        /// <summary>
        /// Method <c>UpdateEmployeeRecord</c> takes in an object of EmployeeInfo and executes a UPDATE to the database to change the record of a specific employee.
        /// </summary>
        /// <param name="info">An object containing the employee's id, first name, last name, username, and password. Any of these fields, except for ID, can be changed.</param>
        /// <returns><c>true</c> if the UPDATE operation was successful. <c>false</c> otherwise.</returns>
        private static bool UpdateEmployeeRecord(EmployeeInfo info)
        {
            NpgsqlCommand command;
            string query;
            int status;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "UPDATE Employees SET " +
                    "first_name = $1, " +
                    "last_name = $2, " +
                    "employee_username = $3, " +
                    "employee_password = $4 " +
                    "WHERE employee_id = $5;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = info.FirstName},
                        new() {Value = info.LastName},
                        new() {Value = info.Credentials.Username},
                        new() {Value = info.Credentials.Password},
                        new() {Value = info.EmployeeId}
                    }
                };
                conn.Open();
                status = command.ExecuteNonQuery();
                conn.Close();
            }
            return status == 1;                                             // UPDATE returns the number of rows affected. This operation expects 1 to be consid
        } 

        /*
         * Implementation of the getEmployees RPC for getting information about all employees stored in the database.
         */
        public override Task<AllEmployeesInfo> getEmployees(Empty request, ServerCallContext context)
        {
            return Task.FromResult(SelectAllEmployees());
        }

        /// <summary>
        /// Method <c>SelectAllEmployees</c> selects all rows from the Employee table and packs it into a message to be sent back.
        /// </summary>
        /// <returns>A Protobuf message containing information about every Employee stored in the database</returns>
        private static AllEmployeesInfo SelectAllEmployees()
        {
            AllEmployeesInfo allEmployees = new();
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT * FROM Employees;";
                command = new NpgsqlCommand(@query, conn);
                conn.Open();
                reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        var currentCredentials = new LoginCredentials
                        {
                            Username = reader["employee_username"].ToString(),
                            Password = reader["employee_password"].ToString()
                        };
                        var current = new EmployeeInfo
                        {
                            EmployeeId = Convert.ToInt32(reader["employee_id"]),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            Credentials = currentCredentials
                        };
                        allEmployees.Employees.Add(current);
                    }
                }
                reader.Close();
                conn.Close();
            }
            return allEmployees;
        }

        /*
         * Implementation of the doLogin RPC for verifying login credentials.
         * Request contains the username and password being verified for login.
         */
        public override Task<LoginStatus> doLogin(LoginCredentials request, ServerCallContext context)
        {
            LoginStatus status = new();
            status.IsSuccessfulLogin = CheckCredentials(request.Username, request.Password);
            return Task.FromResult(status);
        }

        /// <summary>
        /// Method <c>CheckCredentials</c> queries the database for the password given the username.
        /// </summary>
        /// <param name="username">The username of the account</param>
        /// <param name="password">The user entered password to be verified against what is already in the database</param>
        /// <returns><c>true</c> if <c>password</c> matches what is stored in the database. <c>false</c> otherwise</returns>
        private static bool CheckCredentials(string username, string password)
        {
            NpgsqlCommand command;
            string query;
            string? expectedPassword;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT employee_password FROM Employees WHERE employee_username = $1;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = username}
                    }
                };
                conn.Open();
                expectedPassword = (command.ExecuteScalar() == null) ? string.Empty : command.ExecuteScalar().ToString();
                conn.Close();
            }
            return password == expectedPassword;
        }
    }
}