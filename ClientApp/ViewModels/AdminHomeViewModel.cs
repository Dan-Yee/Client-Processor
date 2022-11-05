using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using Server;

namespace ClientApp.ViewModels
{
    public class AdminHomeViewModel : ReactiveObject
    {
        //View that this viewmodel is attached to
        AdminHomeView _adminHomeView;
        //List of employees. Binded in view to display the employees
        private ObservableCollection<Models.EmployeeModel> _employees = new();

        public AdminHomeViewModel(AdminHomeView adminHomeView)
        {
            //Pass view to the viewmodel
            _adminHomeView = adminHomeView;
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
            Employees = _employees;
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
            
            //new HomePage(_user,_isAdmin).Show();
            new HomePage().Show();
            _adminHomeView.Close();
        }

        /// <summary>
        /// Opens the importing form view and closes this view
        /// </summary>
        public void OpenImportFormView()
        {
            new ImportFormView().Show();
            _adminHomeView.Close();
        }

        /// <summary>
        /// Opens employee registration view and closes this view
        /// </summary>
        public void CreateEmployeeCommand()
        {
            //new RegisterEmployeeView(_user,_isAdmin).Show();
            new RegisterEmployeeView().Show();
            _adminHomeView.Close();
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
