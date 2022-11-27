
using DotNetEnv;
using Grpc.Core;
using GrpcServer.Protos;
using Npgsql;
using Server;
//using Server;

namespace Server.Services
{
    public class UploadFileService : FileUpload.FileUploadBase
    {
        private readonly ILogger<UploadFileService> _logger;

        public UploadFileService(ILogger<UploadFileService> logger)
        {
            _logger = logger;
        }

        public override async Task<ServiceStatus> FileUpload(IAsyncStreamReader<UploadRequest> requestStream, ServerCallContext context)
        {
            int i = 0; //to check for the first message
            var ByteList = new List<byte>(); //create a List of bytes to store received bytes

            FileMetaData MetaData = new FileMetaData();//to store received metadata

            while (await requestStream.MoveNext()) //loop while receiving data chunks
            {
                if (i == 0) //if this is the first message received
                {
                    if (requestStream.Current.UploadMessageCase.Equals(UploadRequest.UploadMessageOneofCase.FileMeta))
                    {

                        if (requestStream.Current.FileMeta.FileMetaCase.Equals(FileMetaData.FileMetaOneofCase.PhotoMeta))
                        {
                            //storing the photo metadata recived
                            PhotoMeta PhotoMetaData = new PhotoMeta();
                            PhotoMetaData = requestStream.Current.FileMeta.PhotoMeta;
                            MetaData.PhotoMeta = PhotoMetaData;
                        }
                        else if (requestStream.Current.FileMeta.FileMetaCase.Equals(FileMetaData.FileMetaOneofCase.FormTemplateMeta))
                        {
                            //storing the Form Template metadata recived
                            FormTemplateMeta FormTemplateMetaData = new FormTemplateMeta();
                            FormTemplateMetaData = requestStream.Current.FileMeta.FormTemplateMeta;
                            MetaData.FormTemplateMeta = FormTemplateMetaData;
                        }

                    }
                    else // return an error message if this isn't metadata
                    {
                        return new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "Upload Failed!!" };
                    }
                    i++;//increment to know that the next message should contain file chunks
                }
                else if (requestStream.Current.UploadMessageCase.Equals(UploadRequest.UploadMessageOneofCase.FileChunk))
                {   //the current message has file chunks
                    //add the bytes received to the List of Bytes
                    ByteList.AddRange(requestStream.Current.FileChunk.ToByteArray());
                }
                else // return an error message if this isn't the first message and isn't a file chunk
                {
                    return new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "Upload Failed!!" };
                }

            }//end while

            byte[] x = ByteList.ToArray(); // Byte array to store in database

            return InsertRecord(MetaData, x);
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
        /// Method that inserts form templates and photos to their specified table
        /// </summary>
        /// <param name="Meta"></param> metadata of the file or photo
        /// <param name="FileContent"></param> the file or photo in form of an array of bytes
        /// <returns></returns>
        private static ServiceStatus InsertRecord(FileMetaData Meta, byte[] FileContent)
        {
            //variables used in executing the SQL statement
            int n;
            string SQL = null;
            NpgsqlCommand cmd;

            ServiceStatus status = new ServiceStatus(); //to return

            using (NpgsqlConnection conn = GetConnection())
            {
                if (Meta.FileMetaCase.Equals(FileMetaData.FileMetaOneofCase.FormTemplateMeta)) //insert form template
                {
                    conn.Open();
                    SQL = "insert into empty_forms values(:filename,:file_extension,:file_bytes)";
                    cmd = new NpgsqlCommand(SQL);
                    cmd.Connection = conn;

                    cmd.Parameters.Add(new NpgsqlParameter(":filename", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters[0].Value = Meta.FormTemplateMeta.FormName;

                    cmd.Parameters.Add(new NpgsqlParameter(":file_extension", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters[1].Value = $".{Meta.FormTemplateMeta.FormExtension}";

                    cmd.Parameters.Add(new NpgsqlParameter(":file_bytes", NpgsqlTypes.NpgsqlDbType.Bytea));
                    cmd.Parameters[2].Value = FileContent;

                }
                else if (Meta.FileMetaCase.Equals(FileMetaData.FileMetaOneofCase.PhotoMeta)) //insert photo
                {
                    conn.Open();
                    //crafting the SQL command to inser the record
                    SQL = "insert into procedure_photos values(:procedure_id,:photo_name,:photo_extension,:isbefore,:photo_bytes)";
                    cmd = new NpgsqlCommand(SQL);
                    cmd.Connection = conn;

                    cmd.Parameters.Add(new NpgsqlParameter(":procedure_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    cmd.Parameters[0].Value = (int)Meta.PhotoMeta.ProcedureID;

                    cmd.Parameters.Add(new NpgsqlParameter(":photo_name", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters[1].Value = Meta.PhotoMeta.PhotoName;

                    cmd.Parameters.Add(new NpgsqlParameter(":photo_extension", NpgsqlTypes.NpgsqlDbType.Text));
                    cmd.Parameters[2].Value = Meta.PhotoMeta.PhotoExtension;

                    cmd.Parameters.Add(new NpgsqlParameter(":isbefore", NpgsqlTypes.NpgsqlDbType.Boolean));
                    cmd.Parameters[3].Value = Meta.PhotoMeta.IsBefore;

                    cmd.Parameters.Add(new NpgsqlParameter(":photo_bytes", NpgsqlTypes.NpgsqlDbType.Bytea));
                    cmd.Parameters[4].Value = FileContent;

                }
                else
                {
                    status.IsSuccessfulOperation = false;
                    status.StatusMessage = "UPLOAD FAILED!!! INVALID ARGUMENTS!!!";
                    return status;
                }

                //Try to execute the SQL Command
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
                    else if (pgE.SqlState.Equals("23503"))
                    {
                        status.IsSuccessfulOperation = false;
                        status.StatusMessage = $"Error: Procedure #{(int)Meta.PhotoMeta.ProcedureID} Doesn't Exist!";
                        conn.Close();
                        return status;
                    }
                    else
                    {
                        Console.WriteLine(pgE.Message);
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
        }

    }
}

