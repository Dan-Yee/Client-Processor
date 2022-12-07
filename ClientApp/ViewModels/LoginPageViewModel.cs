using ClientApp.Views;
using Grpc.Net.Client;
using Server;
using ReactiveUI;
using System.Reactive;
using System.ComponentModel;
using System;

namespace ClientApp.ViewModels
{
    public class LoginPageViewModel : ReactiveObject, INotifyPropertyChanged,IRoutableViewModel
    {
        //Holds current user
        public static string GlobalUserName { get; set; }
        public static int GlobalEmployeeID{ get; set; }

        //Holds whether the user has admin privilages
        public static bool GlobalIsAdmin { get; set; }
        
        //Holds the username input
        public string UserName { get; set; }
        
        //Holds the password input
        public string Password {get;set;}

        //Tracks number of invalid login attempts
        public static int InvalidCredentialsCount { get; set; }


        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "Login";

        // Required by the IScreen interface.
        public RoutingState Router { get; } = new RoutingState();

        // The command that navigates a user to first view model.
        public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get;}


        /// <summary>
        /// Constructor for the viewmodel. initializes the routing
        /// </summary>
        public LoginPageViewModel()
        {
            //Resets count every time this page loads
            InvalidCredentialsCount = 0;
            //Function for going to home page
            GoNext = ReactiveCommand.CreateFromObservable(
              () => Router.Navigate.Execute(new HomePageViewModel()));
        }


        /// <summary>
        /// onclick event for logging in
        /// </summary>
        public void LoginCommand()
        {
            //Only runs code if username and password fields are filled
            if (UserName != null && UserName != "" && Password != null && Password != "")
            {
                var ClientApp = new Employee.EmployeeClient(Program.gRPCChannel);

                var credentials = new LoginCredentials
                {
                    Username = UserName,
                    Password = Password,
                };

                var serviceResponse = ClientApp.doLogin(credentials);
                //If the credentials are valid
                if (serviceResponse.IsSuccessfulLogin)
                {
                    GlobalUserName = UserName;
                    GlobalIsAdmin = serviceResponse.IsAdmin;

                    //Takes user to home page
                    GoNext.Execute();

                    GlobalEmployeeID = serviceResponse.EmployeeId;

                    //Uncomment if you want a message that lets the user know that they logged in
                    //MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Success", "User: " + GlobalUserName + ", logged in successfully.").Show();
                }
            }

        }
    }
}
