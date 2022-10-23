using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class RegisterEmployeeView : Window
    {
        public RegisterEmployeeView()
        {
            InitializeComponent();
            DataContext = new RegisterEmployeeViewModel(this);
        }
    }
}
