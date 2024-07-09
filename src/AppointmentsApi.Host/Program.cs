using AppointmentsApi.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .ConfigureAppointmentsApiServices();

var app = builder.Build();

app
   .UseSwagger()
   .UseSwaggerUI()
   .UseHttpsRedirection()
   .UseAppointmentsApi();

app.Run();