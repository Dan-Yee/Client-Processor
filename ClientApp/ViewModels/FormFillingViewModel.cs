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
        /*
        {
            new FormInputField
            {
                FieldName = "First Name:",
                IsTextInput = true,
                IsCheckBox = false,
                IsRadioBtn = false,
                CheckListParameters = null
            },
            new FormInputField
            {
                FieldName = "Is appointment today:",
                IsTextInput = false,
                IsCheckBox = false,
                IsRadioBtn = true,
                CheckListParameters = new List<OptionModel>()
                {
                    new("Yes"),
                    new("No")
                }
            },
            new FormInputField
            {
                FieldName = "Appointment type:",
                IsTextInput = false,
                IsCheckBox = true,
                IsRadioBtn = false,
                CheckListParameters = new List<OptionModel>()
                {
                    new("Hair cut"),
                    new("Nails")
                }
            },
            new FormInputField
            {
                FieldName = "Last Name:",
                IsTextInput = true,
                IsCheckBox = false,
                IsRadioBtn = false,
                CheckListParameters = null
            },
        };
        */

        
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
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Object: " + value).Show();
                //this.RaiseAndSetIfChanged(ref _TextInputList, value);
                OnPropertyChanged(nameof(_TextInputList));
            }
        }
        
        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();

        public GrpcChannel channel;
        public Server.Procedure.ProcedureClient procedureClient;
        public FormName name;
        public Server.FormFields formFields;


        public FormFillingViewModel(FormFillingView ffv,int c_ID,string filename)
        {
            _clientId = c_ID;
            _formFillingView = ffv;
            _fileName = filename;

            channel = GrpcChannel.ForAddress("https://localhost:7123");
            procedureClient = new Procedure.ProcedureClient(channel);
            name = new() { FormName_ = _fileName };
            formFields = procedureClient.getFormFields(name);
            //List<FormInputField> InputFields = new List<FormInputField>();
            //MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Object: " + grid).Show();
            int index = 0;
            int lengthOfFields = formFields.Fields.Count;
            
            foreach (var field in formFields.Fields)
            {
                if (field.FieldType.Equals("iText.Forms.Fields.PdfTextFormField"))
                {
                    //InputFields.Add(new FormInputField(field.FieldName, true, false, false, null));
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
            /*
            foreach (TextBox textBox in _formFillingView.FindControl<StackPanel>("notes").Children.OfType<TextBox>().ToList())
            {
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Object: " + textBox.Text).Show();
            }
            */
            int index = 0;
            List<TextBox> textBoxesForInput = _formFillingView.FindControl<StackPanel>("notes").Children.OfType<TextBox>().ToList();
            foreach (var field in formFields.Fields)
            {
                field.FieldValue = textBoxesForInput[index].Text;
                index++;
            }
                /*foreach (var inputThing in TextInputList)
                {
                   MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Object: " + inputThing).Show();
                }*/
                //MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Object: " + inputString).Show();
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
