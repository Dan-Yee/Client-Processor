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
        private ObservableCollection<string> _items = new();

        private HomePage _homePage;
        public HomePageViewModel(HomePage hp)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
            //var client = new Server.Employee.EmployeeClient(channel);
            var client = new Client.ClientClient(channel);

            //var ClientApp = new Server.ClientApp.ClientAppClient(channel);
            //ClientApp.getClientApps(new Google.Protobuf.WellKnownTypes.Empty());
            _homePage = hp;
            var loginSuccessMessage = MessageBox.Avalonia.MessageBoxManager
    .GetMessageBoxStandardWindow("title", "works");
            loginSuccessMessage.Show();
            /*
            var items = new ObservableCollection<string>
            {
                "Customer1",
                "Customer2"
            };
            Items = items;
            */
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
        /*
        public ObservableCollection<string> Items
        {
            get => _items;
            set
            {
                this.RaiseAndSetIfChanged(ref _items, value);
            }
        }
        */
    }
}
