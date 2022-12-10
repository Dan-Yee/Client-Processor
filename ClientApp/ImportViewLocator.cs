using System;
using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;

namespace ClientApp
{
    internal class ImportViewLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            ImportFormViewModel context => new ImportFormView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };


    }
}
