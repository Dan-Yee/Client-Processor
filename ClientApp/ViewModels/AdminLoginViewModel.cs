using Avalonia.Controls;
using ClientApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class AdminLoginViewModel : ViewModelBase
    {
        AdminLoginView _adminLoginView;
        public AdminLoginViewModel(AdminLoginView adminLoginView)
        {
            _adminLoginView = adminLoginView;
        }

        private string _adminUsername = string.Empty;

        public string AdminUserName
        {

            get
            {
                return _adminUsername;
            }

            set
            {
                _adminUsername = value;
                OnPropertyChanged(nameof(AdminUserName));

            }
        }

        private string _adminPassword = string.Empty;

        public string AdminPassword
        {

            get
            {
                return _adminPassword;
            }

            set
            {
                _adminPassword = value;
                OnPropertyChanged(nameof(AdminPassword));

            }
        }

        
        public void AdminLoginCommand()
        {
            bool booltool = true;
            if (booltool)
            {
                new AdminHomeView().Show();
                _adminLoginView.Close();
                var loginSuccessMessage = MessageBox.Avalonia.MessageBoxManager
    .GetMessageBoxStandardWindow("title", "User: " + AdminUserName + " Logged in successfully");
                loginSuccessMessage.Show();
            }
            else
            {
                var loginFailedMessage = MessageBox.Avalonia.MessageBoxManager
    .GetMessageBoxStandardWindow("title", "Logged in failed");
                loginFailedMessage.Show();

            }
        }
        public void ToHomePageCommand()
        {
            new HomePage().Show();
            _adminLoginView.Close();
        }
    }
}
