using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class HomePage : ReactiveUserControl<HomePageViewModel>
    {
        public HomePage()
        {

            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
        }

    }
}
