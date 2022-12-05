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

        public FormViewingViewModel()
        {
            GoBack = ReactiveCommand.CreateFromObservable(
             () => BackRouter.Navigate.Execute(new ProcedureReadViewModel()));
        }
        
        public void GoBackCommand()
        {
            GoBack.Execute();
        }
    }
}
