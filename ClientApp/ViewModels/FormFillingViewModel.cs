using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using AvaloniaEdit.Editing;
using ClientApp.Models;
using ClientApp.Views;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using ReactiveUI;
using Server;
using static Server.Procedure;

namespace ClientApp.ViewModels
{
    public class FormFillingViewModel : ViewModelBase, IRoutableViewModel
    {
        public List<FormInputField> InputFields { get; } = new List<FormInputField>();
        public ObservableCollection<string> UserInputList { get; set; } = new ObservableCollection<string>();
        
        private int _clientId;
        private FormFillingView _formFillingView;
        private string _fileName;
        private List<string> _TextInputList = new List<string>();

        public event PropertyChangingEventHandler? PropertyChanging;

        public List<string> TextInputList
        {
            get => _TextInputList;
            set {
                _TextInputList = value;
                OnPropertyChanged(nameof(_TextInputList));
            }
        }
        
        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();

        public GrpcChannel channel;
        public Server.Procedure.ProcedureClient procedureClient;
        public FormName name;
        public Server.FormFields formFields;
        List<string> ListOfFieldNames = new();


        public FormFillingViewModel(FormFillingView ffv,int c_ID,string filename)
        {
            _clientId = c_ID;
            _formFillingView = ffv;
            _fileName = filename;

            channel = GrpcChannel.ForAddress("https://localhost:7123");
            procedureClient = new Procedure.ProcedureClient(channel);
            name = new() { FormName_ = _fileName };
            formFields = procedureClient.getFormFields(name);
            int index = 0;
            foreach (var field in formFields.Fields)
            {
                if (field.FieldType.Equals("iText.Forms.Fields.PdfTextFormField"))
                {
                    ListOfFieldNames.Add(field.FieldName);
                    TextBlock fieldName= new();
                    fieldName.Text = field.FieldName;
                    _formFillingView.FindControl<StackPanel>("notes").Children.Add(fieldName);
                    TextInputList.Add("");
                    TextBox t1 = new TextBox();
                    t1.Text = TextInputList[index];
                    index++;
                    _formFillingView.FindControl<StackPanel>("notes").Children.Add(t1);
                }
            }

        }

        public void SubmitFormCommand()
        {
            Server.FormFields myFields = new();
            FormName fName = new() { FormName_ = _fileName };
            List<TextBox> textBoxesForInput = _formFillingView.FindControl<StackPanel>("notes").Children.OfType<TextBox>().ToList();
            for(int index=0;index<textBoxesForInput.Count;index++)
            {
                myFields.Fields.Add(new Field() { FieldName = ListOfFieldNames[index], FieldValue = textBoxesForInput[index].Text });
             }
            myFields.FName = fName;
            CompleteFormInfo info = new() { ProcedureID = InitializeProcedureViewModel.CurrentProcedureID, Form = myFields };
            channel = GrpcChannel.ForAddress("https://localhost:7123");
            var resultStatus = new Procedure.ProcedureClient(channel).completeForm(info);
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Status: "+ resultStatus).Show();
            
            FormMenuViewModel.ListOfFilledOutFormNames.Add(name.FormName_);
            
            new FormMenuView(_clientId).Show();
            _formFillingView.Close();
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
