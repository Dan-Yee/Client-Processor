using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class Customer
    {
        public int Employee_ID { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string UserName { get; }
        private readonly string Password;

        public Customer(int employee_ID, string firstName, string lastName, string userName, string password)
        {
            Employee_ID = employee_ID;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Password = password;
        }
    }
}
