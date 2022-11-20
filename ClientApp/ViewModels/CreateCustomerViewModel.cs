using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ClientApp.Views;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components.Routing;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientApp.ViewModels
{
    public class CreateCustomerViewModel:ViewModelBase, IRoutableViewModel
    {
        
        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "CreateCustomer";
        public RoutingState RouterHomePage { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }


        //View that this viewmodel is attached to
        CreateCustomerPage _createCustomerPage;


        public CreateCustomerViewModel()
        {
            GoHome = ReactiveCommand.CreateFromObservable(
             () => RouterHomePage.Navigate.Execute(new HomePageViewModel()));
        }


        /// <summary>
        /// Constructor for the viewmodel. Initializes the view
        /// </summary>
        /// <param name="ccp"></param>
        /*
        public CreateCustomerViewModel(CreateCustomerPage ccp)
        {
            _createCustomerPage = ccp;
        }
        */
        /*
        string _user = string.Empty;
        bool _isAdmin = false;
        public CreateCustomerViewModel(CreateCustomerPage ccp, string user, bool isAdmin)
        {
            _createCustomerPage = ccp;
            _user = user;
            _isAdmin = isAdmin;
        }
        */

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

        /*private string _insuranceCompany = string.Empty;

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
        */
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

        public event PropertyChangingEventHandler? PropertyChanging;

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

        /// <summary>
        /// Onclick event for creating employee.
        /// </summary>
        public void RegisterCommand()
        {

            var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
            var client = new Client.ClientClient(channel);

            //Initializing the client
            var clientInfo = new ClientInfo
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
            var createResponse = client.newClient(clientInfo);

            GoHome.Execute();
            
        }
        /// <summary>
        /// Takes user to the home page
        /// </summary>
        public void ToHomeScreenCommand()
        {
            GoHome.Execute();
            
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
