using DotNetEnv;
using Google.Protobuf;
using Grpc.Core;
using GrpcClient;
using GrpcServer.Protos;
using Npgsql;
using Server;
using System;

namespace Server.Services
{
    public class TemplateNamesService : FormTemplateNames.FormTemplateNamesBase
    {
        private readonly ILogger<TemplateNamesService> _logger;

        public TemplateNamesService(ILogger<TemplateNamesService> logger)
        {
            _logger = logger;
        }



        public override Task<TemplatesResponse> GetTemplateNames(TemplatesRequest request, ServerCallContext context)
        {
            return Task.FromResult(GetTemplateNames());
        }


        /// <summary>
        /// Method that returns photos from the database based on the received request
        /// </summary>
        /// <param name="PID"></param> the procedure ID corresponding to the photos
        /// <param name="isBefore"></param> indicates whether the requested photos are taken
        /// before of after the procedure was don
        /// <returns>List</returns> a list of photos
        private static TemplatesResponse GetTemplateNames()
        {
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            //List to store files got from the database
            //List<TemplateName> Templates = new List<TemplateName>();
            TemplatesResponse Templates = new TemplatesResponse();

            using (NpgsqlConnection conn = GetConnection())
            {
                query = $"SELECT filename, file_extension FROM empty_forms";
                command = new NpgsqlCommand(@query, conn);
                conn.Open();
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string TemplateName = (string)reader["filename"];
                        string TemplateExtension = (string)reader["file_extension"];

                        Templates.TemplateNames.Add(new TemplateName { FormTemplateName = TemplateName, FormTemplateExtension = TemplateExtension });
                    }
                }
                reader.Close();
                conn.Close();
            }

            return Templates;
        }



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
        /// Method that Sends a form's bytes that was requested
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GetForm(FormRequest request, IServerStreamWriter<FormResponse> responseStream, ServerCallContext context)
        {
            byte[] FileBytes;
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = $"SELECT file_bytes FROM empty_forms WHERE filename = \'{request.TemplateName}\'";
                command = new NpgsqlCommand(@query, conn);
                conn.Open();
                //reader = command.ExecuteReader();

                try
                {
                    reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        await responseStream.WriteAsync(new FormResponse
                        {
                            Status = new ServiceStatus
                            {
                                IsSuccessfulOperation = false,
                                StatusMessage = $"There isn't any template with the name: {request.TemplateName}"
                            }
                        });

                    }

                    if (reader.Read())
                    {

                        //else
                        //{

                        FileBytes = (byte[])reader["file_bytes"];
                        int BytesToSend = FileBytes.Length;
                        int buffSize = 4000000;      // set buffer size to 4MB (ie. Max)
                        int sum = 0;                 // keep track of sent bytes


                        //if the number of bytes to send is less than 4MB
                        if (BytesToSend < buffSize)
                        {
                            buffSize = BytesToSend;// set the buffer's size to the number of bytes to send
                        }

                        //create a memory stream for simplicity
                        MemoryStream ms = new MemoryStream(FileBytes);

                        while (BytesToSend > 0) // Loop untill all the bytes are sent
                        {
                            byte[] buffer = new byte[buffSize];   //create buffer for sending the bytes
                            int n = ms.Read(buffer, 0, buffSize); //read into the buffer 

                            //send the chunks/buffer to the server
                            await responseStream.WriteAsync(new FormResponse { FormBytes = ByteString.CopyFrom(buffer) });
                            ///////await responseStream.WriteAsync(new SelectedFormResponse { FormBytes = ByteString.CopyFrom(buffer) });

                            Console.WriteLine($"DATA SENT!!!!!!!!: {n} bytes sent");

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
                        reader.Close();
                        conn.Close();
                    }
                }
                catch
                {
                    await responseStream.WriteAsync(new FormResponse
                    {
                        Status = new ServiceStatus
                        {
                            IsSuccessfulOperation = false,
                            StatusMessage = $"There isn't any template with the name: {request.TemplateName}"
                        }
                    });
                    conn.Close();
                }
            }


        }


        /// <summary>
        /// Method that deletes a template from the DataBase
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ServiceStatus> DeleteTemplate(FormRequest request, ServerCallContext context)
        {
            using (NpgsqlConnection conn = GetConnection())
            {

                conn.Open();
                string SQL = $"DELETE FROM Empty_forms WHERE filename = \'{request.TemplateName}\'";
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
                    Console.WriteLine("TemplateNameService.DeleteTemplate RPC Postgresql Error State: " + pgE.SqlState);
                    return Task.FromResult(new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "delete failed" });
                }
            }

            return Task.FromResult(new ServiceStatus { IsSuccessfulOperation = true, StatusMessage = "Template deleted" });

        }


    }
}
