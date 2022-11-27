using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class FormViewingViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => "FormViewingViewModel";

        public IScreen HostScreen { get; }
        public RoutingState BackRouter { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoBack { get; }

        public FormViewingViewModel()
        {
            GoBack = ReactiveCommand.CreateFromObservable(
             () => BackRouter.Navigate.Execute(new ProcedureReadViewModel()));
        }

        public void GoBackCommand()
        {
            GoBack.Execute();
        }
    }
}
