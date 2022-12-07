using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;

namespace ClientApp.Views
{
    public partial class UpdateEmployeeView : ReactiveUserControl<UpdateEmployeeViewModel>
    {
        public UpdateEmployeeView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            //Subscribes button to event listener so that the value can be reset
            this.FindControl<Button>("btnToResetFirstName").Click += ResetFirstName;
            //Subscribes button to event listener so that the value can be reset
            this.FindControl<Button>("btnToResetLastName").Click += ResetLastName;
            //Subscribes button to event listener so that the value can be reset
            this.FindControl<Button>("btnToResetUserName").Click += ResetUserName;
            //Subscribes button to event listener so that the value can be reset
            this.FindControl<Button>("btnToResetPassword").Click += ResetPassword;
        }
        
        /// <summary>
        /// </summary>
        /// Resets text to original value
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetFirstName(object? sender, RoutedEventArgs e)
        {
            this.FindControl<TextBox>("FirstNameInput").Text = UpdateEmployeeViewModel.CurrentFirstNameInfo;
        }
        
        /// <summary>
        /// </summary>
        /// Resets text to original value
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetLastName(object? sender, RoutedEventArgs e)
        {
            this.FindControl<TextBox>("LastNameInput").Text = UpdateEmployeeViewModel.CurrentLastNameInfo;
        }

        /// <summary>
        /// </summary>
        /// Resets text to original value
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetUserName(object? sender, RoutedEventArgs e)
        {
            this.FindControl<TextBox>("UserNameInput").Text = UpdateEmployeeViewModel.CurrentUserNameInfo;
        }

        /// <summary>
        /// </summary>
        /// Resets text to original value
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetPassword(object? sender, RoutedEventArgs e)
        {
            this.FindControl<TextBox>("PasswordInput").Text = UpdateEmployeeViewModel.CurrentPasswordInfo;
        }
    }
}
