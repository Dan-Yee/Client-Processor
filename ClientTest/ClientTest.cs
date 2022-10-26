using Grpc.Net.Client;
using Server;

var channel = GrpcChannel.ForAddress("https://localhost:7123");                                 // localhost for testing purposes
/*var client = new Client.ClientClient(channel);

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
        case 1:
            var createResponse = client.newClient(getNewInfo());
            if(createResponse.IsSuccessfulOperation)
            {
                Console.WriteLine("Successfully created a new Client.\n");
            } else
            {
                Console.WriteLine("Error creating a new Client.\n");
            }
            break;
        case 2:
            listAllClients(client.getClients(new Google.Protobuf.WellKnownTypes.Empty()));
            break;
        case 3:
            continueProgram = true;
            break;
        default:
            Console.WriteLine("Error: Action not recognized.\n");
            break;
    }
} while (!continueProgram);

static void displayActions()
{
    Console.WriteLine("Select An Action:");
    Console.WriteLine("\t(1) Create New Client");
    Console.WriteLine("\t(2) List All Clients");
    Console.WriteLine("\t(3) Quit");
}

static ClientInfo getNewInfo()
{
    Console.WriteLine("Enter New Client Information:");
    Console.Write("First Name: ");
    string? newFirstName = Console.ReadLine();
    Console.Write("Last Name: ");
    string? newLastName = Console.ReadLine();
    Console.Write("Phone Number: ");
    string? phoneNumber = Console.ReadLine();
    Console.Write("Email: ");
    string? email = Console.ReadLine();

    var clientInfo = new ClientInfo
    {
        FirstName = newFirstName,
        LastName = newLastName,
        PhoneNumber = phoneNumber,
        Email = email
    };
    return clientInfo;
}

static void listAllClients(AllClients info)
{
    Console.WriteLine("----------< Begin All Clients >----------");
    Console.WriteLine();
    foreach (ClientInfo client in info.Clients)
    {
        Console.WriteLine("Client ID: " + client.ClientId);
        Console.WriteLine("First Name: " + client.FirstName);
        Console.WriteLine("Last Name: " + client.LastName);
        Console.WriteLine("Phone Number: " + client.PhoneNumber);
        Console.WriteLine("Email: " + client.Email);
        Console.WriteLine();
    }
    Console.WriteLine("----------< End All Clients >----------");
    Console.WriteLine();
}*/

// <BELOW> TESTS FOR EMPLOYEE SERVICES <BELOW>
var client = new Employee.EmployeeClient(channel);
bool continueProgram = false;
do
{
    login(client);
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
                Console.WriteLine("Successfully created a new Employee.");
            } else
            {
                Console.WriteLine("Error creating a new Employee.");
            }
            Console.WriteLine(createResponse.StatusMessage + "\n");
            break;
        case 2:             // updating an existing employee
            var updateResponse = client.updateEmployee(getUpdatedInfo());
            if(updateResponse.IsSuccessfulOperation)
            {
                Console.WriteLine("Successfully updated Employee information.");
            } else
            {
                Console.WriteLine("Error updating Employee information.");
            }
            Console.WriteLine(updateResponse.StatusMessage + "\n");
            break;
        case 3:             // listing all employees
            listAllEmployees(client.getEmployees(new Google.Protobuf.WellKnownTypes.Empty()));
            break;
        case 4:             // quit program
            continueProgram = true;
            break;
        default:
            Console.WriteLine("ERROR: Action Not Recognized\n");
            break;
    }
} while (!continueProgram);

Console.WriteLine("Press any key to exit...");
Console.ReadLine();

static void login(Employee.EmployeeClient client)
{
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

        var serviceResponse = client.doLogin(credentials);                               // asynchronous rpc to Server to verify login credentials
        if (serviceResponse.IsSuccessfulLogin)                                            // successful login
        {
            isSuccessfulLogin = true;
            Console.WriteLine("Login Successful!");
            Console.WriteLine("You have logged in with Employee ID: " + serviceResponse.EmployeeId);
            Console.WriteLine("Account Privilege (isAdmin): " + serviceResponse.IsAdmin + "\n");
        }
        else                                                                             // incorrect login credentials
        {
            Console.WriteLine("Incorrect username or password. Please try again.\n");
        }
    } while (!isSuccessfulLogin);
}

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
    Console.Write("Is Admin? (y/n): ");
    string? admin = Console.ReadLine();
    bool isAdmin = false;
    if (admin.Equals("y"))
    {
        isAdmin = true;
    }

    var newCredentials = new LoginCredentials
    {
        Username = newUsername,
        Password = newPassword
    };
    var employeeInfo = new EmployeeInfo
    {
        FirstName = newFirstName,
        LastName = newLastName,
        Credentials = newCredentials,
        IsAdmin = isAdmin
    };
    return employeeInfo;
}

static EmployeeInfo getUpdatedInfo()
{
    Console.WriteLine("Enter Updated Employee Information:");
    Console.Write("Employee ID: ");
    string? employeeID = Console.ReadLine();
    int id = int.Parse(employeeID);
    Console.Write("First Name: ");
    string? newFirstName = Console.ReadLine();
    Console.Write("Last Name: ");
    string? newLastName = Console.ReadLine();
    Console.Write("Username: ");
    string? newUsername = Console.ReadLine();
    Console.Write("Password: ");
    string? newPassword = Console.ReadLine();
    Console.Write("Is Admin? (y/n): ");
    string? admin = Console.ReadLine();
    bool isAdmin = false;
    if (admin.Equals("y"))
    {
        isAdmin = true;
    }

    var newCredentials = new LoginCredentials
    {
        Username = newUsername,
        Password = newPassword
    };
    var employeeInfo = new EmployeeInfo
    {
        EmployeeId = id,
        FirstName = newFirstName,
        LastName = newLastName,
        Credentials = newCredentials,
        IsAdmin = isAdmin
    };
    return employeeInfo;
}

static void listAllEmployees(AllEmployees info)
{
    Console.WriteLine("----------< Begin All Employees >----------");
    Console.WriteLine();
    foreach(EmployeeInfo employee in info.Employees)
    {
        Console.WriteLine("Employee ID: " + employee.EmployeeId);
        Console.WriteLine("First Name: " + employee.FirstName);
        Console.WriteLine("Last Name: " + employee.LastName);
        Console.WriteLine("Username: " + employee.Credentials.Username);
        Console.WriteLine("Is Admin?: " + employee.IsAdmin);
        Console.WriteLine();
    }
    Console.WriteLine("----------< End All Employees >----------");
    Console.WriteLine();
}