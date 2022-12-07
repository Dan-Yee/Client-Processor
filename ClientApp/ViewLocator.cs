using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ClientApp.ViewModels;
using ClientApp.Views;
using ReactiveUI;
using System;

namespace ClientApp
{
    /*
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
    */
    public class ViewLocator : ReactiveUI.IViewLocator
    {

        IViewFor? IViewLocator.ResolveView<T>(T viewModel, string? contract) => viewModel switch
        {
            HomePageViewModel context => new HomePage { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))


        };

    }

}
