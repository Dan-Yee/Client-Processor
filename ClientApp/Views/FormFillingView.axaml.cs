using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class FormFillingView : Window
    {
        public FormFillingView()
        {

        }
        public FormFillingView(int c_ID,string filename)
        {
            InitializeComponent();
            DataContext = new FormFillingViewModel(this, c_ID,filename);
        }
    }
}
