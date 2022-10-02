using Grpc.Net.Client;
using Server;

var channel = GrpcChannel.ForAddress("https://localhost:7143");                                 // localhost for testing purposes
var client = new Login.LoginClient(channel);

bool isSuccessfulLogin = false;
do
{
    Console.Write("Username: ");
    string? username = Console.ReadLine();
    Console.Write("Password: ");
    string? password = Console.ReadLine();

    var credentials = new Credentials
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
Console.WriteLine("Press any key to exit...");
Console.ReadLine();