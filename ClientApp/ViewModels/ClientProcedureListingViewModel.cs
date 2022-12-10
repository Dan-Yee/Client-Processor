using Avalonia.Controls.Selection;
using ClientApp.Models;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using Procedure = Server.Procedure;


namespace ClientApp.ViewModels
{
    public class ClientProcedureListingViewModel : ReactiveObject, IRoutableViewModel
    {
        //The list of procedures for the client. Binded in the view to display the procedures
        private ObservableCollection<ProcedureModel> _procedures = new();
        private ObservableCollection<string> _displayedProcedures = new();

        public static int Procedure_Id { get; set; }
        public static ProcedureModel SelectedProcedure { get; set; }

        //Determines if an element has been selected in the list view
        private bool _selectButtonEnabled;
        public bool SelectButtonEnabled
        {
            get
            {
                return _selectButtonEnabled;
            }
            set
            {
                _selectButtonEnabled = value;
                //Updates that a value has been selected
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectButtonEnabled)));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "ClientProcedureListing";

        public RoutingState Router { get; } = new RoutingState();
        public RoutingState Router2 { get; } = new RoutingState();
        public RoutingState GoToReadProcedureRouter { get; } = new RoutingState();
        public RoutingState GoToInitializeProcedureRouter { get; } = new RoutingState();
        public RoutingState GoToUpdateProcedureRouter { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> MakeProcedurePage { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> GoToReadProcedureView { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToInitializeProcedure { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToUpdateProcedure { get; }

        public RoutingState Router0 { get; } = new RoutingState();
        

        public List<int> ListOfProcedureIDs { get; set; } = new();


        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
            //Automatically uploads procedure id
            try
            {
                Procedure_Id = ListOfProcedureIDs[Selection.SelectedIndex];
                SelectedProcedure = _procedures[Selection.SelectedIndex];
                SelectButtonEnabled = true;
            }
            catch (Exception exception)
            {
                Procedure_Id = ListOfProcedureIDs[0];
                SelectedProcedure = _procedures[0];
                SelectButtonEnabled = true;
            }
            
        }


        public ClientProcedureListingViewModel()
        {
            var client = new Procedure.ProcedureClient(Program.gRPCChannel);

            //Getting the clients from the database
            AllProcedures info = client.getProcedures(new ClientID { CID = HomePageViewModel.Client_ID});

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
            SelectButtonEnabled = false;

            //Selection = new SelectionModel<ProcedureModel>();
            Selection = new SelectionModel<string>();
            Selection.SelectionChanged += SelectionChanged;

            

            GoHome = ReactiveCommand.CreateFromObservable(
             () => Router.Navigate.Execute(new HomePageViewModel()));
            GoToReadProcedureView = ReactiveCommand.CreateFromObservable(
              () => GoToReadProcedureRouter.Navigate.Execute(new ProcedureReadViewModel()));
            NavigateToInitializeProcedure= ReactiveCommand.CreateFromObservable(
              () => GoToInitializeProcedureRouter.Navigate.Execute(new InitializeProcedureViewModel()));
            NavigateToUpdateProcedure = ReactiveCommand.CreateFromObservable(
              () => GoToUpdateProcedureRouter.Navigate.Execute(new ProcedureUpdateViewModel()));
        }

        
        public void GoToMakeProcedurePageCommand()
        {
            //new InitializeProcedureView(_clientId).Show();
            NavigateToInitializeProcedure.Execute();
        }

        public async void DeleteProcedureCommand()
        {
            ButtonResult result = await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Confirm deletion", ButtonEnum.YesNo).Show();
            if (result == ButtonResult.Yes)
            {
                new Procedure.ProcedureClient(Program.gRPCChannel).deleteProcedure(new ProcedureUpdateInfo() { PID = ListOfProcedureIDs[Selection.SelectedIndex], EmployeeID = LoginPageViewModel.GlobalEmployeeID });
                SelectButtonEnabled = false;
                _displayedProcedures.RemoveAt(Selection.SelectedIndex);
            }
            else
            {
                //User picked no
            }
            
        }

        public void GoToReadProcedurePageCommand()
        {

            GoToReadProcedureView.Execute();
        }

        public static string ProcedureName { get; set; }
        public static string ProcedureNotes { get; set; }
        public void GoToUpdateProcedurePageCommand()
        {
            ProcedureName = _procedures[Selection.SelectedIndex].ProcedureName;
            ProcedureNotes = _procedures[Selection.SelectedIndex].procedureNotes;
            NavigateToUpdateProcedure.Execute();
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