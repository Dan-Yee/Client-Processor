using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class ImportFormView : Window
    {
        public ImportFormView()
        {
            InitializeComponent();
            DataContext = new ImportFormViewModel(this);
        }
    }
}
