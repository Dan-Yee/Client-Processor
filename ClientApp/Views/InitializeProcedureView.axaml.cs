using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class InitializeProcedureView : Window
    {
        public InitializeProcedureView()
        {
            InitializeComponent();
        }
        public InitializeProcedureView(int c_id)
        {
            InitializeComponent();
            DataContext = new InitializeProcedureViewModel(this, c_id);
        }
    }
}
