using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;

namespace ClientApp
{
    public class AdminLoginViewLocator : ReactiveUI.IViewLocator
    {

        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            AdminLoginViewModel context => new AdminLoginView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))


        };

    }
}
