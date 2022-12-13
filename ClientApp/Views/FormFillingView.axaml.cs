using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ClientApp.ViewModels;
using ReactiveUI;
using Server;
using System.Collections.Generic;
using System.Linq;

namespace ClientApp.Views
{
    public partial class FormFillingView : ReactiveUserControl<FormFillingViewModel>
    {
        List<string> ListOfFieldNames { get; set; } = new();

        public FormFillingView()
        {
            this.WhenActivated(disposables => { /* Handle interactions etc. */ });
            AvaloniaXamlLoader.Load(this);
            Button submitBtn = this.FindControl<Button>("SubmitBtn");
            submitBtn.Click += clickedSumbit;
            List<Server.Field> ListOfField = FormFillingViewModel.ListOfFields;
            foreach(var field in ListOfField)
            {
                ListOfFieldNames.Add(field.FieldName);
                TextBlock fieldName = new();
                fieldName.Text = field.FieldName;
                this.FindControl<StackPanel>("notes").Children.Add(fieldName);
                this.FindControl<StackPanel>("notes").Children.Add(new TextBox());
            }
        }
        public void clickedSumbit(object? sender, RoutedEventArgs e)
        {
            Server.FormFields myFields = new();
            List<TextBox> textBoxesForInput = this.FindControl<StackPanel>("notes").Children.OfType<TextBox>().ToList();
            for (int index = 0; index < textBoxesForInput.Count; index++)
            {
                if(textBoxesForInput[index] != null && textBoxesForInput[index].Text!=null)
                {
                    myFields.Fields.Add(new Field() { FieldName = ListOfFieldNames[index], FieldValue = textBoxesForInput[index].Text });
                }
                else
                {
                    myFields.Fields.Add(new Field() { FieldName = ListOfFieldNames[index], FieldValue = "" });
                }
                //MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", textBoxesForInput[index].Text).Show();
            }
            FormName fName = new() { FormName_ = FormMenuViewModel.FormName };
            myFields.FName = fName;
            CompleteFormInfo info = new() { ProcedureID = ClientProcedureListingViewModel.Procedure_Id, Form = myFields, EmployeeID = LoginPageViewModel.GlobalEmployeeID };
            var resultStatus = new Procedure.ProcedureClient(Program.gRPCChannel).completeForm(info);
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Status: " + resultStatus).Show();

            FormMenuViewModel.ListOfFilledOutFormNames.Add(FormMenuViewModel.FormName);

            //FormFillingViewModel.NavigateToFormMenu.Execute();
        }
        
        
    }
}
