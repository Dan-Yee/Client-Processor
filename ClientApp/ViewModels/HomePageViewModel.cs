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
using System.Reactive;
using Microsoft.AspNetCore.Components.Routing;
using System.ComponentModel;

namespace ClientApp.ViewModels
{
    
    public class HomePageViewModel : ReactiveObject, IRoutableViewModel
    {
        //Holds whether the user has admin privilages or not
        public bool ShowAdminButton { get; set; }
   
        //Determines if an element has been selected in the list view
        private bool _selectButtonEnabled;
        public bool SelectButtonEnabled
        {
            get
            {
                return _selectButtonEnabled;
            }
            set
            {
                _selectButtonEnabled  = value;
                //Updates that a value has been selected
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectButtonEnabled)));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        //The client list
        public ObservableCollection<Customer> Items
        {
            get => _items;
            set
            {
                this.RaiseAndSetIfChanged(ref _items, value);
            }
        }

        
        public SelectionModel<Customer> Selection { get; }

        private ObservableCollection<Customer> _items = new();
        

        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "Homepage";

        public RoutingState Router { get; } = new RoutingState();

        public RoutingState Router0 { get; } = new RoutingState();

        public RoutingState Router1 { get; } = new RoutingState();

        public RoutingState Router2 { get; } = new RoutingState();

        public RoutingState Router3 { get; } = new RoutingState();

        public RoutingState RouterToLogin { get; } = new RoutingState();


        // The command that navigates a user to a view model.
        public ReactiveCommand<Unit, IRoutableViewModel> GoCreateCustomerCommand { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoGoToAdminLoginCommand { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoGoToAdminHomeCommand { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoGoToClientProceduresCommand { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> BackToLogin { get; }
       

        public ReactiveCommand<Unit, IRoutableViewModel> GoToLogin { get; }

        /// <summary>
        /// Constructor for the viewmodel. initializes the list of employees
        /// </summary>
        public HomePageViewModel()
        {
            
            // localhost for testing purposes
            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new Client.ClientClient(channel);

            AllClients info = client.getClients(new Google.Protobuf.WellKnownTypes.Empty());
            foreach (ClientInfo c in info.Clients)
            {
                //Adds employees to the list
                _items.Add(new Customer(c.ClientId, c.FirstName, c.LastName, c.PhoneNumber, c.Email));
            }
            Items = _items;

            //enables employee selection
            Selection = new SelectionModel<Customer>();
            Selection.SelectionChanged += SelectionChanged;
            SelectButtonEnabled = false;
            ShowAdminButton = LoginPageViewModel.GlobalIsAdmin;


          GoGoToClientProceduresCommand = ReactiveCommand.CreateFromObservable(
              () => Router0.Navigate.Execute(new ClientProcedureListingViewModel(Selection.SelectedItem.Client_ID)));
          GoCreateCustomerCommand = ReactiveCommand.CreateFromObservable(
             () => Router1.Navigate.Execute(new CreateCustomerViewModel()));
            GoGoToAdminHomeCommand = ReactiveCommand.CreateFromObservable(
              () => Router2.Navigate.Execute(new AdminHomeViewModel()));
            GoToLogin = ReactiveCommand.CreateFromObservable(
             () => RouterToLogin.Navigate.Execute(new LoginPageViewModel()));

            
        }

        /* Event Handlers Below */

        /// <summary>
        /// On click event to creating customer button
        /// </summary>
        public void CreateCustomerCommand()
        {
            GoCreateCustomerCommand.Execute();
           
        }
       

        /// <summary>
        /// Takes user to admin home view
        /// </summary>
        public void GoToAdminHomeCommand()
        {
            GoGoToAdminHomeCommand.Execute();
            
        }

        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
            SelectButtonEnabled = true;
        }

        /// <summary>
        /// Logs out user
        /// </summary>
        public void LogoutCommand()
        {
            GoToLogin.Execute();
          
        }

        /// <summary>
        /// Once client is selected, goes to the procedure listing page
        /// </summary>
        public void GoToClientProceduresCommand()
        {
            //If a client is selected
            if (Selection != null)
            {
                
                GoGoToClientProceduresCommand.Execute();
                var loginSuccessMessage = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Selection: " + Selection.SelectedItem.Client_ID);
                loginSuccessMessage.Show();
                
            }
            else
            {
                var loginSuccessMessage = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Selection: null");
                loginSuccessMessage.Show();
            }

        }
    }
}
