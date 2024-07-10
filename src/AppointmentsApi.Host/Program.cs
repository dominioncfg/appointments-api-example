using AppointmentsApi.Api;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Appointments API",
                Description = "An ASP.NET Core Web API for managing yout appointments",
            });
            var xmlFilename = $"{typeof(AppointmentsApiExtension).Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        }
     )
    .ConfigureAppointmentsApiServices(builder.Configuration);

var app = builder.Build();

app
   .UseSwagger()
   .UseSwaggerUI()
   .UseHttpsRedirection()
   .UseAppointmentsApi();

app.Run();