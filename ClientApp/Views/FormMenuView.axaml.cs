using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class FormMenuView : Window
    { 
        public FormMenuView()
        {
            InitializeComponent();
        }
        public FormMenuView(int c_ID)
        {
            InitializeComponent();
            DataContext = new FormMenuViewModel(this, c_ID);
        }
    }
}
