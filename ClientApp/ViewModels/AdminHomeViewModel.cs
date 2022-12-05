using ClientApp.Views;
using Grpc.Net.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using Server;
using System.Reactive;
using System.ComponentModel;
using Avalonia.Controls.Selection;
using AvaloniaEdit.Editing;
using ClientApp.Models;
using MessageBox.Avalonia.Enums;

namespace ClientApp.ViewModels
{
    public class AdminHomeViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }

        //Holds onto the selected employee so that other pages can access the employee's information
        public static EmployeeModel? SelectedEmployee { get; set; }

        //Current employee selected
        public SelectionModel<EmployeeModel> EmployeeSelection { get; } = new();

        public string UrlPathSegment { get; } = "AdminHome";
        public RoutingState RouterToViewEmployee { get; } = new RoutingState();
        public RoutingState RouterHomePageProcedure { get; } = new RoutingState();

        public RoutingState RouterToImport { get; } = new RoutingState();

        public RoutingState RouterRegister { get; } = new RoutingState();
        public RoutingState RouterToUpdateEmployeeInfo { get; } = new RoutingState();



        public ReactiveCommand<Unit, IRoutableViewModel> NavigateHome { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToImport { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToRegister { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToEmployeeInformation { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToUpdateEmployeeInfo { get; }


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
        public event PropertyChangedEventHandler PropertyChanged;

        //List of employees. Binded in view to display the employees
        private ObservableCollection<Models.EmployeeModel> _employees = new();
        public ObservableCollection<Models.EmployeeModel> Employees
        {
            get => _employees;
            set
            {
                this.RaiseAndSetIfChanged(ref _employees, value);
            }
        }


        public AdminHomeViewModel()
        {
            var client = new Server.Employee.EmployeeClient(Program.gRPCChannel);
            //Get employees from database
            AllEmployees info = client.getEmployees(new Google.Protobuf.WellKnownTypes.Empty());
            foreach (EmployeeInfo e in info.Employees)
            {
                //Add employees to the list
                _employees.Add(new Models.EmployeeModel(e.EmployeeId, e.FirstName, e.LastName, e.Credentials.Username, e.IsAdmin));
            }
            //Disables button every time the page is loaded
            SelectButtonEnabled = false;

            //Subscribes the selection to an event listener
            EmployeeSelection.SelectionChanged += SelectionChanged;

            NavigateHome = ReactiveCommand.CreateFromObservable(
             () => RouterHomePageProcedure.Navigate.Execute(new HomePageViewModel()));

            NavigateToImport = ReactiveCommand.CreateFromObservable(
             () => RouterToImport.Navigate.Execute(new ImportFormViewModel()));

            NavigateToRegister = ReactiveCommand.CreateFromObservable(
             () => RouterRegister.Navigate.Execute(new RegisterEmployeeViewModel()));

            NavigateToEmployeeInformation = ReactiveCommand.CreateFromObservable(
             () => RouterToViewEmployee.Navigate.Execute(new EmployeeInformationViewModel()));

            NavigateToUpdateEmployeeInfo = ReactiveCommand.CreateFromObservable(
             () => RouterToUpdateEmployeeInfo.Navigate.Execute(new UpdateEmployeeViewModel()));
        }

        /// <summary>
        /// When the user selects an employee, the button(s) are enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
            SelectButtonEnabled = true;
            SelectedEmployee = EmployeeSelection.SelectedItem;
        }

        /// <summary>
        /// Opens home page view and closes this view
        /// </summary>
        public void GoToHomeFromAdminHomeCommand()
        {
            NavigateHome.Execute();

        }

        /// <summary>
        /// Opens the importing form view and closes this view
        /// </summary>
        public void OpenImportFormView()
        {
            NavigateToImport.Execute();

        }

        /// <summary>
        /// Opens employee registration view and closes this view
        /// </summary>
        public void CreateEmployeeCommand()
        {
            NavigateToRegister.Execute();
        }

        /// <summary>
        /// Deletes employee from database
        /// </summary>
        public async void DeleteEmployeeCommand()
        {

            //Want yes no dialog here
            ButtonResult result = await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Confirm deletion", ButtonEnum.YesNo).Show();
            if (result == ButtonResult.Yes)
            {
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "To delete").Show();
            }
            else
            {
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Clicked no").Show();
            }
        }

        /// <summary>
        /// Takes user to view the employee information
        /// </summary>
        public void GoToReadEmployeeInfoCommand()
        {
            
            NavigateToEmployeeInformation.Execute();
        }
        public void GoToUpdateEmployeeInfoCommand()
        {
            NavigateToUpdateEmployeeInfo.Execute();
        }
    }
}
