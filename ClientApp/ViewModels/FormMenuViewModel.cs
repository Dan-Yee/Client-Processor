using Avalonia.Controls.Selection;
using ClientApp.Models;
using ClientApp.Views;
using DynamicData;
using Grpc.Net.Client;
using GrpcServer.Protos;
using Org.BouncyCastle.Utilities;
using ReactiveUI;
using Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class FormMenuViewModel : ReactiveObject, IRoutableViewModel
    {
        //View that this viewmodel is attached to
        private FormMenuView _formMenuView;
        //Id of the client
        private int _clientId;


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

        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen => throw new NotImplementedException();

        /// <summary>
        /// Constructor of the view model. Initializes the view and the client id
        /// </summary>
        /// <param name="fmv"></param>
        /// <param name="c_ID"></param>
        public FormMenuViewModel(FormMenuView fmv, int c_ID)
        {
            _formMenuView = fmv;
            _clientId = c_ID;
            CurrentFormSelection.SelectionChanged += SelectionChanged;


            TemplatesResponse templates = GetTemplateNames();

            
            foreach(var template in templates.TemplateNames)
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
            _formMenuView.Close();
            new FormFillingView(_clientId, CurrentFormSelection.SelectedItem.FileName).Show();
        }
    }
}
