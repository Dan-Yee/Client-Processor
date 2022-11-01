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
using Avalonia.Controls.Selection;

namespace ClientApp.ViewModels
{
    public class HomePageViewModel: ReactiveObject
    {
        string _user = string.Empty;
        private bool _isAdmin=false;
        
        
        public bool IsAdmin
        {
            get => _isAdmin;
            set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
        }

        //private ObservableCollection<string> _items = new();
        private ObservableCollection<Customer> _items = new();

        private HomePage _homePage;
        public HomePageViewModel(HomePage hp,string user, bool isAdmin)
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

            Selection = new SelectionModel<Customer>();
            Selection.SelectionChanged += SelectionChanged;

            _user = user;
            _isAdmin = isAdmin;
            //IsAdmin = false;
            

            _homePage = hp;
        }

        //Binded onclick event
        public void CreateCustomerCommand()
        {
            new CreateCustomerPage(_user, IsAdmin).Show();
            _homePage.Close();
        }

        /*public void GoToAdminLoginCommand()
        {
            new AdminLoginView().Show();
            _homePage.Close();
        }*/

        public void GoToAdminHomeCommand()
        {
            new AdminHomeView(_user, IsAdmin).Show();
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

        public SelectionModel<Customer> Selection { get; }

        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
        }

        public void LogoutCommand()
        {
            new LoginPage().Show();
            _homePage.Close();
        }
        public void GoToClientProceduresCommand()
        {
            if(Selection != null)
            {
                new ClientProcedureListingView().Show();
                var loginSuccessMessage = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Selection: " + Selection.SelectedItem.Client_ID);
                loginSuccessMessage.Show();
                _homePage.Close();
            }
            else
            {
                var loginSuccessMessage = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Selection: null");
                loginSuccessMessage.Show();
            }
            
        }
    }
}
