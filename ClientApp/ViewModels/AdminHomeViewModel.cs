using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using Server;
using System.Reactive;

namespace ClientApp.ViewModels
{
    public class AdminHomeViewModel : ReactiveObject,IRoutableViewModel
    {
        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "AdminHome";
        public RoutingState Router2 { get; } = new RoutingState();

        public RoutingState RouterHomePageProcedure { get; } = new RoutingState();

        public RoutingState RouterToImport { get; } = new RoutingState();

        public RoutingState RouterRegister { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoToImport { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoToRegister { get; }


        //View that this viewmodel is attached to
        AdminHomeView _adminHomeView;
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

            GoHome = ReactiveCommand.CreateFromObservable(
             () => RouterHomePageProcedure.Navigate.Execute(new HomePageViewModel()));

            GoToImport = ReactiveCommand.CreateFromObservable(
             () => RouterToImport.Navigate.Execute(new ImportFormViewModel()));

            GoToRegister = ReactiveCommand.CreateFromObservable(
             () => RouterRegister.Navigate.Execute(new RegisterEmployeeViewModel()));
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
        public void DeleteEmployeeCommand()
        {
            
            //Want yes no dialog here
        }

    }
}
