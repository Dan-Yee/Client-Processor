using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;
using System;

namespace ClientApp
{
    public class EmployeeInformationViewLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            EmployeeInformationViewModel context => new EmployeeInformationView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))


        };
    }
}
