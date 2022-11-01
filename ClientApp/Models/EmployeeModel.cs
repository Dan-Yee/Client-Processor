using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class EmployeeModel
    {
        public int ID { get; }
        public string FirstName{ get; }
        public string LastName{ get; }
        public string UserName{ get; }
        public bool IsAdmin { get; }

        public EmployeeModel(int id,string firstName, string lastName, string userName, bool isAdmin)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            IsAdmin = isAdmin;
        }
        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }

}
