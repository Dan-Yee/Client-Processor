using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    internal class ClientInformationViewLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            ClientInformationViewModel context => new ClientInformationView{ DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))


        };


    }
}
