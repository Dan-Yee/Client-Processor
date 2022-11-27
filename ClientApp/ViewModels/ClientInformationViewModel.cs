using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class ClientInformationViewModel : ReactiveObject, IRoutableViewModel
    {
        public ObservableCollection<string> ListOfInformationFields { get; set; } = new()
        { "First name: ","Last name:","Phone number: ", "Email: "};

        public string ClientFirstNameInfo { get; set; }
        public string ClientLastNameInfo { get; set; }
        public string ClientPhoneNumberInfo { get; set; }
        public string ClientEmailInfo { get; set; }

        public RoutingState Router { get; } = new RoutingState();
        public string UrlPathSegment { get; } = "ClientInformationView";

        public ReactiveCommand<Unit, IRoutableViewModel> GoToHomePage { get; }

        public IScreen HostScreen { get; }

        public ClientInformationViewModel()
        {
            GoToHomePage = ReactiveCommand.CreateFromObservable(
             () => Router.Navigate.Execute(new HomePageViewModel()));
            //HomePageViewModel.Client_ID

            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new Client.ClientClient(channel);

            AllClients info = client.searchClientsByName(new ClientName() { CName = HomePageViewModel.ClientName });
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Selection: " + info.Clients[0].FirstName).Show();
            if (info.Clients.Count > 0)
            {
                var c = info.Clients[0];
                ClientFirstNameInfo = c.FirstName;
                ClientLastNameInfo = c.LastName;
                ClientPhoneNumberInfo = c.PhoneNumber;
                ClientEmailInfo = c.Email;
            }
        }
        public void goToHomePageCommand()
        {
            GoToHomePage.Execute();
        }
    }
}
