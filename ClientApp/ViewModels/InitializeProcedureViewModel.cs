using ClientApp.Views;
using Grpc.Net.Client;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class InitializeProcedureViewModel
    {

        InitializeProcedureView _initializeProcedureView;
        int _clientID;
        public static int CurrentProcedureID { get; set; }

        public string _ProcedureName { get; set; }
        public string _ProcedureDescription { get; set; }

        public InitializeProcedureViewModel(InitializeProcedureView ipv, int c_id)
        {
            _initializeProcedureView = ipv;
            _clientID = c_id;
        }

        public void ProceedToMakeProcedure()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
            var RPCProcedure= new Procedure.ProcedureClient(channel);
            //LoginPageViewModel.
            ProcedureInfo i = new()
            {
                ClientID = _clientID,
                EmployeeID = LoginPageViewModel.GlobalEmployeeID,
                ProcedureName = _ProcedureName,
                ProcedureNotes = _ProcedureDescription
            };
            ProcedureID pid = RPCProcedure.addProcedure(i);
            CurrentProcedureID = pid.PID;
            new MakeProcedureView(_clientID).Show();
            _initializeProcedureView.Close();
        }
    }
}
