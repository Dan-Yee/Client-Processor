using Avalonia.Controls;
using Avalonia.Controls.Selection;
using ClientApp.Models;
using ClientApp.Views;
using Google.Protobuf;
using Grpc.Net.Client;
using GrpcServer.Protos;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class MakeAProcedureViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => throw new NotImplementedException();
        
        public IScreen HostScreen { get; }

        private ObservableCollection<FormModel> _formTemplateList = new();
        public ObservableCollection<FormModel> FormTemplateList
        {
            get => _formTemplateList;
            set
            {
                this.RaiseAndSetIfChanged(ref _formTemplateList, value);
            }
        }

        public ObservableCollection<FormModel> _currentlySelectedForms = new();
        public ObservableCollection<FormModel> CurrentlySelectedForms
        {
            get => _currentlySelectedForms;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentlySelectedForms, value);
            }
        }

        public SelectionModel<FormModel> FormTemplateSelection { get; } = new SelectionModel<FormModel>();
        public SelectionModel<FormModel> CurrentFormSelection { get; } = new SelectionModel<FormModel>();

        //public List<FormInputField> InputFields { get; } = new List<FormInputField>();

        public List<FormInputField> _inputList = new List<FormInputField>();
        public List<FormInputField> InputFields
        {
            get => _inputList;
            set
            {
                this.RaiseAndSetIfChanged(ref _inputList, value);
            }
        }

        public RoutingState RouterToFormMenu { get; } = new RoutingState();
        public RoutingState RouterToProcedureListing { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToFormMenu { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToProcedureListing { get; }

        public static int ProcedureID { get; set; }

        public MakeAProcedureViewModel()
        {
            ProcedureID = InitializeProcedureViewModel.CurrentProcedureID;

            NavigateToFormMenu = ReactiveCommand.CreateFromObservable(
                () => RouterToFormMenu.Navigate.Execute(new FormMenuViewModel()));
            NavigateToProcedureListing = ReactiveCommand.CreateFromObservable(
                () => RouterToProcedureListing.Navigate.Execute(new ClientProcedureListingViewModel()));
            TemplatesResponse templates = GetTemplateNames();


            foreach (var template in templates.TemplateNames)
            {
                FormTemplateList.Add(new FormModel(template.FormTemplateName, null, null));

            }

        }
        public static TemplatesResponse GetTemplateNames()
        {
            TemplatesResponse templates = new TemplatesResponse();

            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new FormTemplateNames.FormTemplateNamesClient(channel);
            templates = client.GetTemplateNames(new TemplatesRequest { });

            return templates;
        }

        public void GoToFormMenu()
        {
            NavigateToFormMenu.Execute();
        }
        public void BackToListingCommand()
        {
            NavigateToProcedureListing.Execute();
        }

        public void SelectFormTemplate()
        {
            _currentlySelectedForms.Add(FormTemplateSelection.SelectedItem);
        }

        //=============================CREATE.MESSAGE_FOR.PHOTOS===============================

        /// <summary>
        /// Method used to upload photos and create the corresponding metadata
        /// </summary>
        /// <param name="paths"></param> that contains all the paths of the phhotos to send
        /// <param name="IsBefore"></param> specifies if the photos are pre or post procedure photos
        /// <param name="PID"></param> specifies the procedureID corresponding to the photos
        /// <returns></returns>
        public static async Task UploadPhoto(string[] paths, bool IsBefore, int PID)
        {

            for (int i = 0; i < paths.Length; i++)
            {
                //creating the grpc request message
                string FileName = Path.GetFileNameWithoutExtension(paths[i]);
                string FileExtension = Path.GetExtension(paths[i]);
                byte[] FileBytes = File.ReadAllBytes(paths[i]);


                PhotoMeta PhotoMetaData = new PhotoMeta();
                PhotoMetaData.PhotoName = FileName;
                PhotoMetaData.PhotoExtension = FileExtension;
                PhotoMetaData.IsBefore = IsBefore;
                PhotoMetaData.ProcedureID = PID;

                FileMetaData MetaData = new FileMetaData();
                MetaData.PhotoMeta = PhotoMetaData;

                //call the method that sends the files to the server
                await UploadFile(FileBytes, MetaData);
            }
        }

        //*****************************************UPLOAD_FORMS_AND_PHOTOS*************************************************

        /// <summary>
        /// Method that sends a file to the server in chunks(byte[])
        /// </summary>
        /// <param name="bytes"> The file to send in bytes </param>
        /// <param name="fName"> The name of the file we will be sending </param>
        /// <param name="fExtension"> The files Extension</param>
        /// <param name="PID"> The procedure ID corresponding to the file, if it's a template pass set to: -1</param>
        /// <param name="destination"> Indicates to the server the table where the file should be stored set to 0 if the file iss a template </param>
        /// <returns> Doesn't return </returns>
        /// <exception cref="Exception"></exception>
        private static async Task UploadFile(byte[] bytes, FileMetaData MetaData)
        {
            //connect to server
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new FileUpload.FileUploadClient(channel);
            var stream = client.FileUpload();

            //send the MetaData
            await stream.RequestStream.WriteAsync(new UploadRequest { FileMeta = MetaData });

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
                    await stream.RequestStream.WriteAsync(new UploadRequest { FileChunk = ByteString.CopyFrom(buffer) });

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
        /// open file dialog to grab the before pic
        /// </summary>
        public async void Before()
        {


            var wind = new Window();
            var ofd = new OpenFileDialog();
            // ofd.InitialDirectory = @"C:\Users";
            //ofd.AllowMultiple = true;
            //ofd.Filters = pdf;
            var y = await ofd.ShowAsync(wind);
            if (y != null)
            {
                //need to update PID to match the person to uplaod photos
                // 0 is a place holder until it is tied to the PID
                await UploadPhoto(y, true, ProcedureID);
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
        /// opens a file dialog to uplaod the after
        /// </summary>
        public async void After()
        {
            
            var wind = new Window();
            var ofd = new OpenFileDialog();
            // ofd.InitialDirectory = @"C:\Users";
            //ofd.AllowMultiple = true;
            //ofd.Filters = pdf;
            var y = await ofd.ShowAsync(wind);
            if (y != null)
            {
                //need to update PID to match the person to uplaod photos
                // 0 is a place holder until it is tied to the PID
                await UploadPhoto(y, false, ProcedureID);
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
    }
}
