using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class CreateCustomerPage : Window
    {
        public CreateCustomerPage()
        {
            InitializeComponent();
            DataContext = new CreateCustomerViewModel(this);
        }
    }
}
