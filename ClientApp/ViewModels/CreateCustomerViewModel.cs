using ReactiveUI;
using Server;
using System;
using System.ComponentModel;
using System.Reactive;

namespace ClientApp.ViewModels
{
    public class CreateCustomerViewModel:ViewModelBase, IRoutableViewModel
    {
        
        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "CreateCustomer";
        public RoutingState RouterHomePage { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }
        //Holds input for first name of client
        public string FirstName { get; set; } = string.Empty;
        
        //Holds input for last name of client
        public string LastName { get; set; } = string.Empty;

        //Holds input for email of client
        public string Email { get; set; } = string.Empty;

        //Holds input for phone number of client
        public string PhoneNumber { get; set; } = string.Empty;
        public event PropertyChangingEventHandler? PropertyChanging;

        public CreateCustomerViewModel()
        {
            GoHome = ReactiveCommand.CreateFromObservable(
             () => RouterHomePage.Navigate.Execute(new HomePageViewModel()));
        }

        /// <summary>
        /// Onclick event for creating employee.
        /// </summary>
        public void RegisterCommand()
        {
            //if(FirstName != null && FirstName!="" && LastName!=null && LastName!="" && Email!=null && Email!="" && PhoneNumber!=null && PhoneNumber != "")
            //Makes sure that required fields have values
            if(FirstName != null && FirstName!="" && LastName!=null && LastName!="" && PhoneNumber!=null && PhoneNumber != "")
            {
                var client = new Client.ClientClient(Program.gRPCChannel);

                //Initializing the client
                var clientInfo = new ClientInfo
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    PhoneNumber = PhoneNumber,
                    Email = Email
                };
                //Creating the client
                var createResponse = client.newClient(clientInfo);

                GoHome.Execute();
            }
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
