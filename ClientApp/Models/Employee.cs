using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class Employee
    {
        public int ID { get; }
        public string FirstName{ get; }
        public string LastName{ get; }
        public string UserName{ get; }
        private readonly string Password;

        public Employee(int id,string firstName,string lastName,string userName,string password)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Password = password;
        }
    }
}
