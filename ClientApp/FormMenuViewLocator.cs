using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;
using System;

namespace ClientApp
{
    public class FormMenuViewLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            FormMenuViewModel context => new FormMenuView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
}
}
