using Avalonia.Controls.Selection;
using ClientApp.Models;
using GrpcServer.Protos;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;

namespace ClientApp.ViewModels
{
    public class FormMenuViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen { get; }
        public static List<string> ListOfFilledOutFormNames { get; set; } = new();


        private ObservableCollection<FormModel> _currentlySelectedForms = new();
        public ObservableCollection<FormModel> CurrentlySelectedForms
        {
            get => _currentlySelectedForms;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentlySelectedForms, value);
            }
        }

        public SelectionModel<FormModel> CurrentFormSelection { get; } = new SelectionModel<FormModel>();


        private ObservableCollection<FormModel> _formTemplateList = new();
        public ObservableCollection<FormModel> FormTemplateList
        {
            get => _formTemplateList;
            set
            {
                this.RaiseAndSetIfChanged(ref _formTemplateList, value);
            }
        }
        public SelectionModel<FormModel> FormTemplateSelection { get; } = new SelectionModel<FormModel>();
        public RoutingState RouterToMakeProcedure { get; } = new RoutingState();
        public RoutingState RouterToFillOutForms{ get; set; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToMakeProcedure{ get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToFillOutForms{ get; }

        public static string FormName { get; set; }

        public FormMenuViewModel()
        {
            CurrentFormSelection.SelectionChanged += SelectionChanged;
            ListOfFilledOutFormNames.Clear();
            var client = new CompletedForm.CompletedFormClient(Program.gRPCChannel);
            var response = client.CompletedFormNames(new CompletedFormsRequest() { ProcedureID = ClientProcedureListingViewModel.Procedure_Id });
            foreach (var formInformation in response.FormInfo)
            {
                ListOfFilledOutFormNames.Add(formInformation.FormName);
            }
            RouterToFillOutForms = new();

            TemplatesResponse templates = GetTemplateNames();


            foreach (var template in templates.TemplateNames)
            {
                FormTemplateList.Add(new FormModel(template.FormTemplateName, null, null));

            }
            NavigateToMakeProcedure = ReactiveCommand.CreateFromObservable(
                () => RouterToMakeProcedure.Navigate.Execute(new MakeAProcedureViewModel()));

            NavigateToFillOutForms = ReactiveCommand.CreateFromObservable(
                () => RouterToFillOutForms.Navigate.Execute(new FormFillingViewModel()));
        }
        public static TemplatesResponse GetTemplateNames()
        {
            TemplatesResponse templates = new TemplatesResponse();
            var client = new FormTemplateNames.FormTemplateNamesClient(Program.gRPCChannel);
            templates = client.GetTemplateNames(new TemplatesRequest { });

            return templates;
        }


        public void SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            // ... handle selection changed
            

        }

        public void SelectFormTemplate()
        {
            CurrentlySelectedForms.Add(FormTemplateSelection.SelectedItem);
            //MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Selection: "+ FormTemplateSelection.SelectedItem).Show();
        }

        public void GoToFillOutForm()
        {
            FormName = FormTemplateSelection.SelectedItem.FileName;
            NavigateToFillOutForms.Execute();
        }

        public void GoToMakeProcedureCommand()
        {
            NavigateToMakeProcedure.Execute();
        }
    }
}
