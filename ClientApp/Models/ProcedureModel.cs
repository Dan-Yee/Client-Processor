namespace ClientApp.Models
{
    public class ProcedureModel
    {
        public int Procedure_ID { get; }
        public string ProcedureName { get; }
        public string procedureDate{ get; }
        public int Cleint_ID { get; }
        public int EmployeeID { get; }
        public string procedureNotes { get; }

        ProcedureModel()
        {

        }

        public ProcedureModel(int procedure_ID, string procedureName, string procedureDate, int cleint_ID, int employeeID, string procedureNotes)
        {
            this.Procedure_ID = procedure_ID;
            this.ProcedureName = procedureName;
            this.procedureDate = procedureDate;
            this.procedureNotes = procedureNotes;
            this.Cleint_ID = cleint_ID;
            this.EmployeeID = employeeID;
        }
    }
}
