using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class ProcedureReadView : ReactiveUserControl<ProcedureReadViewModel>
    {
        public ProcedureReadView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
