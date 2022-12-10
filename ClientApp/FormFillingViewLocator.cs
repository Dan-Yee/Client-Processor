using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;
using System;

namespace ClientApp
{
    public class FormFillingViewLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            FormFillingViewModel context => new FormFillingView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
