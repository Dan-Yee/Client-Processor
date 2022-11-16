using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class RegisterEmployeeView : ReactiveUserControl<RegisterEmployeeViewModel>
    {
        public RegisterEmployeeView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
        }
        /*
        public RegisterEmployeeView(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new RegisterEmployeeViewModel(this,user,isAdmin);
        }
        */
    }
}