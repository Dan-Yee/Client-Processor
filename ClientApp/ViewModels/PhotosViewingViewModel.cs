using Microsoft.AspNetCore.Components.Routing;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class PhotosViewingViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => "PhotosViewingViewModel";

        public IScreen HostScreen { get; }
        
        public RoutingState BackRouter { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoBack { get; }

        public PhotosViewingViewModel()
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
