using ClientApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using Grpc.Net.Client;
using Server;
using Avalonia.Controls.Selection;
using System.Reactive;
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
                _selectButtonEnabled = value;
                //Updates that a value has been selected
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectButtonEnabled)));
            }
        }

        private string searchNameTextInput;
        public string SearchNameTextInput
        {
            get
            {
                return searchNameTextInput;
            }
            set
            {
                searchNameTextInput = value;
                //Search for the updated string
                SearchForClientCommand();
            }
        }

        //Holds the client name
        public static string ClientName { get; set; }

        //Holds the client Id
        public static int Client_ID { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        //The client list
        public ObservableCollection<Customer> CustomerItems { get; set; } = new();
        
        //Holds the Ids of the clients that are listed
        public List<int> ListOfClientIDs { get; set; } = new();

        //Holds the selected client
        public SelectionModel<Customer> ClientSelection { get; } = new();

        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "Homepage";

        public RoutingState RouterToClientProcedureListing { get; } = new RoutingState();

        public RoutingState RouterToCreateCustomer { get; } = new RoutingState();

        public RoutingState RouterToAdminHome { get; } = new RoutingState();

        public RoutingState RouterToClientInformation { get; } = new RoutingState();

        public RoutingState RouterToLogin { get; } = new RoutingState();
        public RoutingState RouterToUpdateClientInfo{ get; } = new RoutingState();


        // The command that navigates a user to a view model.
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToCreateCustomer { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToAdminHome { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToClientProcedures { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToClientInformation { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToLogin { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToUpdateClientInfo { get; }

        

        /// <summary>
        /// Constructor for the viewmodel. initializes the list of employees
        /// </summary>
        public HomePageViewModel()
        {
            //Subscribes the selection event listener
            ClientSelection.SelectionChanged += SelectionChanged;
            //Sets button to false when page is loaded
            SelectButtonEnabled = false;
            //Determines whether or not to show the admin button
            ShowAdminButton = LoginPageViewModel.GlobalIsAdmin;


            NavigateToClientProcedures = ReactiveCommand.CreateFromObservable(
                () => RouterToClientProcedureListing.Navigate.Execute(new ClientProcedureListingViewModel()));
            NavigateToCreateCustomer = ReactiveCommand.CreateFromObservable(
               () => RouterToCreateCustomer.Navigate.Execute(new CreateCustomerViewModel()));
            NavigateToAdminHome = ReactiveCommand.CreateFromObservable(
              () => RouterToAdminHome.Navigate.Execute(new AdminHomeViewModel()));
            NavigateToLogin = ReactiveCommand.CreateFromObservable(
             () => RouterToLogin.Navigate.Execute(new LoginPageViewModel()));
            NavigateToClientInformation = ReactiveCommand.CreateFromObservable(
              () => RouterToClientInformation.Navigate.Execute(new ClientInformationViewModel()));
            NavigateToUpdateClientInfo = ReactiveCommand.CreateFromObservable(
              () => RouterToUpdateClientInfo.Navigate.Execute(new UpdateClientViewModel()));

        }

        /* Event Handlers Below */
        public void SearchForClientCommand()
        {
            //Clear displayed list
            if(CustomerItems!=null)CustomerItems.Clear();
            //Clear Id list associated with displayed clients
            ListOfClientIDs.Clear();

            //If there is a name to search
            if (SearchNameTextInput != null && SearchNameTextInput.Length > 0)
            {
                var client = new Client.ClientClient(Program.gRPCChannel);
                AllClients info = client.searchClientsByName(new ClientName() { CName = SearchNameTextInput });
                if (info.Clients.Count > 0)
                {
                    foreach (var clientInformation in info.Clients)
                    {
                        CustomerItems.Add(new Customer(clientInformation.ClientId, clientInformation.FirstName, clientInformation.LastName, clientInformation.PhoneNumber, clientInformation.Email));

                        ListOfClientIDs.Add(clientInformation.ClientId);
                    }
                }
                else
                {
                    //There are no clients to pick from, so disables buttons
                    SelectButtonEnabled = false;
                }
            }
            else
            {
                //There are no clients to pick from, so disables buttons
                SelectButtonEnabled = false;
            }
        }

        /// <summary>
        /// On click event to creating customer button
        /// </summary>
        public void CreateCustomerCommand()
        {
            NavigateToCreateCustomer.Execute();
        }


        /// <summary>
        /// Takes user to admin home view
        /// </summary>
        public void GoToAdminHomeCommand()
        {
            NavigateToAdminHome.Execute();
        }

        public void GoGoToClientInformationCommand()
        {
                NavigateToClientInformation.Execute();
        }

        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
            SelectButtonEnabled = true;
            Client_ID = ListOfClientIDs[ClientSelection.SelectedIndex];
            ClientName = ClientSelection.SelectedItem.FirstName;
        }

        /// <summary>
        /// Logs out user
        /// </summary>
        public void LogoutCommand()
        {
            NavigateToLogin.Execute();
        }

        /// <summary>
        /// Once client is selected, goes to the procedure listing page
        /// </summary>
        public void GoToClientProceduresCommand()
        {
            //If a client is selected
            if (ClientSelection != null)
            {
                NavigateToClientProcedures.Execute();
            }
        }

        public void GoToUpdateClientCommand()
        {
            NavigateToUpdateClientInfo.Execute();
        }
    }
}
