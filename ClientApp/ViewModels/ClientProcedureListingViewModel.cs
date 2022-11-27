using Avalonia.Controls.Selection;
using AvaloniaEdit.Editing;
using ClientApp.Models;
using ClientApp.Views;
using DynamicData;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components.Routing;
using ReactiveUI;
using Server;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Procedure = Server.Procedure;


namespace ClientApp.ViewModels
{
    public class ClientProcedureListingViewModel : ReactiveObject, IRoutableViewModel
    {
        //View that this viewmodel is attached to
        //private ClientProcedureListingView _ClientProcedureListingView;
        //The list of procedures for the client. Binded in the view to display the procedures
        private ObservableCollection<ProcedureModel> _procedures = new();
        private ObservableCollection<string> _displayedProcedures = new();
        //ID of the client selected
        private int _clientId;

        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "ClientProcedureListing";

        public RoutingState Router { get; } = new RoutingState();
        public RoutingState Router2 { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> MakeProcedurePage { get; }

        public RoutingState Router0 { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> GoGoToClientProceduresCommand { get; }

        public List<int> ListOfProcedureIDs { get; set; } = new();


        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
        }

        /// <summary>
        /// Constructor for the viewmodel. Initializes the procedure list and subscribes the list to being able to be selected
        /// </summary>
        /// <param name="c_ID"></param>
        public ClientProcedureListingViewModel(int c_ID)
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
                ListOfProcedureIDs.Add(procedure.ProcedureID);
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

            GoHome = ReactiveCommand.CreateFromObservable(
             () => Router.Navigate.Execute(new HomePageViewModel()));

            GoGoToClientProceduresCommand = ReactiveCommand.CreateFromObservable(
              () => Router0.Navigate.Execute(new ClientProcedureListingViewModel(c_ID)));
            //MakeProcedurePage = ReactiveCommand.CreateFromObservable(
            //() => Router2.Navigate.Execute(new MakeProcedureViewModel(c_ID)));


        }

        public void GoToMakeProcedurePageCommand()
        {
            //new MakeProcedureView(_clientId).Show();
            new InitializeProcedureView(_clientId).Show();
            //Locator.CurrentMutable.Register(() => new MakeProcedureView(), typeof(IViewFor<MakeProcedureViewModel>));
            //MakeProcedurePage.Execute();
            //= ReactiveCommand.CreateFromObservable(
             //() => Router.Navigate.Execute(new MakeProcedureViewModel(c_ID)));

        }

        public void DeleteProcedureCommand()
        {
            new Procedure.ProcedureClient(GrpcChannel.ForAddress("https://localhost:7123")).deleteProcedure(new ProcedureID() { PID = ListOfProcedureIDs[Selection.SelectedIndex] });
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Selection: "+ ListOfProcedureIDs[Selection.SelectedIndex] + " deleted.").Show();
            _displayedProcedures.RemoveAt(Selection.SelectedIndex);
        }

        /// <summary>
        /// Opens the home page view and closes the current view
        /// </summary>
        public void GoHomeCommand()
        {
            GoHome.Execute();
            
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
        

    }

}