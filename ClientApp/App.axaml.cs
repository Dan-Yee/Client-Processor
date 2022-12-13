using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ClientApp.Views;

namespace ClientApp
{
    public partial class App : Application
    {
        
        
        public App()
        {
        }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                
                desktop.MainWindow = new MainWindow();
                
                /*
                {
                    DataContext = new LoginPageViewModel(desktop),
                };
                */

            }

            base.OnFrameworkInitializationCompleted();

        }
    }
}
