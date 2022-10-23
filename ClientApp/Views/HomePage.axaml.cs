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
    }
}
