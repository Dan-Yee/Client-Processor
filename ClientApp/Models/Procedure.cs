using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class Procedure
    {
        public int Procedure_ID { get; }
        public string ProcedureType { get; }
        public DateTime procedureDate{ get; }
        public int Cleint_ID { get; }
        public int EmployeeID { get; }

        public Procedure(int procedure_ID, string procedureType, DateTime procedureDate, int cleint_ID, int employeeID)
        {
            Procedure_ID = procedure_ID;
            ProcedureType = procedureType;
            this.procedureDate = procedureDate;
            Cleint_ID = cleint_ID;
            EmployeeID = employeeID;
        }
    }
}
