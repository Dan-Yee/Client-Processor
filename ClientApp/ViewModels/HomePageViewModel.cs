using ClientApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia.Controls.ApplicationLifetimes;
using System.Windows.Input;
using ClientApp.Views;
using Avalonia.Controls;
using Grpc.Net.Client;
using Server;

namespace ClientApp.ViewModels
{
    public class HomePageViewModel: ReactiveObject
    {

        //private ObservableCollection<string> _items = new();
        private ObservableCollection<Customer> _items = new();

        private HomePage _homePage;
        public HomePageViewModel(HomePage hp)
        {
            // localhost for testing purposes
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new Client.ClientClient(channel);
            
            AllClients info = client.getClients(new Google.Protobuf.WellKnownTypes.Empty());
            foreach (ClientInfo c in info.Clients)
            {
                _items.Add(new Customer(c.ClientId, c.FirstName, c.LastName, c.PhoneNumber, c.Email));
            }
            Items = _items;

            _homePage = hp;
        }

        //Binded onclick event
        public void CreateCustomerCommand()
        {
            new CreateCustomerPage().Show();
            _homePage.Close();
        }

        public void GoToAdminLoginCommand()
        {
            new AdminLoginView().Show();
            _homePage.Close();
        }
        
        public ObservableCollection<Customer> Items
        {
            get => _items;
            set
            {
                this.RaiseAndSetIfChanged(ref _items, value);
            }
        }
        
    }
}
