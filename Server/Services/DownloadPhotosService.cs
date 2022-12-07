using DotNetEnv;
using Google.Protobuf;
using Grpc.Core;
using GrpcClient;
using GrpcServer.Protos;
using Npgsql;
using Server;
using System.Linq.Expressions;
using System.Text;
//using GrpcServer.classes;

namespace Server.Services
{
    public class DownloadPhotosService : PhotoDownload.PhotoDownloadBase
    {
        private readonly ILogger<DownloadPhotosService> _logger;

        public DownloadPhotosService(ILogger<DownloadPhotosService> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Method that gets and sends the photoes in chunks
        /// </summary>
        /// <param name="request"></param> the request from the client containing the photos info
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task PhotosDownload(PhotosRequest request, IServerStreamWriter<PhotoResponse> responseStream, ServerCallContext context)
        {
            //get the photos from the database
            List<Photo> photos = GetRecords(request.ProcedureID, request.IsBefore);

            //sending the photos
            if (photos.Count == 0)
            {
                await responseStream.WriteAsync(new PhotoResponse { Status = new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "" } });
            }
            else
            {
                //iterate over all the photos to send
                foreach (Photo photo in photos)
                {
                    await responseStream.WriteAsync(new PhotoResponse { NameAndExtention = new PhotoInfo { PhotoName = photo.PhotoName, PhotoExtension = photo.PhotoExtension } });

                    byte[] PhotoData = photo.PhotoBytes;


                    int BytesToSend = PhotoData.Length;
                    int buffSize = 4000000;      // set buffer size to 4MB (ie. Max)
                    int sum = 0;                 // keep track of sent bytes


                    //if the number of bytes to send is less than 4MB
                    if (BytesToSend < buffSize)
                    {
                        buffSize = BytesToSend;// set the buffer's size to the number of bytes to send
                    }

                    //create a memory stream for simplicity
                    MemoryStream ms = new MemoryStream(PhotoData);

                    while (BytesToSend > 0) // Loop untill all the bytes are sent
                    {
                        byte[] buffer = new byte[buffSize];   //create buffer for sending the bytes
                        int n = ms.Read(buffer, 0, buffSize); //read into the buffer 

                        //send the chunks/buffer to the server
                        await responseStream.WriteAsync(new PhotoResponse { PhotoBytes = ByteString.CopyFrom(buffer) });


                        //Console.WriteLine($"DATA SENT!!!!!!!!: {n} bytes sent");

                        if (n == 0)
                        {
                            break; //all bytes are sent
                        }

                        BytesToSend -= n; //update the number of bytes to send
                        sum += n;         //update the sum

                        if (BytesToSend < buffSize)
                        {
                            buffSize = BytesToSend; //decrese the size of the buffer if needed when the end is reached
                        }

                    }
                }
            }
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
        /// Method that returns photos from the database based on the received request
        /// </summary>
        /// <param name="PID"></param> the procedure ID corresponding to the photos
        /// <param name="isBefore"></param> indicates whether the requested photos are taken
        /// before of after the procedure was don
        /// <returns>List</returns> a list of photos
        private static List<Photo> GetRecords(int PID, bool isBefore)
        {
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            //List to store files got from the database
            List<Photo> photos = new List<Photo>();


            using (NpgsqlConnection conn = GetConnection())
            {
                query = $"SELECT photo_name, photo_extension, photo_bytes FROM procedure_photos" +
                    $" WHERE procedure_id = {PID} AND isBefore = {isBefore};";
                command = new NpgsqlCommand(@query, conn);
                conn.Open();
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //read the data and create a photo object
                        string PhotoName = (string)reader["photo_name"];
                        string PhotoExtension = (string)reader["photo_extension"];
                        byte[] data = (byte[])reader["photo_bytes"];

                        //add the photo to the List of photos
                        photos.Add(new Photo(PhotoName, PhotoExtension, data));
                    }
                }
                reader.Close();
                conn.Close();
            }

            return photos;
        }


        /// <summary>
        /// Method that deletes a photo from the database based on a made request
        /// </summary>
        /// <param name="request"></param> contains the info needed to perform the deletion
        /// <param name="context"></param> 
        /// <returns></returns>
        public override Task<ServiceStatus> PhotoDelete(DeleteRequest request, ServerCallContext context)
        {
            using (NpgsqlConnection conn = GetConnection())
            {

                conn.Open();
                string SQL = $"UPDATE procedure_photos SET isDeleted = true, last_edited_by = {request.EID}, last_edited_datetime = DEFAULT WHERE procedure_id = {request.PID} AND photo_name = \'{request.PhotoName}\'";
                NpgsqlCommand cmd = new NpgsqlCommand(SQL, conn);

                int n;
                try
                {
                    n = cmd.ExecuteNonQuery();
                    conn.Close();

                }
                catch (PostgresException pgE)
                {
                    conn.Close();
                    Console.WriteLine("DownloadPhotosService.PhotoDelete RPC Postgresql Error State: " + pgE.SqlState);
                    return Task.FromResult(new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "delete failed" });
                }
            }

            return Task.FromResult(new ServiceStatus { IsSuccessfulOperation = true, StatusMessage = "photo deleted" });
        }



    }
}
