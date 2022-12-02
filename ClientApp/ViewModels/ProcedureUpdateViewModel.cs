using Grpc.Net.Client;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using static Server.Procedure;

namespace ClientApp.ViewModels
{
    public class ProcedureUpdateViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen { get; }

        public string _ProcedureName { get; set; }
        public string _ProcedureDescription { get; set; }

        public RoutingState RouterToProcedureListing { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToProcedureListing { get; }

        public ProcedureUpdateViewModel()
        {
            _ProcedureName = ClientProcedureListingViewModel.ProcedureName;
            _ProcedureDescription = ClientProcedureListingViewModel.ProcedureNotes;
            NavigateToProcedureListing = ReactiveCommand.CreateFromObservable(
              () => RouterToProcedureListing.Navigate.Execute(new ClientProcedureListingViewModel()));
        }

        public void UpdateProcedureCommand()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
            var RPCProcedure = new Procedure.ProcedureClient(channel);
            ProcedureInfo newInfo = new()
            {
                ProcedureID = ClientProcedureListingViewModel.Procedure_Id,
                ProcedureName = _ProcedureName,
                ProcedureNotes = _ProcedureDescription,
                EmployeeID = LoginPageViewModel.GlobalEmployeeID
            };
            ServiceStatus status = RPCProcedure.updateProcedure(newInfo);
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", status.ToString()).Show();
            //Route back
            NavigateToProcedureListing.Execute();
        }

        public void BackCommand()
        {
            //Route back
            NavigateToProcedureListing.Execute();
        }
    }
}
