using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class CreateCustomerPage : ReactiveUserControl<CreateCustomerViewModel>
    {
        public CreateCustomerPage()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            //InitializeComponent();
            // DataContext = new CreateCustomerViewModel(this);
        }
        /*
        public CreateCustomerPage(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new CreateCustomerViewModel(this,user,isAdmin);
        }
        */
    }
}
