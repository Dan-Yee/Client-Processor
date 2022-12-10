using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class FormMenuView : ReactiveUserControl<FormMenuViewModel>
    {
        public FormMenuView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
