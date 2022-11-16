using Avalonia.Controls;
using ReactiveUI;
using System;
using ClientApp.Views;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using System.Collections.ObjectModel;
using Google.Protobuf;
using Grpc.Net.Client;
using System.IO;
using System.Reactive;

namespace ClientApp.ViewModels
{

    public class ImportFormViewModel : ReactiveObject, IRoutableViewModel
    {
        
        //global variables used to store the information corresponding to files
        private static List<byte[]> FilesInBytes = new List<byte[]>();
        private static string[] FileNames = new string[1];
        private static string[] FileExtentions = new string[1];
        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "ImportForm";

        public RoutingState RouterAdmimHomePageProcedure { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoToAdminHome { get; }

        /// <summary>
        /// Metheod that sends a file to the server in cunks(byte[])
        /// </summary>
        /// <param name="bytes"> The file to send in bytes </param>
        /// <param name="fName"> The name of the file we will be sending </param>
        /// <param name="fExtension"> The files Extension</param>
        /// <param name="PID"> The procedure ID corresponding to the file, if it's a template pass set to: -1</param>
        /// <param name="destination"> Indicates to the server the table where the file should be stored set to 0 if the file iss a template </param>
        /// <returns> Doesn't return </returns>
        /// <exception cref="Exception"></exception>
        private static async Task UploadFile(byte[] bytes, string fName, string fExtension, int PID, int destination)
        {

            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            ///var client = new FileTransfer.FileTransferClient(channel);
            var client = new FileTransfer.FileTransferClient(channel);
            var stream = client.UploadFile();

            if ((destination < 0) || (destination > 2))
            {
                throw new Exception("inavlid Storage Destination (TableID)");
            }

            //construct the MetaData message
            MetaData md = new MetaData();
            md.Filename = fName;
            md.Extension = fExtension;
            md.ProcedureID = PID;
            md.StorageLocation = destination;


            //send the MetaData
            await stream.RequestStream.WriteAsync(new UploadFileRequest { Metadata = md });

            //create a MemoryStream to facilitate sending the bytes
            MemoryStream ms = new MemoryStream(bytes);


            try
            {
                int length = (int)ms.Length; // get file length
                int buffSize = 4000000;      // set buffer size to 4MB (ie. Max)
                int sum = 0;                 // keep track of sent bytes


                int BytesToSend = (int)ms.Length;  //get the number of bytes to send

                //if the number of bytes to send is less than 4MB
                if (BytesToSend < buffSize)
                {
                    buffSize = BytesToSend;// set the buffer's size to the number of bytes to send
                }


                while (BytesToSend > 0) // Loop untill all the bytes are sent
                {
                    byte[] buffer = new byte[buffSize];   //create buffer for sending the bytes
                    int n = ms.Read(buffer, 0, buffSize); //read into the buffer 

                    //send the chunks/buffer to the server
                    await stream.RequestStream.WriteAsync(new UploadFileRequest { ChunkData = ByteString.CopyFrom(buffer) });

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

                await stream.RequestStream.CompleteAsync();           //let the server know we are done
                ServiceStatus response = await stream.ResponseAsync; //get the response from the server

                Console.WriteLine("Bytes to send: " + BytesToSend + " / Bytes sent: " + sum);

                Console.WriteLine("Server responce: " + response.StatusMessage);
            }
            finally
            {
                ms.Close();
            }
        }


        /// <summary>
        /// Sets the arguments for the FileUpload method
        /// </summary>
        /// <param name="FilePaths"> array with the paths of all files to send</param>
        /// <exception cref="Exception"></exception>
        private static void SetArgs(string[] FilePaths)
        {

            for (int i = 0; i < FilePaths.Length; i++)
            {
                string FileNameAndExtention = null;
                FileNameAndExtention = Path.GetFileName(FilePaths[i]);

                string[] FileNE = FileNameAndExtention.Split('.');
                if (FileNE.Length != 2)
                {
                    throw new Exception("INVALID FILE NAME!!");
                }

                FileNames[i] = FileNE[0];
                FileExtentions[i] = FileNE[1];
                byte[] FileBytes = File.ReadAllBytes(FilePaths[i]);
                FilesInBytes.Add(FileBytes);
            }

        }

        /// <summary>
        /// The value returned serves as a flag for the server to know where to store the file in the database
        /// </summary>
        /// <returns></returns>
        private static int FormTemplate()
        {
            return 0;
        }

        /// <summary>
        /// The value returned lets the server know that the form isn't related to any procedure
        /// </summary>
        /// <returns></returns>
        private static int NotProcedureForm()
        {
            return -1;
        }



        //View that this viewmodel is attached to
        ImportFormView _importFormView;
        //Holds the paths of the selected files
        private string _filePaths;
        
        public string FilePaths
        {
            get => _filePaths;
            set
            {
                this.RaiseAndSetIfChanged(ref _filePaths, value);
            }
        }
        /// <summary>
        /// Constructor for view model. Initializes view
        /// </summary>
        /// <param name="ifv"></param>
        public ImportFormViewModel()
        {
            GoToAdminHome = ReactiveCommand.CreateFromObservable(
            () => RouterAdmimHomePageProcedure.Navigate.Execute(new AdminHomeViewModel()));
            //_importFormView = ifv;
            FilePaths = "path";
        }

        /// <summary>
        /// Handles importing files
        /// </summary>
        public async void Files()
        {

            var wind = new Window();
            var ofd = new OpenFileDialog();
            // ofd.InitialDirectory = @"C:\Users";
            //ofd.AllowMultiple = true;
            var y = await ofd.ShowAsync(wind);
            if (y != null)
            {
                //FilePaths = "change";
                FilePaths = y[0];
                SetArgs(y);
                /*
                foreach (var path in y)
                {
                    Console.WriteLine(path);
                    var filename = System.IO.Path.GetFileName(path);

                }
                */
                //Console.WriteLine(y[0]);
            }
        }

        /// <summary>
        /// Onclick event for storing the file
        /// </summary>
        /// 

        public async void StoreFileCommand()
        {
           
            for (int i = 0; i < FileNames.Length; i++)
            {
                await UploadFile(FilesInBytes[i], FileNames[i], FileExtentions[i], NotProcedureForm(), FormTemplate());
            }


            //This brings you back to last page
            GoToAdminHome.Execute();
          
        }
    }
}
