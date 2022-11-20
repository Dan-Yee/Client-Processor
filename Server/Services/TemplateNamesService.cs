﻿using DotNetEnv;
using Google.Protobuf;
using Grpc.Core;
using GrpcClient;
using GrpcServer.Protos;
using Npgsql;
using System;

namespace GrpcServer.Services
{
    public class TemplateNamesService : FormTemplateNames.FormTemplateNamesBase
    {
        private readonly ILogger<TemplateNamesService> _logger;

        public TemplateNamesService(ILogger<TemplateNamesService> logger)
        {
            _logger = logger;
        }


        public override async Task GetTemplateNames(TemplatesRequest request, IServerStreamWriter<TemplateName> responseStream, ServerCallContext context)
        {
            List<TemplateName> templates = GetTemplateNames();    

            foreach (TemplateName template in templates)
            {
                await responseStream.WriteAsync(template);
            }    
            //return null;
            //return base.GetTemplateNames(request, responseStream, context);
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
        /// Method that returns photos from the database based on the received request
        /// </summary>
        /// <param name="PID"></param> the procedure ID corresponding to the photos
        /// <param name="isBefore"></param> indicates whether the requested photos are taken
        /// before of after the procedure was don
        /// <returns>List</returns> a list of photos
        private static List<TemplateName> GetTemplateNames()
        {
            NpgsqlCommand command;
            NpgsqlDataReader reader;
            string query;

            //List to store files got from the database
            List<TemplateName> Templates = new List<TemplateName>();


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
                        //read the data and create a photo object
                        string TemplateName = (string)reader["filename"];
                        string TemplateExtension = (string)reader["file_extension"];

                        //add the photo to the List of photos
                        Templates.Add(new TemplateName { FormTemplateName = TemplateName, FormTemplateExtension = TemplateExtension });
                    }
                }
                reader.Close();
                conn.Close();
            }

            return Templates;
        }




    }
}
