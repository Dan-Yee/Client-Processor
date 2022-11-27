using Grpc.Net.Client;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class ProcedureReadViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => "ProcedureReadView";

        public IScreen HostScreen { get; }

        public string NameOfProcedure { get; set; }

        public RoutingState ViewPhotosRouter { get; } = new RoutingState();
        public RoutingState ViewFormRouter { get; } = new RoutingState();
        public RoutingState ProcedureViewingRouter { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> GoToViewPhotos { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> GoToViewForms { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> GoBackToListing { get; }

        public ProcedureReadViewModel()
        {

            //ClientProcedureListingViewModel.Procedure_Id
            //ClientProcedureListingViewModel.SelectedProcedure
            NameOfProcedure = ClientProcedureListingViewModel.SelectedProcedure.ProcedureName;
            GoToViewPhotos = ReactiveCommand.CreateFromObservable(
              () => ViewPhotosRouter.Navigate.Execute(new PhotosViewingViewModel()));
            GoToViewForms = ReactiveCommand.CreateFromObservable(
              () => ViewFormRouter.Navigate.Execute(new FormViewingViewModel()));
            GoBackToListing = ReactiveCommand.CreateFromObservable(
              () => ProcedureViewingRouter.Navigate.Execute(new ClientProcedureListingViewModel(HomePageViewModel.Client_ID)));
        }

        public void GoToFormViewingMenu()
        {
            GoToViewForms.Execute();
        }

        public void GoToPhotosViewingMenu()
        {
            GoToViewPhotos.Execute();
        }

        public void GoToProcedureListingCommand()
        {
            GoBackToListing.Execute();
        }
    }
}
