using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class CreateCustomerPage : ReactiveUserControl<CreateCustomerViewModel>
    {
        public CreateCustomerPage()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            //Subscribles click event to button
            this.FindControl<Button>("RegisterBtn").Click += Button_Click;
        }
        /// <summary>
        /// Onclick event for button. Will display that field(s) are blank
        /// If the fields are all filled out, the page will move to home view, so the user won't see the message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            //Sets textblock to display that the user has to fill in empty fields
            this.FindControl<TextBlock>("Warning").Text = "Please fill in required fields.";
        }
    }
}
