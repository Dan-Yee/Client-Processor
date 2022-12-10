using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;
using System;

namespace ClientApp
{
    public class InitializeProcedureViewLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            InitializeProcedureViewModel context => new InitializeProcedureView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))


        };
    }
}
