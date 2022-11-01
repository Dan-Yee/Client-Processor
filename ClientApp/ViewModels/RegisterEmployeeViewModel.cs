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
    public class RegisterEmployeeViewModel : ViewModelBase
    {
        string _user = string.Empty;
        bool _isAdmin = false;
        RegisterEmployeeView _registerEmployeeView;
        public RegisterEmployeeViewModel(RegisterEmployeeView registerEmployeeView, string user, bool isAdmin)
        {
            _registerEmployeeView = registerEmployeeView;
            _user = user;
            _isAdmin = isAdmin;
        }

        private string _employeeFirstName = string.Empty;

        public string EmployeeFirstName
        {

            get
            {
                return _employeeFirstName;
            }

            set
            {
                _employeeFirstName = value;
                OnPropertyChanged(nameof(EmployeeFirstName));

            }
        }

        private string _employeeLastName = string.Empty;

        public string EmployeeLastName
        {

            get
            {
                return _employeeLastName;
            }

            set
            {
                _employeeLastName = value;
                OnPropertyChanged(nameof(EmployeeLastName));

            }
        }

        private string _employeeUserName = string.Empty;

        public string EmployeeUserName
        {

            get
            {
                return _employeeUserName;
            }

            set
            {
                _employeeUserName = value;
                OnPropertyChanged(nameof(EmployeeUserName));

            }
        }

        private string _employeePassword = string.Empty;

        public string EmployeePassword
        {

            get
            {
                return _employeePassword;
            }

            set
            {
                _employeePassword = value;
                OnPropertyChanged(nameof(EmployeePassword));

            }
        }

        private bool _employeeIsAdmin = false;
        public bool EmployeeIsAdmin
        {
            get { return _employeeIsAdmin; }
            set
            {
                _employeeIsAdmin = value;
                OnPropertyChanged(nameof(EmployeeIsAdmin));
            }
        }

        

        public void EmployeeRegisterCommand()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
            var employee = new Server.Employee.EmployeeClient(channel);
            var loginCredentials = new LoginCredentials 
            {
                Username = EmployeeUserName,
                Password = EmployeePassword
            };

            var employeeInfo = new EmployeeInfo
            {
                FirstName = EmployeeFirstName,
                LastName = EmployeeLastName,
                Credentials = loginCredentials,
                IsAdmin = EmployeeIsAdmin,
            };
            var createResponse = employee.newEmployee(employeeInfo);

            new AdminHomeView(_user,_isAdmin).Show();
            _registerEmployeeView.Close();
        }

        public void ToAdminHomeCommand()
        {
            new AdminHomeView(_user, _isAdmin).Show();
            _registerEmployeeView.Close();
        }
    }
}
