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
    internal class CreateProcedureListingLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            ClientProcedureListingViewModel context => new ClientProcedureListingView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };


    }
}
