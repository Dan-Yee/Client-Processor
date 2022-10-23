using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ClientApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientApp.ViewModels
{
    public class CreateCustomerViewModel:ViewModelBase
    {
        CreateCustomerPage _createCustomerPage;

        public CreateCustomerViewModel(CreateCustomerPage ccp)
        {
            _createCustomerPage = ccp;
        }

        private string _firstName = string.Empty;

        public string FirstName
        {

            get
            {
                return _firstName;
            }

            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));

            }
        }

        private string _lastName = string.Empty;

        public string LastName
        {

            get
            {
                return _lastName;
            }

            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));

            }
        }

        private string _insuranceCompany = string.Empty;

        public string InsuranceCompany
        {

            get
            {
                return _insuranceCompany;
            }

            set
            {
                _insuranceCompany = value;
                OnPropertyChanged(nameof(InsuranceCompany));

            }
        }

        private string _phoneNumber = string.Empty;

        public string PhoneNumber
        {

            get
            {
                return _phoneNumber;
            }

            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));

            }
        }

        private string _email = string.Empty;

        public string Email
        {

            get
            {
                return _email;
            }

            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));

            }
        }
        public void RegisterCommand()
        {
            //Do magic here
            new HomePage().Show();
            _createCustomerPage.Close();
            var didStuffMsg = MessageBox.Avalonia.MessageBoxManager
    .GetMessageBoxStandardWindow("title", "Clicked");
            didStuffMsg.Show();
        }
    }
}
