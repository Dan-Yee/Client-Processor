using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ClientApp.Views;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tmds.DBus;
using Grpc.Net.Client;
using Server;
using System.Security.Cryptography;

namespace ClientApp.ViewModels
{
    public class LoginPageViewModel: ViewModelBase
    {
        //Holds current user
        public static string GlobalUserName { get; set; }

        //Holds whether the user has admin privilages
        public static bool GlobalIsAdmin{ get; set; }
        public string UserName { get; set; }

        //private string _password= string.Empty;
        public string Password { get; set; }

        //View that this viewmodel is attached to
        LoginPage _loginPage;

        /// <summary>
        /// Constructor for the viewmodel. initializes the view
        /// </summary>
        /// <param name="lp"></param>
        public LoginPageViewModel(LoginPage lp)
        {
            _loginPage = lp;
            
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
                new HomePage().Show();
                _loginPage.Close();
                //var loginSuccessMessage = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "User: " + GlobalUserName + " Logged in successfully. Admin="+GlobalIsAdmin);
                //loginSuccessMessage.Show();
            }
            else
            {
                //Display message that log in failed
                var loginFailedMessage = MessageBox.Avalonia.MessageBoxManager
    .GetMessageBoxStandardWindow("title", "Logged in failed");
                loginFailedMessage.Show();

            }
            
        }
    }
}
