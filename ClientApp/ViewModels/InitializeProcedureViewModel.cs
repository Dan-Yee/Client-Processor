using Grpc.Net.Client;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class InitializeProcedureViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();

        public string _ProcedureName { get; set; }
        public string _ProcedureDescription { get; set; }
        public static int CurrentProcedureID { get; set; }

        public RoutingState RouterToMakeProcedure { get; } = new RoutingState();
        public RoutingState RouterToClientProcedureListing { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToMakeProcedure { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToClientProcedureListing { get; }

        public InitializeProcedureViewModel()
        {
            NavigateToMakeProcedure = ReactiveCommand.CreateFromObservable(
            () => RouterToMakeProcedure.Navigate.Execute(new MakeAProcedureViewModel()));
            NavigateToClientProcedureListing = ReactiveCommand.CreateFromObservable(
                () => RouterToClientProcedureListing.Navigate.Execute(new ClientProcedureListingViewModel()));
        }
        public void ProceedToMakeProcedure()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
            var RPCProcedure = new Procedure.ProcedureClient(channel);
            //LoginPageViewModel.
            ProcedureInfo i = new()
            {
                ClientID = HomePageViewModel.Client_ID,
                EmployeeID = LoginPageViewModel.GlobalEmployeeID,
                ProcedureName = _ProcedureName,
                ProcedureNotes = _ProcedureDescription
            };
            ProcedureID pid = RPCProcedure.addProcedure(i);
            ClientProcedureListingViewModel.Procedure_Id = pid.PID;
            NavigateToMakeProcedure.Execute();
        }

        public void BackCommand()
        {
            NavigateToClientProcedureListing.Execute();
        }
    }
}
