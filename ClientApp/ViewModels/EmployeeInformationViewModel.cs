using Grpc.Net.Client;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class EmployeeInformationViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment { get; } = "EmployeeInformationView";

        public IScreen HostScreen { get; }

        public RoutingState Router { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoToHomePage { get; }

        public string EmployeeFirstNameInfo { get; set; }
        public string EmployeeLastNameInfo { get; set; }
        public string EmployeeUserNameInfo { get; set; }
        public string EmployeeIsAdminInfo { get; set; }
        

        public EmployeeInformationViewModel()
        {
            GoToHomePage = ReactiveCommand.CreateFromObservable(
             () => Router.Navigate.Execute(new AdminHomeViewModel()));
            //AdminHomeViewModel.SelectedEmployee
            EmployeeFirstNameInfo = AdminHomeViewModel.SelectedEmployee.FirstName;
            EmployeeLastNameInfo = AdminHomeViewModel.SelectedEmployee.LastName;
            EmployeeUserNameInfo = AdminHomeViewModel.SelectedEmployee.UserName;
            if (AdminHomeViewModel.SelectedEmployee.IsAdmin)
            {
                EmployeeIsAdminInfo = "Is admin";
            }
            else
            {
                EmployeeIsAdminInfo = "Not admin";
            }
        }

        public void GoToHomePageCommand()
        {
            GoToHomePage.Execute();
        }
    }
}
