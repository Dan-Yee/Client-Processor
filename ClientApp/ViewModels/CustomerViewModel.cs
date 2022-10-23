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
        public int Customer_ID => _customer.Client_ID;
        public string FirstName => _customer.FirstName;
        public string LastName => _customer.LastName;
        public string PhoneNumber => _customer.PhoneNumber;
        public string Email => _customer.Email;
        //public string UserName => _customer.UserName;
        //private readonly string Password;
    }
}
