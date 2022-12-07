using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using GrpcServer.Protos;
using iText.Kernel.XMP.Impl.XPath;
using ReactiveUI;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Threading;
using System;
using Avalonia.Media.Imaging;

namespace ClientApp.Views
{
    public partial class FormViewingView : ReactiveUserControl<FormViewingViewModel>
    {
        public static class Globals
        {
            public static string GName;
            public static string GExtent;
            public static byte[] GBytes;
        }
        public FormViewingView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            initializetherest();
            
        }
        public void initializetherest()
        {
            var client = new CompletedForm.CompletedFormClient(Program.gRPCChannel);
            var response = client.CompletedFormNames(new CompletedFormsRequest() { ProcedureID = ClientProcedureListingViewModel.Procedure_Id });
            foreach (var formInformation in response.FormInfo)
            {
                //Convert to pdf
                Globals.GName = formInformation.FormName;
                Globals.GExtent = formInformation.FormExtension;
                CallToCompleteForm(formInformation);

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
        public List<string> ImgPaths { get; set; } = new();
        public void ConvertPdfToImage()
        {

            string filePath = Globals.GName + Globals.GExtent;
            using (var pdfDocument = PdfiumViewer.PdfDocument.Load(@filePath))
            {

                for (int i = 0; i < pdfDocument.PageCount; i++)
                {
                    System.Drawing.Image bitmapImage = pdfDocument.Render(i, 300, 300, true);
                    bitmapImage.Save(@Globals.GName + i + ".bmp", ImageFormat.Bmp);
                    ImgPaths.Add(@Globals.GName + i + ".bmp");
                    Image img = new Image();
                    img.Height = 1000;
                    img.Width = 1000;
                    img.Margin = new(0,10);
                    img.Source = new Bitmap(Globals.GName + i + ".bmp");
                    this.FindControl<StackPanel>("ParentStackPanel").Children.Add(img);
                }
            }
        }

        public static async Task<byte[]> getCompletedForm(int FID)
        {
            var client = new CompletedForm.CompletedFormClient(Program.gRPCChannel);
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
            Globals.GBytes = ByteList.ToArray();
            return ByteList.ToArray();

        }

        public void byteArrayToPdf(byte[] byteArrayIn)
        {
            System.IO.File.WriteAllBytes(Globals.GName + Globals.GExtent, byteArrayIn);
        }
    }
}
