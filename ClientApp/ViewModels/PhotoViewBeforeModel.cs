using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class PhotoViewBeforeModel : ViewModelBase
    {
        public string GName;
        public string GExtent;

        public PhotoViewBeforeModel(string Name,string Extenstion)
        {
           GName = Name;   
           GExtent = Extenstion;
        }

        public string ImaPath2 => GName + GExtent;



    }
}
