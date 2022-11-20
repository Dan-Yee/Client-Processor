using Avalonia.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class MakeProcedureView : Window
    {
        public MakeProcedureView()
        {
            InitializeComponent();
            //DataContext = new MakeProcedureViewModel(this);
        }

        public MakeProcedureView(int c_ID)
        {
            InitializeComponent();
            DataContext = new MakeProcedureViewModel(this, c_ID);
        }
        /*
        public MakeProcedureView(string user, bool isAdmin, int c_ID)
        {
            InitializeComponent();
            DataContext = new MakeProcedureViewModel(this, user, isAdmin,c_ID);
        }
        */
    }
}