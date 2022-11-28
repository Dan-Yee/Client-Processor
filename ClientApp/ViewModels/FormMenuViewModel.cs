﻿using Avalonia.Controls.Selection;
using ClientApp.Models;
using ClientApp.Views;
using Grpc.Net.Client;
using GrpcServer.Protos;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

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
        public RoutingState RouterToFillOutForms{ get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToMakeProcedure{ get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToFillOutForms{ get; }


        public FormMenuViewModel()
        {
            CurrentFormSelection.SelectionChanged += SelectionChanged;


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

            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new FormTemplateNames.FormTemplateNamesClient(channel);
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

        public static string FormName { get; set; }
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
