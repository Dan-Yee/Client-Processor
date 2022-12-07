using DotNetEnv;
using Google.Protobuf;
using Grpc.Core;
using GrpcClient;
using GrpcServer.Protos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.SqlServer.Server;
using Npgsql;
using Server;


namespace Server.Services
{
    public class CompletedFormService : CompletedForm.CompletedFormBase
    {
        private readonly ILogger<CompletedFormService> _logger;

        public CompletedFormService(ILogger<CompletedFormService> logger)
        {
            _logger = logger;
        }


        //*************************************************************************
        public override async Task /*Task<ServiceStatus>*/ CompletedFormDownload(SelectedFormRequest request, IServerStreamWriter<SelectedFormResponse> responseStream, ServerCallContext context)
        {
            byte[] FileBytes;
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            using (NpgsqlConnection conn = GetConnection())
            {
                query = $"SELECT file_bytes FROM procedure_forms WHERE form_id = {request.FormID}";
                command = new NpgsqlCommand(@query, conn);
                conn.Open();
                //reader = command.ExecuteReader();

                try
                {
                    reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        await responseStream.WriteAsync(new SelectedFormResponse
                        {
                            Status = new ServiceStatus
                            {
                                IsSuccessfulOperation = false,
                                StatusMessage = "No such file"
                            }
                        });

                        return;
                        //return (new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "Form doesn't exist" });
                    }

                    //////else /* if (reader.HasRows)*/
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
                            //await responseStream.WriteAsync(new PhotoResponse { PhotoBytes = ByteString.CopyFrom(buffer) });
                            await responseStream.WriteAsync(new SelectedFormResponse { FormBytes = ByteString.CopyFrom(buffer) });

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
                    await responseStream.WriteAsync(new SelectedFormResponse
                    {
                        Status = new ServiceStatus
                        {
                            IsSuccessfulOperation = false,
                            StatusMessage = "No such file"
                        }
                    });
                    //reader.Close();
                    conn.Close();
                }
            }
        }

        //****************************************GET FORM NAMES OF A PROCEDURE ******************************

        public override Task<CompletedFormsResponse> CompletedFormNames(CompletedFormsRequest request, ServerCallContext context)
        {
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            //List to store files got from the database
            CompletedFormsResponse Forms = new CompletedFormsResponse();

            using (NpgsqlConnection conn = GetConnection())
            {
                query = $"SELECT form_id, filename, file_extension FROM procedure_forms WHERE procedure_id = {request.ProcedureID}";
                command = new NpgsqlCommand(@query, conn);
                conn.Open();
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int FormID = (int)reader["form_id"];
                        string FormName = (string)reader["filename"];
                        string FormExtension = (string)reader["file_extension"];

                        Forms.FormInfo.Add(new FormInfo { FormID = FormID, FormExtension = FormExtension, FormName = FormName });

                    }
                }
                reader.Close();
                conn.Close();
            }

            return Task.FromResult(Forms);

            //return Task.FromResult(GetFormNames(request.ProcedureID));
        }
        //**************************************** END GET FORM NAMES OF A PROCEDURE ******************************




        //=======================================DELETE SPECIFIC FORM ==========================================

        public override Task<ServiceStatus> DeleteForm(SelectedFormRequest request, ServerCallContext context)
        {

            using (NpgsqlConnection conn = GetConnection())
            {

                conn.Open();
                //$"UPDATE procedure_photos SET isDeleted = true, last_edited_by = {request.EID}, last_edited_datetime = DEFAULT WHERE procedure_id = {request.PID} AND photo_name = \'{request.PhotoName}\'";
                string SQL = $"UPDATE procedure_forms SET isDeleted = true, last_edited_by = {request.EID}, last_edited_datetime = DEFAULT WHERE form_id = {request.FormID}";
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
                    Console.WriteLine("CompletedFormService.DeleteForm RPC Postgresql Error State: " + pgE.SqlState);
                    return Task.FromResult(new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "delete failed" });
                }

                if (n == 0)
                {
                    return Task.FromResult(new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "delete failed" });
                }
            }

            //conn.Close();
            return Task.FromResult(new ServiceStatus { IsSuccessfulOperation = true, StatusMessage = "form deleted" });
            //return base.DeleteForm(request, context);
        }

        //=======================================END DELETE SPECIFIC FORM ==========================================



        //////////////////////////////////////////STORING COMPLETED FORMS//////////////////////////////////////////////////////////

        private static void InsertRecord(int PID, string FName, string FExtension, byte[] x)
        {
            //bool success = false;

            ServiceStatus status = new ServiceStatus();

            using (NpgsqlConnection conn = GetConnection())
            {

                conn.Open();
                string SQL = "insert into procedure_forms (procedure_id, filename, file_extension, file_bytes) values (:procedure_id,:filename,:file_extension,:file_bytes)";
                NpgsqlCommand cmd = new NpgsqlCommand(SQL);
                cmd.Connection = conn;

                cmd.Parameters.Add(new NpgsqlParameter(":procedure_id", NpgsqlTypes.NpgsqlDbType.Integer));
                cmd.Parameters[0].Value = PID;

                cmd.Parameters.Add(new NpgsqlParameter(":filename", NpgsqlTypes.NpgsqlDbType.Text));
                cmd.Parameters[1].Value = FName;

                cmd.Parameters.Add(new NpgsqlParameter(":file_extension", NpgsqlTypes.NpgsqlDbType.Text));
                cmd.Parameters[2].Value = FExtension;

                cmd.Parameters.Add(new NpgsqlParameter(":file_bytes", NpgsqlTypes.NpgsqlDbType.Bytea));
                cmd.Parameters[3].Value = x;

                int n;
                try
                {
                    n = cmd.ExecuteNonQuery();
                }
                catch (PostgresException pgE)
                {
                    Console.WriteLine("CompletedFormService.InsertRecord Method Postgresql Error State: " + pgE.SqlState);
                }
            }
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

    }
}