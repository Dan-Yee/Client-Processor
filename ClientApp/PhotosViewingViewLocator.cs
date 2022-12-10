using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;
using System;

namespace ClientApp
{
    public class PhotosViewingViewLocator : ReactiveUI.IViewLocator
    {
        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            PhotosViewingViewModel context => new PhotosViewingView{ DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
