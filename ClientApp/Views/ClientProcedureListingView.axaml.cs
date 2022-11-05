using Avalonia.Controls;
using ClientApp.ViewModels;
using Server;

namespace ClientApp.Views
{
    public partial class ClientProcedureListingView : Window
    {
        public ClientProcedureListingView()
        {
            InitializeComponent();
            //DataContext = new ClientProcedureListingViewModel(this);
        }

        public ClientProcedureListingView(int client_ID)
        {
            InitializeComponent();
            DataContext = new ClientProcedureListingViewModel(this, client_ID);
        }
        /*
        public ClientProcedureListingView(string user, bool isAdmin, int client_ID)
        {
            InitializeComponent();
            DataContext = new ClientProcedureListingViewModel(this, user, isAdmin,client_ID);
        }
        */
        
    }
}
