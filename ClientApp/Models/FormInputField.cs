using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class FormInputField
    {
        public string? FieldName { get; set; }

        public bool IsTextInput { get; set; }

        public bool IsRadioBtn { get; set; }
        public bool IsCheckBox { get; set; }
        public List<OptionModel>? CheckListParameters { get; set; }

        public FormInputField(string? fieldName, bool isTextInput, bool isRadioBtn, bool isCheckBox, List<OptionModel>? checkListParameters)
        {
            FieldName = fieldName;
            IsTextInput = isTextInput;
            IsRadioBtn = isRadioBtn;
            IsCheckBox = isCheckBox;
            CheckListParameters = checkListParameters;
        }
    }
}
