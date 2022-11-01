using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class AdminLoginView : Window
    {
        public AdminLoginView()
        {
            InitializeComponent();
        }
        public AdminLoginView(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new AdminLoginViewModel(this,user,isAdmin);
        }
    }
}
