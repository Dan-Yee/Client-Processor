using GrpcServer.Services;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapGrpcService<EmployeeService>();
app.MapGrpcService<ClientService>();
app.MapGrpcService<ProcedureService>();
app.MapGrpcService<FileUploadService>();//to remove and replace with below
app.MapGrpcService<UploadFileService>();
app.MapGrpcService<DownloadPhotosService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();