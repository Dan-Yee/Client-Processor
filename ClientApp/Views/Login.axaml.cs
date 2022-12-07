using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Windows.Input;

namespace ClientApp.Views
{
    public partial class LoginPage : ReactiveUserControl<LoginPageViewModel>
    {
        public LoginPage()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            //Makes it so that button can't be clicked on. Will turn true when there are text values for username and password.
            this.FindControl<Button>("loginBtn").IsEnabled = false;
            //Subscribes login button to click event
            this.FindControl<Button>("loginBtn").Click += Button_Click;
            //Subscribes user name input to key up event
            this.FindControl<TextBox>("UsernameInput").KeyUp += TextUpdate;
            //Subscribes password input to key up event
            this.FindControl<TextBox>("PasswordInput").KeyUp+= TextUpdate;

        }

        /// <summary>
        /// Checks user name input and password input for values. If both input values are not empty, the login button will be enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextUpdate(object? sender, RoutedEventArgs e)
        {
            var userinput = this.FindControl<TextBox>("UsernameInput").Text;
            var passwordinput = this.FindControl<TextBox>("PasswordInput").Text;
            //If there are user name and password values
            if (userinput!=null && userinput != "" && passwordinput!=null&& passwordinput != "")
            {
                //Enable the button to be clicked
                this.FindControl<Button>("loginBtn").IsEnabled = true;
            }
            else
            {
                //un-enables the button to be clicked
                this.FindControl<Button>("loginBtn").IsEnabled = false;
            }
        }

        /// <summary>
        /// Onclick event for button. Will display that the credentials are invalid.
        /// If the credentials are valid, the page will move to home view, so the user won't see the message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            //Sets textblock to display that the user had an invalid login attempt
            this.FindControl<TextBlock>("Warning").Text = "Invalid credentials ("+ LoginPageViewModel.InvalidCredentialsCount + ")";
            //Increment the amount of login attempts
            LoginPageViewModel.InvalidCredentialsCount++;
        }
    }
}
