using Grpc.Net.Client;
using Server;

var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
var client = new Employee.EmployeeClient(channel);

bool isSuccessfulLogin = false;
do
{
    Console.Write("Username: ");
    string? username = Console.ReadLine();
    Console.Write("Password: ");
    string? password = Console.ReadLine();

    var credentials = new LoginCredentials
    {
        Username = username,
        Password = password,
    };

    var serviceResponse = await client.doLoginAsync(credentials);                               // assynchronous rpc to Server to verify login credentials
    if(serviceResponse.IsSuccessfulLogin)                                                       // successful login
    {
        isSuccessfulLogin = true;
        Console.WriteLine("Login Successful!\n");
    }
    else                                                                                        // incorrect login credentials
    {
        Console.WriteLine("Incorrect username or password. Please try again.\n");
    }
} while (!isSuccessfulLogin);

bool continueProgram = false;
do
{
    displayActions();
    Console.Write("Your Choice: ");
    string? input = Console.ReadLine();
    int inputValue = int.Parse(input);
    Console.WriteLine();
    switch(inputValue)
    {
        case 1:             // creating a new employee
            var createResponse = client.newEmployee(getNewInfo());
            if(createResponse.IsSuccessfulOperation)
            {
                Console.WriteLine("Successfully created a new Employee.\n");
            } else
            {
                Console.WriteLine("Error creating a new Employee.\n");
            }
            break;
        case 2:             // updating an existing employee
            break;
        case 3:             // listing all employees
            listAllEmployees(client.getEmployees(new Google.Protobuf.WellKnownTypes.Empty()));
            break;
        case 4:             // quit program
            continueProgram = true;
            break;
        default:
            Console.WriteLine("ERROR: Action Not Recognized");
            break;
    }
} while (!continueProgram);

Console.WriteLine("Press any key to exit...");
Console.ReadLine();

static void displayActions()
{
    Console.WriteLine("Select an action:");
    Console.WriteLine("\t(1) Create New Employee");
    Console.WriteLine("\t(2) Update Existing Employee");
    Console.WriteLine("\t(3) List All Employees");
    Console.WriteLine("\t(4) Quit");
}

static EmployeeInfo getNewInfo()
{
    Console.WriteLine("Enter New Employee Information:");
    Console.Write("First Name: ");
    string? newFirstName = Console.ReadLine();
    Console.Write("Last Name: ");
    string? newLastName = Console.ReadLine();
    Console.Write("Username: ");
    string? newUsername = Console.ReadLine();
    Console.Write("Password: ");
    string? newPassword = Console.ReadLine();

    var newCredentials = new LoginCredentials
    {
        Username = newUsername,
        Password = newPassword
    };
    var employeeInfo = new EmployeeInfo
    {
        FirstName = newFirstName,
        LastName = newLastName,
        Credentials = newCredentials
    };
    return employeeInfo;
}

static void listAllEmployees(AllEmployeesInfo info)
{
    Console.WriteLine("----------< Begin All Employees >----------");
    Console.WriteLine();
    foreach(EmployeeInfo employee in info.Employees)
    {
        Console.WriteLine("Employee ID: " + employee.EmployeeId);
        Console.WriteLine("First Name: " + employee.FirstName);
        Console.WriteLine("Last Name: " + employee.LastName);
        Console.WriteLine("Username: " + employee.Credentials.Username);
        Console.WriteLine("Password: " + employee.Credentials.Password);
        Console.WriteLine();
    }
    Console.WriteLine("----------< End All Employees >----------");
    Console.WriteLine();
}