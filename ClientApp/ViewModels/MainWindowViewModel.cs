using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using Microsoft.AspNetCore.Components.Routing;
using ReactiveUI;

namespace ClientApp.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }

        public string UrlPathSegment { get; } = "Login";

        // Required by the IScreen interface.
        public RoutingState RouterToLogin { get; } = new RoutingState();

        


        public string Greeting => "Welcome to the Business";
        /// <summary>
        /// GoToLogin goes to the login page
        /// </summary>
        public MainWindowViewModel()
        {
            RouterToLogin.Navigate.Execute(new LoginPageViewModel());
            
        }

    }
}
