using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class UpdateClientView : ReactiveUserControl<UpdateClientViewModel>
    {
        public UpdateClientView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            //Subscribes button to event listener so that the value can be reset
            this.FindControl<Button>("btnToResetFirstName").Click += ResetFirstName;
            //Subscribes button to event listener so that the value can be reset
            this.FindControl<Button>("btnToResetLastName").Click += ResetLastName;
            //Subscribes button to event listener so that the value can be reset
            this.FindControl<Button>("btnToResetPhoneNumber").Click += ResetPhoneNumber;
            //Subscribes button to event listener so that the value can be reset
            this.FindControl<Button>("btnToResetEmail").Click += ResetEmail;
        }

        /// <summary>
        /// </summary>
        /// Resets text to original value
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetFirstName(object? sender, RoutedEventArgs e)
        {
            this.FindControl<TextBox>("FirstNameInput").Text = UpdateClientViewModel.CurrentFirstName;
        }

        /// <summary>
        /// </summary>
        /// Resets text to original value
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetLastName(object? sender, RoutedEventArgs e)
        {
            this.FindControl<TextBox>("LastNameInput").Text = UpdateClientViewModel.CurrentLastName;
        }

        /// <summary>
        /// </summary>
        /// Resets text to original value
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetPhoneNumber(object? sender, RoutedEventArgs e)
        {
            this.FindControl<TextBox>("PhoneNumberInput").Text = UpdateClientViewModel.CurrentPhoneNumber;
        }

        /// <summary>
        /// </summary>
        /// Resets text to original value
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetEmail(object? sender, RoutedEventArgs e)
        {
            this.FindControl<TextBox>("EmailInput").Text = UpdateClientViewModel.CurrentEmail;
        }
    }
}
