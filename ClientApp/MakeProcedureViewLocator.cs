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
    public class MakeProcedureViewLocator : ReactiveUI.IViewLocator
    {
        /*
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            MakeProcedureViewModel context => new MakeProcedureView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))


        };
        */
        public IViewFor? ResolveView<T>(T viewModel, string? contract = null)
        {
            throw new NotImplementedException();
        }
    }
}
