using Avalonia.Native;
using ClientApp.Models;
using Grpc.Net.Client;
using GrpcServer.Protos;
using iText.Kernel.Pdf;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Org.BouncyCastle.Crypto.Paddings;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class FormViewingViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => "FormViewingViewModel";

        public IScreen HostScreen { get; }
        public RoutingState BackRouter { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoBack { get; }

        //public List<FormModel> ListOfForms { get; set; } = new();
        public List<byte[]> ListOfByteArrays { get; set; } = new();

        public static class Globals
        {
            public static string GName;
            public static string GExtent;
            public static byte[] GBytes;
        }
        public FormModel formModel { get; set; } = new();
        public FormViewingViewModel()
        {
            GoBack = ReactiveCommand.CreateFromObservable(
             () => BackRouter.Navigate.Execute(new ProcedureReadViewModel()));
            
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new CompletedForm.CompletedFormClient(channel);
            var response = client.CompletedFormNames(new CompletedFormsRequest() { ProcedureID = ClientProcedureListingViewModel.Procedure_Id });
            foreach(var formInformation in response.FormInfo)
            {
                //getCompletedForm(formInformation.FormID);
                //Convert to pdf
                Globals.GName = formInformation.FormName;
                Globals.GExtent = formInformation.FormExtension;
                formModel = new(formInformation.FormName, formInformation.FormExtension, new byte[0]);

                CallToCompleteForm(formInformation);


                break;
        
            }
        }
        
        public async void CallToCompleteForm(FormInfo formInformation)
        {
            await getCompletedForm(formInformation.FormID);
            if (Globals.GBytes != null && Globals.GBytes.Length > 0)
            {
                byteArrayToPdf(Globals.GBytes);
                ConvertPdfToImage();
            }
        }
        private global::PdfiumViewer.PdfViewer pdfViewer;
        public void ConvertPdfToImage()
        {
            
            string filePath = Globals.GName + Globals.GExtent;
            using (var pdfDocument = PdfiumViewer.PdfDocument.Load(@filePath))
            {
                var bitmapImage = pdfDocument.Render(0, 300, 300, true);
                bitmapImage.Save(@Globals.GName+".bmp", ImageFormat.Bmp);
            }
        }
        public string ImaPath => Globals.GName + ".bmp";

        public static async Task<byte[]> getCompletedForm(int FID)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new CompletedForm.CompletedFormClient(channel);
            var response = client.CompletedFormDownload(new SelectedFormRequest { FormID = FID });
            var ByteList = new List<byte>();

            //iterate while the server is still sending messages
            while (await response.ResponseStream.MoveNext(CancellationToken.None))
            {
                if (response.ResponseStream.Current.ResponseCase.Equals(SelectedFormResponse.ResponseOneofCase.FormBytes))
                {
                    ByteList.AddRange(response.ResponseStream.Current.FormBytes.ToByteArray());
                    Globals.GBytes = ByteList.ToArray();
                    return ByteList.ToArray();
                }
                else
                {
                    if (response.ResponseStream.Current.ResponseCase.Equals(SelectedFormResponse.ResponseOneofCase.Status))
                    {
                        Console.WriteLine(response.ResponseStream.Current.Status.StatusMessage);
                        Globals.GBytes = ByteList.ToArray();
                        return ByteList.ToArray();
                    }
                }
            }

            //TEST
            //using var writer = new BinaryWriter(File.OpenWrite(@$"C:/Users/moeme/499/SampleForm1.pdf"));
            //writer.Write(ByteList.ToArray());
            Globals.GBytes = ByteList.ToArray();
            return ByteList.ToArray();
            
        }

        public void byteArrayToPdf(byte[] byteArrayIn)
        {
            System.IO.File.WriteAllBytes(Globals.GName+ Globals.GExtent, byteArrayIn);
        }

        public void GoBackCommand()
        {
            GoBack.Execute();
        }
    }
}
