using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class AdminLoginView : Window
    {
        public AdminLoginView()
        {
            InitializeComponent();
            DataContext = new AdminLoginViewModel(this);
        }
    }
}
