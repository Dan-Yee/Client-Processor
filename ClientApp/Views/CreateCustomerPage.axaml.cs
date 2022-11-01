using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class CreateCustomerPage : Window
    {
        public CreateCustomerPage()
        {
            InitializeComponent();
        }
        public CreateCustomerPage(string user,bool isAdmin)
        {
            InitializeComponent();
            DataContext = new CreateCustomerViewModel(this,user,isAdmin);
        }
    }
}
