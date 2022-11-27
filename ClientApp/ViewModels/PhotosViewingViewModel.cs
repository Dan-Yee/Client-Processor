using Avalonia.Controls;
using Google.Protobuf;
using Grpc.Net.Client;
using GrpcClient;
using GrpcServer.Protos;
using Microsoft.AspNetCore.Components.Routing;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Image = System.Drawing.Image;

namespace ClientApp.ViewModels
{
    public class PhotosViewingViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => "PhotosViewingViewModel";

        public IScreen HostScreen { get; }
        
        public RoutingState BackRouter { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoBack { get; }
        public static class Globals
        {
            public static string GName;
            public static string GExtent;
            public static byte[] GBytes;
        }
        public PhotosViewingViewModel()
        {
            GoBack = ReactiveCommand.CreateFromObservable(
             () => BackRouter.Navigate.Execute(new ProcedureReadViewModel()));
            CallToDownloadPhotos();
            
        }

        public async void CallToDownloadPhotos()
        {
            await PhotosViewingViewModel.DownloadPhoto(5, false);
            if(Globals.GBytes != null && Globals.GBytes.Length > 0)
            {
                //loads the byte array into the image function
                byteArrayToImage(Globals.GBytes);
            }
        }

        /// <summary>
        /// Method that sends a request to the server to get Procedure photos and listens for responses
        /// to create photo objects from the messags received
        /// </summary>
        /// <param name="PID"></param> procedure ID of the pictures requested
        /// <param name="IsBefore"></param> indicates whether the requested photos are the befor or after photos 
        /// <returns></returns>
        private static async Task<List<Photo>> DownloadPhoto(int PID, bool IsBefore)
        {
           
            //connect to server
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            //var client = new FileUpload.FileUploadClient(channel);
            var client = new PhotoDownload.PhotoDownloadClient(channel);
            var response = client.PhotosDownload(new PhotosRequest { ProcedureID = PID, IsBefore = IsBefore });

            String name = String.Empty;
            String extension = String.Empty;
            var ByteList = new List<byte>();
            int i = 0;//counter to keep track of first message

            List<Photo> photos = new List<Photo>();//list to hold photos received from the server

            //iterate while the server is still sending messages
            while (await response.ResponseStream.MoveNext(CancellationToken.None))
            {
                if (response.ResponseStream.Current.PhotoDownloadCase.Equals(PhotoResponse.PhotoDownloadOneofCase.Status))
                {
                    //Console.WriteLine("STATUS");
                    //return response.ResponseStream.Current.Status;
                }
                else if (response.ResponseStream.Current.PhotoDownloadCase.Equals(PhotoResponse.PhotoDownloadOneofCase.NameAndExtention))
                {
                    //if first save the name and extention
                    if (i == 0)
                    {
                        name = response.ResponseStream.Current.NameAndExtention.PhotoName;
                        Globals.GName = name;
                        extension = response.ResponseStream.Current.NameAndExtention.PhotoExtension;
                        Globals.GExtent = extension;
                        i++;
                    }
                    else //if not first save the received bytes(ie. previous file was fully sent)
                    {
                        if ((!String.IsNullOrEmpty(name)) && (!String.IsNullOrEmpty(extension)))
                        {
                            //create a new photo object and add it to the List
                            photos.Add(new Photo(name, extension, ByteList.ToArray()));

                            //clear the variables
                            name = String.Empty;
                            extension = String.Empty;
                            ByteList.Clear();

                            //set the variables to the new values
                            name = response.ResponseStream.Current.NameAndExtention.PhotoName;
                            extension = response.ResponseStream.Current.NameAndExtention.PhotoExtension;

                        }
                        else
                        {
                            //return new ServiceStatus { IsSuccessfulOperation = false, StatusMessage = "Photo info not received"};  
                        }


                    }

                }
                else if (response.ResponseStream.Current.PhotoDownloadCase.Equals(PhotoResponse.PhotoDownloadOneofCase.PhotoBytes))
                {
                    //add the bytes
                    ByteList.AddRange(response.ResponseStream.Current.PhotoBytes.ToByteArray());
                   
                }

            }

            //the last photo bytes were sent
            //create the last file
            photos.Add(new Photo(name, extension, ByteList.ToArray()));
            Globals.GBytes = ByteList.ToArray();
            
            
            return photos;
            //return new ServiceStatus { IsSuccessfulOperation = true, StatusMessage ="Files Downloaded!!!"};
        }
        /// <summary>
        /// takes a byte array and converts it to an image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        public void byteArrayToImage(byte[] byteArrayIn)
        {
            using (Image image = Image.FromStream(new MemoryStream(byteArrayIn)))
            {
                image.Save(Globals.GName + Globals.GExtent, ImageFormat.Png);  // Or Png
            }
        
        }
        /// <summary>
        /// Binding to display image uses the globals to determine name and extension
        /// </summary>
        public string ImaPath => Globals.GName + Globals.GExtent;




        public void GoBackCommand()
        {
            GoBack.Execute();
        }
    }
}
