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
        RegisterEmployeeView _registerEmployeeView;
        public RegisterEmployeeViewModel(RegisterEmployeeView registerEmployeeView)
        {
            _registerEmployeeView = registerEmployeeView;
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
                
            };
            var createResponse = employee.newEmployee(employeeInfo);

            new AdminHomeView().Show();
            _registerEmployeeView.Close();
        }
    }
}
