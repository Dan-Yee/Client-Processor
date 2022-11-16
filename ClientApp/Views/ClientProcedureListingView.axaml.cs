using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;
using Server;

namespace ClientApp.Views
{
    public partial class ClientProcedureListingView : ReactiveUserControl<ClientProcedureListingViewModel>
    {
        public ClientProcedureListingView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            //InitializeComponent();
            //DataContext = new ClientProcedureListingViewModel(this);
        }
        /*
        public ClientProcedureListingView(int client_ID)
        {
            //InitializeComponent();
            //DataContext = new ClientProcedureListingViewModel(client_ID);
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
