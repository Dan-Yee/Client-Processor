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
        }

    }
}
