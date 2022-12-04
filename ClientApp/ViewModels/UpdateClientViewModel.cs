using Grpc.Net.Client;
using ReactiveUI;
using Server;
using System.Reactive;

namespace ClientApp.ViewModels
{
    public class UpdateClientViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment { get; } = "UpdateClientViewModel";

        public IScreen HostScreen { get; }

        public RoutingState RouterToHomePage { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> GoToHomePage { get; }

        //Holds value of first name before update
        public static string CurrentFirstName = string.Empty;
        //Holds value of last name before update
        public static string CurrentLastName = string.Empty;
        //Holds value of phone number before update
        public static string CurrentPhoneNumber = string.Empty;
        //Holds value of email before update
        public static string CurrentEmail = string.Empty;

        //Holds value of first name to update
        public string ClientFirstNameInfo { get; set; }
        
        //Holds value of last name to update
        public string ClientLastNameInfo { get; set; }
        
        //Holds value of phone number to update
        public string ClientPhoneNumberInfo { get; set; }
        
        //Holds value of email to update
        public string ClientEmailInfo { get; set; }

        public UpdateClientViewModel()
        {
            GoToHomePage = ReactiveCommand.CreateFromObservable(
             () => RouterToHomePage.Navigate.Execute(new HomePageViewModel()));

            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new Client.ClientClient(channel);

            AllClients info = client.searchClientsByName(new ClientName() { CName = HomePageViewModel.ClientName });

            if (info.Clients.Count > 0)
            {
                var c = info.Clients[0];
                ClientFirstNameInfo = c.FirstName;
                ClientLastNameInfo = c.LastName;
                ClientPhoneNumberInfo = c.PhoneNumber;
                ClientEmailInfo = c.Email;
                CurrentFirstName = c.FirstName;
                CurrentLastName = c.LastName;
                CurrentPhoneNumber = c.PhoneNumber;
                CurrentEmail = c.Email;
            }
        }

        /// <summary>
        /// Takes user to home page
        /// </summary>
        public void goToHomePageCommand()
        {
            GoToHomePage.Execute();
        }

        /// <summary>
        /// Updates values and takes user back to home page
        /// </summary>
        public void UpdateCommand()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new Client.ClientClient(channel);
            if(ClientFirstNameInfo != null && ClientFirstNameInfo != "")
            {
                CurrentFirstName = ClientFirstNameInfo;
            }
            if(ClientLastNameInfo != null && ClientLastNameInfo != "")
            {
                CurrentLastName = ClientLastNameInfo;
            }
            if(ClientPhoneNumberInfo != null && ClientPhoneNumberInfo != "")
            {
                CurrentPhoneNumber = ClientPhoneNumberInfo;
            }
            if(ClientEmailInfo != null && ClientEmailInfo != "")
            {
                CurrentEmail = ClientEmailInfo;
            }
            ClientInfo newInfo = new()
            {
                ClientId = HomePageViewModel.Client_ID,
                FirstName = CurrentFirstName,
                LastName = CurrentLastName,
                PhoneNumber = CurrentPhoneNumber,
                Email = CurrentEmail,
            };

            ServiceStatus status = client.updateClient(newInfo);
            GoToHomePage.Execute();
        }

        /// <summary>
        /// Resets value of first name
        /// </summary>
        public void ResetFirstName()
        {
            ClientFirstNameInfo = CurrentFirstName;
        }

        /// <summary>
        /// resets value of last name
        /// </summary>
        public void ResetLastName()
        {
            ClientLastNameInfo = CurrentLastName;
        }

        /// <summary>
        /// Resets value of phone number
        /// </summary>
        public void ResetPhoneNumber()
        {
            ClientPhoneNumberInfo = CurrentPhoneNumber;
        }

        /// <summary>
        /// Resets value of email
        /// </summary>
        public void ResetEmail()
        {
            ClientEmailInfo = CurrentEmail;
        }

    }
}
