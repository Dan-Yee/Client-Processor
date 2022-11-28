using Grpc.Core;
using DotNetEnv;
using Npgsql;
using Google.Protobuf.WellKnownTypes;

namespace Server.Services
{
    public class ClientService : Client.ClientBase
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
        /// Implementation of the RPC for creating a new Client in the database
        /// </summary>
        /// <param name="request">An object containing all the new Client's information: first name, last name, phone number, email (optional)</param>
        /// <param name="context"></param>
        /// <returns><c>true</c> if the operation was successful. <c>false</c> otherwise.</returns>
        public override Task<ServiceStatus> newClient(ClientInfo request, ServerCallContext context)
        {
            ServiceStatus status = new();
            status.IsSuccessfulOperation = CreateNewClient(request);
            return Task.FromResult(status);
        }

        /// <summary>
        /// Method <c>CreateNewClient</c> inserts a new record into the Clients table in the database
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        private static bool CreateNewClient(ClientInfo clientInfo)
        {
            NpgsqlCommand command;
            string query;
            int status;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "INSERT INTO Clients (first_name, last_name, phone_number, email_address) VALUES ($1, $2, $3, $4);";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = clientInfo.FirstName},
                        new() {Value = clientInfo.LastName},
                        new() {Value = clientInfo.PhoneNumber},
                        new() {Value = clientInfo.Email}
                    }
                };
                conn.Open();
                status = command.ExecuteNonQuery();
                conn.Close();
            }
            return status == 1;                                             // INSERT returns the number of rows affected. For this operation, we expect 1 row to be affected for it to be considered successful.
        }

        /// <summary>
        /// Implementation of the getClients RPC that retrieves all records of all Clients from the database.
        /// </summary>
        /// <param name="request">Empty. This RPC does not require any arguments</param>
        /// <param name="context"></param>
        /// <returns>An object of objects where each sub-object represents an entry in the Clients table in the database</returns>
        public override Task<AllClients> getClients(Empty request, ServerCallContext context)
        {
            return Task.FromResult(SelectAllClients());
        }

        /// <summary>
        /// Method <c>SelectAllClients</c> selects all rows from the Clients table and packs it into a message to be sent back.
        /// </summary>
        /// <returns>A protobuf message object containing information about every Client in the database.</returns>
        private static AllClients SelectAllClients()
        {
            AllClients clients = new();
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT * FROM Clients;";
                command = new NpgsqlCommand(@query, conn);
                conn.Open();
                reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        var current = new ClientInfo
                        {
                            ClientId = Convert.ToInt32(reader["client_id"]),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            PhoneNumber = reader["phone_number"].ToString(),
                            Email = reader["email_address"].ToString()
                        };
                        clients.Clients.Add(current);
                    }
                }
                reader.Close();
                conn.Close();
            }
            return clients;
        }

        /// <summary>
        /// Implementation of the searchClientsByName RPC that retrieves all records of all Clients that fulfill a search pattern
        /// </summary>
        /// <param name="request">The phrase being used to search the Client table.</param>
        /// <param name="context"></param>
        /// <returns>An object of objects where each sub-object represents an entry in the search result of the Client table.</returns>
        public override Task<AllClients> searchClientsByName(ClientName request, ServerCallContext context)
        {
            return Task.FromResult(searchClients(request));
        }

        /// <summary>
        /// Method <c>searchClients</c> searches the database, given a phrase (first name), for records that are similar.
        /// </summary>
        /// <param name="name">The first name being used to perform the search.</param>
        /// <returns>A protobuf message containing records for each Client that fulfilled the search criteria.</returns>
        private static AllClients searchClients(ClientName name)
        {
            AllClients clients = new();
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string searchTerm = name.CName + "%";
            string query;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "SELECT * FROM Clients WHERE LOWER(CONCAT(first_name, ' ', last_name)) LIKE LOWER($1);";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = searchTerm}
                    }
                };
                conn.Open();
                reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        ClientInfo current = new()
                        {
                            ClientId = Convert.ToInt32(reader["client_id"]),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            PhoneNumber = reader["phone_number"].ToString(),
                            Email = reader["email_address"].ToString()
                        };
                        clients.Clients.Add(current);
                        Console.WriteLine(current.FirstName);
                    }
                }
                reader.Close();
                conn.Close();
            }
            return clients;
        }

        /// <summary>
        /// Implementation of the updateClient RPC that updates the information stored about a client (name, phone number, etc.).
        /// </summary>
        /// <param name="request">An object containing the (possibly) new information of the Client</param>
        /// <param name="context"></param>
        /// <returns>An object that states whether or not this operation was successful.</returns>
        public override Task<ServiceStatus> updateClient(ClientInfo request, ServerCallContext context)
        {
            return Task.FromResult(UpdateClient(request));
        }

        /// <summary>
        /// Method <c>UpdateClient</c> sends an UPDATE statement to the Clients table and updates information about the client such as their name, phone number and email address.
        /// </summary>
        /// <param name="newInfo">An object containing the new information of the Client.</param>
        /// <returns>An object that states whether or not this operation was successful.</returns>
        private static ServiceStatus UpdateClient(ClientInfo newInfo)
        {
            ServiceStatus sStatus = new();
            NpgsqlCommand command;
            string query;
            int status;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = "UPDATE Clients " +
                    "SET first_name = $1, " +
                    "last_name = $2, " +
                    "phone_number = $3, " +
                    "email_address = $4 " +
                    "WHERE client_id = $5;";
                command = new NpgsqlCommand(@query, conn)
                {
                    Parameters =
                    {
                        new() {Value = newInfo.FirstName},
                        new() {Value = newInfo.LastName},
                        new() {Value = newInfo.PhoneNumber},
                        new() {Value = newInfo.Email},
                    }
                };
                conn.Open();
                try
                {
                    status = command.ExecuteNonQuery();
                } catch (NpgsqlException pgE)
                {
                    status = 0;
                }
                conn.Close();

                if (status == 1)
                {
                    sStatus.IsSuccessfulOperation = true;
                    sStatus.StatusMessage = "Success: Client information updated.";
                }
                else
                {
                    sStatus.IsSuccessfulOperation = false;
                    sStatus.StatusMessage = "Error: Unable to update client information.";
                }
            }
            return sStatus;
        }
    }
}
