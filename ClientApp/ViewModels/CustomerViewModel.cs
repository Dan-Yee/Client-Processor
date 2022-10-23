using ClientApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        private readonly Customer _customer;
        public int Employee_ID => _customer.Employee_ID;
        public string FirstName => _customer.FirstName;
        public string LastName => _customer.LastName;
        public string UserName => _customer.UserName;
        //private readonly string Password;
    }
}
