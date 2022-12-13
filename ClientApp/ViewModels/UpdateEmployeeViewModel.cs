using ReactiveUI;
using Server;
using System;
using System.Reactive;

namespace ClientApp.ViewModels
{
    public class UpdateEmployeeViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();
        public RoutingState RouterToAdminHomePage { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToAdminHomePage { get; }

        //Holds initial value of first name
        public static string CurrentFirstNameInfo { get; set; } = string.Empty;
        //Holds initial value of last name
        public static string CurrentLastNameInfo { get; set; } = string.Empty;
        //Holds initial value of user name
        public static string CurrentUserNameInfo { get; set; } = string.Empty;
        //Holds initial value of password
        public static string CurrentPasswordInfo { get; set; } = string.Empty;
        //Holds initial value of admin status
        public static bool CurrentIsAdminInfo { get; set; } = false;

        //Holds input value of first name
        public string EmployeeFirstNameInfo { get; set; } = string.Empty;
        //Holds input value of last name
        public string EmployeeLastNameInfo { get; set; } = string.Empty;
        //Holds input value of user name
        public string EmployeeUserNameInfo { get; set; } = string.Empty;
        //Holds input value of password
        public string EmployeePasswordInfo { get; set; } = string.Empty;
        //Holds input value of admin status
        public bool EmployeeIsAdminInfo { get; set; } = false;

        public UpdateEmployeeViewModel()
        {
            //This information is the info that will be shown in the text boxes
            EmployeeFirstNameInfo = AdminHomeViewModel.SelectedEmployee.FirstName;
            EmployeeLastNameInfo = AdminHomeViewModel.SelectedEmployee.LastName;
            EmployeeUserNameInfo = AdminHomeViewModel.SelectedEmployee.UserName;
            EmployeeIsAdminInfo = AdminHomeViewModel.SelectedEmployee.IsAdmin;

            //This information is the info that will be used when the user clicks on a reset button
            CurrentFirstNameInfo = AdminHomeViewModel.SelectedEmployee.FirstName;
            CurrentLastNameInfo = AdminHomeViewModel.SelectedEmployee.LastName;
            CurrentUserNameInfo = AdminHomeViewModel.SelectedEmployee.UserName;
            CurrentIsAdminInfo = AdminHomeViewModel.SelectedEmployee.IsAdmin;

            NavigateToAdminHomePage = ReactiveCommand.CreateFromObservable(
             () => RouterToAdminHomePage.Navigate.Execute(new AdminHomeViewModel()));
        }

        /// <summary>
        /// Handles update command. First checks input for changed/non-empty values. Then sends updated info to database.
        /// </summary>
        public void UpdateCommand()
        {
            var employee = new Employee.EmployeeClient(Program.gRPCChannel);
            
            //Updates info if input is not null
            if (EmployeeFirstNameInfo != null && EmployeeFirstNameInfo != "")
            {
                CurrentFirstNameInfo = EmployeeFirstNameInfo;
            }
            if (EmployeeLastNameInfo != null && EmployeeLastNameInfo != "")
            {
                CurrentLastNameInfo = EmployeeLastNameInfo;
            }
            if (EmployeeUserNameInfo != null && EmployeeUserNameInfo != "")
            {
                CurrentUserNameInfo = EmployeeUserNameInfo;
            }
            if (EmployeePasswordInfo != null && EmployeePasswordInfo != "")
            {
                CurrentPasswordInfo = EmployeePasswordInfo;
            }

            //Initializing updated employee
            CurrentIsAdminInfo = EmployeeIsAdminInfo;
            EmployeeInfo newInfo = new()
            {
                EmployeeId = AdminHomeViewModel.SelectedEmployee.ID,
                FirstName = CurrentFirstNameInfo,
                LastName = CurrentLastNameInfo,
                Credentials = new LoginCredentials() { Username = CurrentUserNameInfo, Password = CurrentPasswordInfo },
                IsAdmin = CurrentIsAdminInfo
            };

            //Sends updated client to database
            ServiceStatus status = employee.updateEmployee(newInfo);
            //Takes user back to admin home page
            NavigateToAdminHomePage.Execute();
        }

        /// <summary>
        /// Takes user to admin home page
        /// </summary>
        public void GoToHomePageCommand()
        {
            NavigateToAdminHomePage.Execute();
        }

        /// <summary>
        /// Resets value of first name
        /// </summary>
        public void ResetFirstName()
        {
            EmployeeFirstNameInfo = CurrentFirstNameInfo;
        }

        /// <summary>
        /// resets value of last name
        /// </summary>
        public void ResetLastName()
        {
            EmployeeLastNameInfo = CurrentLastNameInfo;
        }

        /// <summary>
        /// Resets value of user name
        /// </summary>
        public void ResetUserName()
        {
            EmployeeUserNameInfo = CurrentUserNameInfo;
        }

        /// <summary>
        /// Resets value of Password
        /// </summary>
        public void ResetPassword()
        {
            EmployeePasswordInfo = CurrentPasswordInfo;
        }
    }
}
