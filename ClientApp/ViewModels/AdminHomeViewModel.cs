using ClientApp.Models;
using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Server;
using DynamicData;

namespace ClientApp.ViewModels
{
    public class AdminHomeViewModel : ReactiveObject
    {
        AdminHomeView _adminHomeView;
        private ObservableCollection<Models.Employee> _employees = new();

        public AdminHomeViewModel(AdminHomeView adminHomeView)
        {
            _adminHomeView = adminHomeView;
            // localhost for testing purposes
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new Server.Employee.EmployeeClient(channel);
            AllEmployeesInfo info = client.getEmployees(new Google.Protobuf.WellKnownTypes.Empty());
            foreach (EmployeeInfo e in info.Employees)
            {
                _employees.Add(new Models.Employee(e.EmployeeId,e.FirstName,e.LastName,e.Credentials.Username));
            }
            Employees = _employees;
        }
        public ObservableCollection<Models.Employee> Employees
        {
            get => _employees;
            set
            {
                this.RaiseAndSetIfChanged(ref _employees, value);
            }
        }


        public void GoToHomeFromAdminHomeCommand()
        {
            new HomePage().Show();
            _adminHomeView.Close();
        }

        public void CreateEmployeeCommand()
        {
            new RegisterEmployeeView().Show();
            _adminHomeView.Close();
        }

    }
}
