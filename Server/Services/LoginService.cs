using Grpc.Core;

namespace Server.Services
{
    public class LoginService : Login.LoginBase
    {
        private readonly ILogger<LoginService> _logger;

        // credentials for testing purposes.
        private readonly string username = "admin";
        private readonly string password = "password12222";
        public LoginService(ILogger<LoginService> logger)
        {
            _logger = logger;
        }

        /*
         * Method to verify the credentials entered from the client side of the RPC
         */ 
        public override Task<LoginStatus> doLogin(Credentials request, ServerCallContext context)
        {
            LoginStatus status = new();
            
            // verify the login credentials with the test credentials on the server
            if(request.Username == username && request.Password == password)
            {
                status.IsSuccessfulLogin = true;
            } 
            else
            {
                status.IsSuccessfulLogin = false;
            }
            return Task.FromResult(status);
        }
    }
}
