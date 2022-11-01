using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class RegisterEmployeeView : Window
    {
        public RegisterEmployeeView()
        {
            InitializeComponent();
        }
        public RegisterEmployeeView(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new RegisterEmployeeViewModel(this,user,isAdmin);
        }
    }
}
