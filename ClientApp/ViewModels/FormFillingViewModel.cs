using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientApp.Models;
using ClientApp.Views;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Server;

namespace ClientApp.ViewModels
{
    public class FormFillingViewModel : ViewModelBase
    {
        public List<FormInputField> InputFields { get; } = new List<FormInputField>();
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
        public FormFillingViewModel(FormFillingView ffv,int c_ID,string filename)
        {
            _clientId = c_ID;
            _formFillingView = ffv;
            _fileName = filename;

            var channel = GrpcChannel.ForAddress("https://localhost:7123");
            var procedureClient = new Procedure.ProcedureClient(channel);

            FormName name = new() { FormName_ = _fileName };
            var formFields = procedureClient.getFormFields(name);
            //List<FormInputField> InputFields = new List<FormInputField>();
            foreach (var field in formFields.Fields)
            {
                if (field.FieldType.Equals("iText.Forms.Fields.PdfTextFormField"))
                {
                    InputFields.Add(new FormInputField(field.FieldName, true, false, false, null));
                    
                }
            }

        }
    }
}
