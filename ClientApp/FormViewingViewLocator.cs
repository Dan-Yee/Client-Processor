using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;
using System;

namespace ClientApp
{
    public class FormViewingViewLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            FormViewingViewModel context => new FormViewingView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
