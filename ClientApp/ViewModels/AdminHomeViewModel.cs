using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using Server;

namespace ClientApp.ViewModels
{
    public class AdminHomeViewModel : ReactiveObject
    {
        AdminHomeView _adminHomeView;
        private ObservableCollection<Models.EmployeeModel> _employees = new();
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
        public ObservableCollection<Models.EmployeeModel> Employees
        {
            get => _employees;
            set
            {
                this.RaiseAndSetIfChanged(ref _employees, value);
            }
        }


        public void GoToHomeFromAdminHomeCommand()
        {
            new HomePage(_user,_isAdmin).Show();
            _adminHomeView.Close();
        }

        public void CreateEmployeeCommand()
        {
            new RegisterEmployeeView(_user,_isAdmin).Show();
            _adminHomeView.Close();
        }

    }
}
