using Avalonia.Controls.Selection;
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
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class MakeProcedureViewModel : ReactiveObject, IRoutableViewModel
    {
        //View that this viewmodel is attached to
        private MakeProcedureView _makeProcedureView;
        //Id of the client
        private int _clientId;


        private ObservableCollection<FormModel> _formTemplateList = new();
        public ObservableCollection<FormModel> FormTemplateList
        {
            get => _formTemplateList;
            set
            {
                this.RaiseAndSetIfChanged(ref _formTemplateList, value);
            }
        }

        public ObservableCollection<FormModel> _currentlySelectedForms = new();
        public ObservableCollection<FormModel> CurrentlySelectedForms
        {
            get => _currentlySelectedForms;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentlySelectedForms, value);
            }
        }

        public SelectionModel<FormModel> FormTemplateSelection { get; } = new SelectionModel<FormModel>();
        public SelectionModel<FormModel> CurrentFormSelection { get; } = new SelectionModel<FormModel>();

        //public List<FormInputField> InputFields { get; } = new List<FormInputField>();

        public List<FormInputField> _inputList = new List<FormInputField>();
        public List<FormInputField> InputFields
        {
            get => _inputList;
            set
            {
                this.RaiseAndSetIfChanged(ref _inputList, value);
            }
        }

        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();

        /// <summary>
        /// Constructor of the view model. Initializes the view and the client id
        /// </summary>
        /// <param name="mpv"></param>
        /// <param name="c_ID"></param>
        public MakeProcedureViewModel(MakeProcedureView mpv, int c_ID)
        {
            _makeProcedureView = mpv;
            _clientId = c_ID;
            
            TemplatesResponse templates = GetTemplateNames();


            foreach (var template in templates.TemplateNames)
            {
                FormTemplateList.Add(new FormModel(template.FormTemplateName, null, null));

            }
        }

        public static TemplatesResponse GetTemplateNames()
        {
            TemplatesResponse templates = new TemplatesResponse();

            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var client = new FormTemplateNames.FormTemplateNamesClient(channel);
            templates = client.GetTemplateNames(new TemplatesRequest { });

            return templates;
        }

        public void GoToFormMenu()
        {
            new FormMenuView(_clientId).Show();
            _makeProcedureView.Close();
        }

        public void SelectFormTemplate()
        {
            _currentlySelectedForms.Add(FormTemplateSelection.SelectedItem);
        }

        public void GoToFillOutForm()
        {
            new FormFillingView(_clientId, CurrentFormSelection.SelectedItem.FileName).Show();
            //_makeProcedureView.Close();
        }
    }
}