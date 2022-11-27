using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using Server;
using System.Reactive;
using System.ComponentModel;
using Avalonia.Controls.Selection;
using AvaloniaEdit.Editing;
using ClientApp.Models;
using MessageBox.Avalonia.Enums;

namespace ClientApp.ViewModels
{
    public class AdminHomeViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }

        public static EmployeeModel SelectedEmployee {get;set;}

        public SelectionModel<EmployeeModel> Selection { get; }

        public string UrlPathSegment { get; } = "AdminHome";
        public RoutingState Router2 { get; } = new RoutingState();
        public RoutingState ViewEmployeeRouter{ get; } = new RoutingState();

        public RoutingState RouterHomePageProcedure { get; } = new RoutingState();

        public RoutingState RouterToImport { get; } = new RoutingState();

        public RoutingState RouterRegister { get; } = new RoutingState();
        

        public ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoToImport { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoToRegister { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> GoToEmployeeInformation{ get; }

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

        //List of employees. Binded in view to display the employees
        private ObservableCollection<Models.EmployeeModel> _employees = new();

        public AdminHomeViewModel()
        {
            //Pass view to the viewmodel
            //_adminHomeView = adminHomeView;
            // localhost for testing purposes
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new Server.Employee.EmployeeClient(channel);
            //AllEmployeesInfo info = client.getEmployees(new Google.Protobuf.WellKnownTypes.Empty());
            //Get employees from database
            AllEmployees info = client.getEmployees(new Google.Protobuf.WellKnownTypes.Empty());
            foreach (EmployeeInfo e in info.Employees)
            {
                //Add employees to the list
                _employees.Add(new Models.EmployeeModel(e.EmployeeId, e.FirstName, e.LastName, e.Credentials.Username, e.IsAdmin));
            }
            //Employees = _employees;
            SelectButtonEnabled = false;
            Selection = new SelectionModel<EmployeeModel>();
            Selection.SelectionChanged += SelectionChanged;

            GoHome = ReactiveCommand.CreateFromObservable(
             () => RouterHomePageProcedure.Navigate.Execute(new HomePageViewModel()));

            GoToImport = ReactiveCommand.CreateFromObservable(
             () => RouterToImport.Navigate.Execute(new ImportFormViewModel()));

            GoToRegister = ReactiveCommand.CreateFromObservable(
             () => RouterRegister.Navigate.Execute(new RegisterEmployeeViewModel()));

            GoToEmployeeInformation = ReactiveCommand.CreateFromObservable(
             () => ViewEmployeeRouter.Navigate.Execute(new EmployeeInformationViewModel()));
        }

        /*
        string _user =string.Empty;
        bool _isAdmin = false;
        public AdminHomeViewModel(AdminHomeView adminHomeView,string user, bool isAdmin)
        {
            _user = user;
            _isAdmin = isAdmin;
            _adminHomeView = adminHomeView;
            // localhost for testing purposes
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new Server.Employee.EmployeeClient(channel);
            //AllEmployeesInfo info = client.getEmployees(new Google.Protobuf.WellKnownTypes.Empty());
            AllEmployees info = client.getEmployees(new Google.Protobuf.WellKnownTypes.Empty());
            foreach (EmployeeInfo e in info.Employees)
            {
                _employees.Add(new Models.EmployeeModel(e.EmployeeId,e.FirstName,e.LastName,e.Credentials.Username,e.IsAdmin));
            }
            Employees = _employees;
        }
        */
        public ObservableCollection<Models.EmployeeModel> Employees
        {
            get => _employees;
            set
            {
                this.RaiseAndSetIfChanged(ref _employees, value);
            }
        }

        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
            SelectButtonEnabled = true;
        }

        /// <summary>
        /// Opens home page view and closes this view
        /// </summary>
        public void GoToHomeFromAdminHomeCommand()
        {
            GoHome.Execute();
            
        }

        /// <summary>
        /// Opens the importing form view and closes this view
        /// </summary>
        public void OpenImportFormView()
        {
            GoToImport.Execute();
        
        }

        /// <summary>
        /// Opens employee registration view and closes this view
        /// </summary>
        public void CreateEmployeeCommand()
        {

            GoToRegister.Execute();
            
        }

        /// <summary>
        /// Deletes employee from database
        /// </summary>
        public async void DeleteEmployeeCommand()
        {

            //Want yes no dialog here
            ButtonResult result = await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Confirm deletion", ButtonEnum.YesNo).Show();
            if(result == ButtonResult.Yes)
            {

                
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "To delete").Show();
            }
            else
            {
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Clicked no").Show();
            }
        }

        public void GoToReadProcedurePageCommand()
        {
            SelectedEmployee=Selection.SelectedItem;
            GoToEmployeeInformation.Execute();
        }
    }
}
