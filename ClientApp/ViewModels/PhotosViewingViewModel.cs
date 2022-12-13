using GrpcClient;
using GrpcServer.Protos;
//using GrpcServer.Protos;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive;
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

        public static int ProcedureID { get; set; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoBack { get; }
        public PhotosViewingViewModel()
        {
            ProcedureID = ClientProcedureListingViewModel.Procedure_Id;

            GoBack = ReactiveCommand.CreateFromObservable(
             () => BackRouter.Navigate.Execute(new ProcedureReadViewModel()));
            ImageBefore.Clear();
            ImageAfter.Clear();
            CallToDownloadPhotos();
            
        }

        public async static void CallToDownloadPhotos()
        {
            //procedure ID is now implented will show the after photo
            await PhotosViewingViewModel.DownloadPhoto(ProcedureID, false);
           
            await PhotosViewingViewModel.DownloadPhoto2(ProcedureID, true);
           
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
            var client = new PhotoDownload.PhotoDownloadClient(Program.gRPCChannel);
            var response = client.PhotosDownload(new PhotosRequest() { ProcedureID = PID, IsBefore = IsBefore });

            String name = string.Empty;
            String extension = string.Empty;
            var ByteList = new List<byte>();
            int i = 0;//counter to keep track of first message

            List<Photo> photos = new();//list to hold photos received from the server

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
                        extension = response.ResponseStream.Current.NameAndExtention.PhotoExtension;
                        i++;
                    }
                    else //if not first save the received bytes(ie. previous file was fully sent)
                    {
                        if ((!String.IsNullOrEmpty(name)) && (!String.IsNullOrEmpty(extension)))
                        {
                            //create a new photo object and add it to the List
                            photos.Add(new Photo(name, extension, ByteList.ToArray()));

                            //clear the variables
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
            if (ByteList.ToArray().Length != 0)
            {
                photos.Add(new Photo(name, extension, ByteList.ToArray()));
            }

            foreach (Photo x in photos)
            {
                await ByteArrayToImage(x.PhotoBytes.ToArray(), x.PhotoName, x.PhotoExtension);

            }

            foreach (Photo x in photos)
            {

                var Img = new PhotoViewAfterModel(x.PhotoName, x.PhotoExtension);
                ImageAfter.Add(Img);

            }


            return photos;
            //return new ServiceStatus { IsSuccessfulOperation = true, StatusMessage ="Files Downloaded!!!"};
        }

        /// <summary>
        /// Method that sends a request to the server to get Procedure photos and listens for responses
        /// to create photo objects from the messags received
        /// </summary>
        /// <param name="PID"></param> procedure ID of the pictures requested
        /// <param name="IsBefore"></param> indicates whether the requested photos are the befor or after photos 
        /// <returns></returns>
        private static async Task<List<Photo>> DownloadPhoto2(int PID, bool IsBefore)
        {
            var client = new PhotoDownload.PhotoDownloadClient(Program.gRPCChannel);
            var response = client.PhotosDownload(new PhotosRequest { ProcedureID = PID, IsBefore = IsBefore });

            String name = string.Empty;
            String extension = string.Empty;
            var ByteList = new List<byte>();
            int i = 0;//counter to keep track of first message

            List<Photo> photos = new();//list to hold photos received from the server

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
                        extension = response.ResponseStream.Current.NameAndExtention.PhotoExtension;
                        i++;
                    }
                    else //if not first save the received bytes(ie. previous file was fully sent)
                    {
                        if ((!String.IsNullOrEmpty(name)) && (!String.IsNullOrEmpty(extension)))
                        {
                            //create a new photo object and add it to the List
                            photos.Add(new Photo(name, extension, ByteList.ToArray()));

                            //clear the variables
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
            if (ByteList.ToArray().Length != 0)
            {
                photos.Add(new Photo(name, extension, ByteList.ToArray()));
            }
            

            foreach (Photo x in photos)
            {
                await ByteArrayToImage2(x.PhotoBytes.ToArray(),x.PhotoName,x.PhotoExtension);

            }

            foreach (Photo x in photos)
            {
               
                var Img = new PhotoViewBeforeModel(x.PhotoName, x.PhotoExtension);
                ImageBefore.Add(Img);

            }

                return photos;

           
                //return new ServiceStatus { IsSuccessfulOperation = true, StatusMessage ="Files Downloaded!!!"};
            }






        /// <summary>
        /// takes a byte array and converts it to an image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        public static async Task<int> ByteArrayToImage(byte[] byteArrayIn, string Name, string Exte)
        {
            using (Image image = Image.FromStream(new MemoryStream(byteArrayIn)))
            {
                image.Save(Name + Exte, ImageFormat.Png);  // Or Png
            }
            return 0;
        }


        public static async Task<int> ByteArrayToImage2(byte[] byteArrayIn,string Name,string Exte)
        {
            using (Image image = Image.FromStream(new MemoryStream(byteArrayIn)))
            {
                image.Save(Name + Exte, ImageFormat.Png);  // Or Png
            }
            return 0;

        }
        /// <summary>
        /// Binding to display image uses the globals to determine name and extension
        /// </summary>
        //public string ImaPath => Globals.GName + Globals.GExtent;

        // public string ImaPath2 => Globals.GName2 + Globals.GExtent2;

        public static ObservableCollection<PhotoViewBeforeModel> ImageBefore { get; } = new();

        public static ObservableCollection<PhotoViewAfterModel> ImageAfter { get; } = new();

        public void GoBackCommand()
        {
            GoBack.Execute();
        }
    }
}
