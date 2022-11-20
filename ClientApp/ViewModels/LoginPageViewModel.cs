using ClientApp.Views;
using Grpc.Net.Client;
using Server;
using ReactiveUI;
using System.Reactive;

namespace ClientApp.ViewModels
{
    public class LoginPageViewModel : ReactiveObject,IRoutableViewModel
    {
        //Holds current user
        public static string GlobalUserName { get; set; }

        //Holds whether the user has admin privilages
        public static bool GlobalIsAdmin { get; set; }
        public string UserName { get; set; }

        //private string _password= string.Empty;
        public string Password { get; set; }

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
            GoNext = ReactiveCommand.CreateFromObservable(
              () => Router.Navigate.Execute(new HomePageViewModel()));
        }


        /// <summary>
        /// onclick event for logging in
        /// </summary>
        public void LoginCommand()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
            var ClientApp = new Employee.EmployeeClient(channel);

            var credentials = new LoginCredentials
            {
                Username = UserName,
                Password = Password,
            };
            var serviceResponse = ClientApp.doLogin(credentials);                               // assynchronous rpc to Server to verify login credentials
            //If the credentials are valid
            if (serviceResponse.IsSuccessfulLogin)
            {
                GlobalUserName = UserName;
                GlobalIsAdmin = serviceResponse.IsAdmin;

                //Takes user to home page
                GoNext.Execute();

                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Success", "User: "+GlobalUserName+", logged in successfully.").Show();
            }
            else
            {
                //Display message that log in failed 
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Failed", "Login failed").Show();

            }

        }
    }
}
