using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class AdminHomeView : ReactiveUserControl<AdminHomeViewModel>
    {
        public AdminHomeView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            //InitializeComponent();
            //DataContext = new AdminHomeViewModel(this);
        }
        /*
        public AdminHomeView(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new AdminHomeViewModel(this,user,isAdmin);
        }
        */
    }
}
