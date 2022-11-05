using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class HomePage : Window
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = new HomePageViewModel(this);
        }
        
        /*
        public HomePage(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new HomePageViewModel(this,user,isAdmin);
        }
        */
        
    }
}
