using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class AdminHomeView : Window
    {
        public AdminHomeView()
        {
            InitializeComponent();
        }
        public AdminHomeView(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new AdminHomeViewModel(this,user,isAdmin);
        }
    }
}
