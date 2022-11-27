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
    public class ProcedureReadViewLocator : ReactiveUI.IViewLocator
    {

        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            ProcedureReadViewModel context => new ProcedureReadView{ DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
