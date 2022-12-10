namespace ClientApp.Models
{
    public class Customer
    {
        public int Client_ID { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PhoneNumber { get; }
        public string Email { get; }


        public Customer()
        {
            
        }

        public Customer(int client_ID, string firstName, string lastName, string phonenumber, string email)
        {
            Client_ID = client_ID;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phonenumber;
            Email = email;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
