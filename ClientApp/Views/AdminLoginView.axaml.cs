using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class AdminLoginView : ReactiveUserControl<AdminLoginViewModel>
    {
        public AdminLoginView()
        {

            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            //InitializeComponent();
            //DataContext = new AdminLoginViewModel(this);
        }
        /*
        public AdminLoginView(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new AdminLoginViewModel(this,user,isAdmin);
        }
        */
    }
}
