using ClientApp.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class AdminHomeViewModel : ReactiveObject
    {
        AdminHomeView _adminHomeView;
        private ObservableCollection<string> _employees = new();
        public AdminHomeViewModel(AdminHomeView adminHomeView)
        {
            _adminHomeView = adminHomeView;
            var employees = new ObservableCollection<string>
            {
                "Employee1",
                "Employee2"
            };
            Employees = employees;
        }
        public ObservableCollection<string> Employees
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
