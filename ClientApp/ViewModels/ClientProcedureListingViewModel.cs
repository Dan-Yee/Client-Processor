using Avalonia.Controls.Selection;
using AvaloniaEdit.Editing;
using ClientApp.Models;
using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Procedure = Server.Procedure;


namespace ClientApp.ViewModels
{
    public class ClientProcedureListingViewModel : ReactiveObject
    {
        //View that this viewmodel is attached to
        private ClientProcedureListingView _ClientProcedureListingView;
        //The list of procedures for the client. Binded in the view to display the procedures
        private ObservableCollection<ProcedureModel> _procedures = new();
        private ObservableCollection<string> _displayedProcedures = new();
        //ID of the client selected
        private int _clientId;
        

        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
        }

        /// <summary>
        /// Constructor for the viewmodel. Initializes the procedure list and subscribes the list to being able to be selected
        /// </summary>
        /// <param name="cplv"></param>
        /// <param name="c_ID"></param>
        public ClientProcedureListingViewModel(ClientProcedureListingView cplv, int c_ID)
        {
            // localhost for testing purposes
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            //var client = new Client.ClientClient(channel);
            var client = new Procedure.ProcedureClient(channel);

            //Getting the clients from the database
            AllProcedures info = client.getProcedures(new ClientID { CID = c_ID });
            
            foreach (ProcedureInfo procedure in info.Procedures)
            {
                //Add procedures to the list
                _procedures.Add(new ProcedureModel(procedure.ProcedureID, procedure.ProcedureName, procedure.ProcedureDatetime, procedure.ClientID, procedure.EmployeeID, procedure.ProcedureNotes));
                _displayedProcedures.Add(procedure.ProcedureDatetime.Split(" ")[0] + " - " + procedure.ProcedureName);
            }
            //Makes the list of procedures publicly available
            Procedures = _procedures;
            DisplayedProcedures = _displayedProcedures;
            
            //Selection = new SelectionModel<ProcedureModel>();
            Selection = new SelectionModel<string>();
            Selection.SelectionChanged += SelectionChanged;

            //IsAdmin = false;
            _clientId = c_ID;

            _ClientProcedureListingView = cplv;
        }

        /*
        string _user = string.Empty;
        private bool _isAdmin = false;
        public ClientProcedureListingViewModel(ClientProcedureListingView cplv, string user, bool isAdmin, int c_ID)
        {
            // localhost for testing purposes
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            //var client = new Client.ClientClient(channel);
            var client = new Procedure.ProcedureClient(channel);

            //AllClients info = client.getClients(new Google.Protobuf.WellKnownTypes.Empty());
            AllProcedures info = client.getProcedures(new ClientID { CID=c_ID });

            //foreach (ProcedureInfo procedure in ProcedureClient.getProcedures(c_ID))
            foreach (ProcedureInfo procedure in info.Procedures)
            {
                _procedures.Add(new ProcedureModel(procedure.ProcedureID,procedure.ProcedureName, procedure.ProcedureDatetime,procedure.ClientID,procedure.EmployeeID,procedure.ProcedureNotes));
            }

            Procedures = _procedures;

            Selection = new SelectionModel<ProcedureModel>();
            Selection.SelectionChanged += SelectionChanged;

            _user = user;
            _isAdmin = isAdmin;
            //IsAdmin = false;
            _clientId = c_ID;

            _ClientProcedureListingView = cplv;
        }
        public bool IsAdmin
        {
            get => _isAdmin;
            set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
        }
        */
        /// <summary>
        /// Opens the home page view and closes the current view
        /// </summary>
        public void GoHomeCommand()
        {
            //new HomePage(_user, _isAdmin).Show();
            new HomePage().Show();
            _ClientProcedureListingView.Close();
        }
        
        public ObservableCollection<ProcedureModel> Procedures
        {
            get => _procedures;
            set
            {
                this.RaiseAndSetIfChanged(ref _procedures, value);
            }
        }
        public ObservableCollection<string> DisplayedProcedures
        {
            get => _displayedProcedures;
            set
            {
                this.RaiseAndSetIfChanged(ref _displayedProcedures, value);
            }
        }
        //public SelectionModel<ProcedureModel> Selection { get; }
        public SelectionModel<string> Selection { get; }
        /// <summary>
        /// Opens the make procedure view and closes current view
        /// </summary>
        public void GoToMakeProcedurePageCommand()
        {
            //new MakeProcedureView(_user,_isAdmin,_clientId).Show();
            new MakeProcedureView(_clientId).Show();
            _ClientProcedureListingView.Close();
        }

    }

}