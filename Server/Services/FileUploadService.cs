using DotNetEnv;
using Grpc.Core;
using Npgsql;




namespace Server.Services
{
    public class FileUploadService : FileTransfer.FileTransferBase
    {
        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(ILogger<FileUploadService> logger)
        {
            _logger = logger;
        }


        //server gets upload request. receives the data ie client wants to upload to server
        public override async Task<ServiceStatus> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {

            int i = 0; //to check for the first message
            var ByteList = new List<byte>(); //create a List of bytes to store received bytes

            string? FileName = null;
            string? Extension = null;
            int ProcedureID = -2;
            int Destination = -2;

            bool success = false;

            while (await requestStream.MoveNext()) //loop while receiving data chunks
            {
                if (i == 0) // if this is the fist message received
                {
                    //verify that the message contains MetaData
                    if (requestStream.Current.RequestCase.Equals(UploadFileRequest.RequestOneofCase.Metadata))
                    {
                        FileName = requestStream.Current.Metadata.Filename;  // get the file's name 
                        Extension = requestStream.Current.Metadata.Extension;// get the file's extention
                        ProcedureID = requestStream.Current.Metadata.ProcedureID;
                        Destination = requestStream.Current.Metadata.StorageLocation;

                        Console.WriteLine($"File Name: {FileName} Extension: {Extension}");
                    }
                    else //throw an exception if the message didn't have MetaData
                    {
                        //throw new Exception("Meta Data Expexted!!");
                        return new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "UPLOAD FAILED! MetaData Expected!" };
                    }
                    i++;

                }
                else if (requestStream.Current.RequestCase.Equals(UploadFileRequest.RequestOneofCase.ChunkData))
                { //the current message has file chunks

                    //add the bytes received to the List of Bytes
                    ByteList.AddRange(requestStream.Current.ChunkData.ToByteArray());
                }
                else // throw an exception if this isn't the first message and isn't a file chunk
                {
                    //throw new Exception("UPLOAD FAILED!! Bytes of Data Expexted!!"); //return
                    return new ServiceStatus { IsSuccessfulOperation = success, StatusMessage = "" };//StringResponse { Message = "UPLOAD FAILED!!" };
                }
            }

            byte[] x = ByteList.ToArray(); // Byte array to store in database

            if((FileName == null) || (Extension == null) || (ProcedureID == -2) || (Destination == -2))
            {
                //return new StringResponse { Message = "UPLOAD FAILED!!" };
                return new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "UPLOAD FAILED! Invalid Arguments!" };
            }

            return InsertRecord(FileName, Extension, ProcedureID, Destination, x);
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


        /// <summary>
        /// Method <c>InsertRecord</c> Inserts received record to database.
        /// </summary>
        /// <returns>An instance of ServiceStatus that indicates if the process was successful or not with a short description</returns>
        private static ServiceStatus InsertRecord(string fNAme, string fExtension, int PID, int table, byte[] x)
        {

            ServiceStatus status = new ServiceStatus(); 

            using (NpgsqlConnection conn = GetConnection())
            {


                if (table == 0) //insert form template
                {
                    conn.Open();
                    string SQL = "insert into empty_forms values(:filename,:file_extension,:file_bytes)";
                    NpgsqlCommand cmd = new NpgsqlCommand(SQL);
                    cmd.Connection = conn;

                    cmd.Parameters.Add(new NpgsqlParameter(":filename", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters[0].Value = fNAme;

                    cmd.Parameters.Add(new NpgsqlParameter(":file_extension", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters[1].Value = $".{fExtension}";

                    cmd.Parameters.Add(new NpgsqlParameter(":file_bytes", NpgsqlTypes.NpgsqlDbType.Bytea));
                    cmd.Parameters[2].Value = x;

                    int n;


                    try
                    {
                        n = cmd.ExecuteNonQuery();
                    }
                    catch (PostgresException pgE)
                    {
                        if (pgE.SqlState.Equals("23505"))
                        {
                            status.IsSuccessfulOperation = false;
                            status.StatusMessage = "Error: FileName already exists!";
                            conn.Close();
                            return status;
                        }
                        else
                        {
                            status.IsSuccessfulOperation = false;
                            status.StatusMessage = "Error: File Insertion Failed!";
                            conn.Close();
                            return status;
                        }
                    }
                    conn.Close();


                    if (n == 1)
                    { 
                        status.IsSuccessfulOperation = true;
                        status.StatusMessage = "FILE UPLOADED SUCCESSFULLY!";
                        return status;
                    }
                    else 
                    {
                        status.IsSuccessfulOperation = false;
                        status.StatusMessage = "UPLOAD FAILED!!!";
                        return status;
                    }

                }
                else
                {
                    status.IsSuccessfulOperation = false;
                    status.StatusMessage = "UPLOAD FAILED!!! INVALID ARGUMENTS!!!";
                    return status;
                }

            }
        }

    }
}
