using AppointmentsApi.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .ConfigureAppointmentsApiServices(builder.Configuration);

var app = builder.Build();

app
   .UseSwagger()
   .UseSwaggerUI()
   .UseHttpsRedirection()
   .UseAppointmentsApi();

app.Run();