using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class OptionModel
    {
        public string OptionName { get; set; }
        public OptionModel(string name)
        {
            OptionName = name;
        }
    }
}
