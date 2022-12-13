using ClientApp.Models;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;

namespace ClientApp.ViewModels
{
    public class FormFillingViewModel : ViewModelBase, IRoutableViewModel
    {
        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();

        public event PropertyChangingEventHandler? PropertyChanging;

        public List<FormInputField> InputFields { get; } = new List<FormInputField>();
        public ObservableCollection<string> UserInputList { get; set; } = new ObservableCollection<string>();

        private List<string> _TextInputList = new List<string>();

        public List<string> TextInputList
        {
            get => _TextInputList;
            set
            {
                _TextInputList = value;
                OnPropertyChanged(nameof(_TextInputList));
            }
        }

        public Server.Procedure.ProcedureClient procedureClient;
        public FormName name;
        public Server.FormFields formFields;
        List<string> ListOfFieldNames = new();

        public static List<Server.Field> ListOfFields { get; set; } = new();

        public static RoutingState RouterToFormMenu { get; set; } = new RoutingState();
        public static ReactiveCommand<Unit, IRoutableViewModel> NavigateToFormMenu { get; set; }
        public FormFillingViewModel()
        {
            ListOfFields = new();
            RouterToFormMenu = new();
            NavigateToFormMenu = ReactiveCommand.CreateFromObservable(
                () => RouterToFormMenu.Navigate.Execute(new FormMenuViewModel()));

            procedureClient = new Procedure.ProcedureClient(Program.gRPCChannel);
            name = new() { FormName_ = FormMenuViewModel.FormName };
            formFields = procedureClient.getFormFields(name);
            int index = 0;
            foreach (var field in formFields.Fields)
            {
                if (field.FieldType.Equals("iText.Forms.Fields.PdfTextFormField"))
                {
                    ListOfFields.Add(field);
                }
            }
        }

        public void SubmitFormCommand()
        {
            NavigateToFormMenu.Execute();
        }
        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
