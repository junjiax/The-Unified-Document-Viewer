using Chap10.Repositories;
using Chap10.Repositories.Sale;
using Chap10.Repositories.Service;
using Chap10.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Chap10.Controllers;
using Chap10.Services;
using Chap10.Models.SaleModels;
using Chap10.Models.ServiceModels;
using Serilog;
using Serilog.Events;
using Chap10.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for comprehensive logging
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Debug()
        .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/unified-document-viewer-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            fileSizeLimitBytes: 10485760, // 10MB
            retainedFileCountLimit: 7)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "UnifiedDocumentViewer");
});

var saleConnectionString = builder.Configuration.GetConnectionString("SaleDBConnection")
    ?? throw new InvalidOperationException("Connection string 'SaleDBConnection' not found.");

var serviceConnectionString = builder.Configuration.GetConnectionString("ServiceDBConnection")
    ?? throw new InvalidOperationException("Connection string 'ServiceDBConnection' not found.");

builder.Services.AddDbContext<SaleDbContext>(options =>
    options.UseMySql(saleConnectionString, ServerVersion.AutoDetect(saleConnectionString))
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Debug));

builder.Services.AddDbContext<ServiceDbContext>(options =>
    options.UseMySql(serviceConnectionString, ServerVersion.AutoDetect(serviceConnectionString))
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Debug));

builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<FinancingContract>, FinancingContractRepository>();
builder.Services.AddScoped<IRepository<SalesDocument>, SalesDocumentRepository>();
builder.Services.AddScoped<IRepository<SalesTransaction>, SalesTransactionRepository>();
builder.Services.AddScoped<IRepository<WarrantyRegistration>, WarrantyRegistrationRepository>();

builder.Services.AddScoped<IRepository<DiagnosticReport>, DiagnosticReportRepository>();
builder.Services.AddScoped<IRepository<ServiceDocument>, ServiceDocumentRepository>();
builder.Services.AddScoped<IRepository<ServiceRecord>, ServiceRecordRepository>();
builder.Services.AddScoped<IRepository<Technician>, TechnicianRepository>();

builder.Services.AddScoped<ISaleApiClient, SaleAPI>();
builder.Services.AddScoped<IServiceApiClient, ServiceAPI>();

builder.Services.AddScoped<IUnifiedDocumentService, UnifiedDocumentService>();
builder.Services.AddScoped<UnifiedDocumentController>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.

builder.Services.AddControllers();

WebApplication app = builder.Build();

// Add custom exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Add request/response logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); 
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting Unified Document Viewer application");
    app.Run();
}
catch (System.IO.IOException ex) when (ex.InnerException is Microsoft.AspNetCore.Connections.AddressInUseException)
{
    Log.Warning(ex, "Configured port is in use, attempting to use ephemeral port");
    // configured port is in use — pick a free ephemeral port and restart
    var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
    listener.Start();
    var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
    listener.Stop();

    Log.Information("Restarting application on port {Port}", port);
    builder.WebHost.UseUrls($"http://127.0.0.1:{port}");
    app = builder.Build();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
