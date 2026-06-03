using Chap10.Models;
using Chap10.Repositories;
using Chap10.Repositories.Sale;
using Chap10.Repositories.Service;
using Chap10.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Chap10.Controllers;
using Chap10.Services;
using Chap10.Models.SaleModels;
using Chap10.Models.ServiceModels;

var builder = WebApplication.CreateBuilder(args);

var saleConnectionString = builder.Configuration.GetConnectionString("SaleDBConnection")
    ?? throw new InvalidOperationException("Connection string 'SaleDBConnection' not found.");

var serviceConnectionString = builder.Configuration.GetConnectionString("ServiceDBConnection")
    ?? throw new InvalidOperationException("Connection string 'ServiceDBConnection' not found.");

builder.Services.AddDbContext<SaleDbContext>(options =>
    options.UseMySql(saleConnectionString, ServerVersion.AutoDetect(saleConnectionString)));

builder.Services.AddDbContext<ServiceDbContext>(options =>
    options.UseMySql(serviceConnectionString, ServerVersion.AutoDetect(serviceConnectionString)));

builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<FinancingContract>, FinancingContractRepository>();
builder.Services.AddScoped<IRepository<SalesDocument>, SalesDocumentRepository>();
builder.Services.AddScoped<IRepository<SalesTransaction>, SalesTransactionRepository>();
builder.Services.AddScoped<IRepository<WarrantyRegistration>, WarrantyRegistrationRepository>();

builder.Services.AddScoped<IRepository<DiagnosticReport>, DiagnosticReportRepository>();
builder.Services.AddScoped<IRepository<ServiceDocument>, ServiceDocumentRepository>();
builder.Services.AddScoped<IRepository<ServiceRecord>, ServiceRecordRepository>();
builder.Services.AddScoped<IRepository<Technician>, TechnicianRepository>();

builder.Services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
builder.Services.AddScoped<IUnifiedDocumentService, UnifiedDocumentService>();
builder.Services.AddScoped<UnifiedDocumentController>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.

builder.Services.AddControllers();

WebApplication app = builder.Build();

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
    app.Run();
}
catch (System.IO.IOException ex) when (ex.InnerException is Microsoft.AspNetCore.Connections.AddressInUseException)
{
    // configured port is in use — pick a free ephemeral port and restart
    var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
    listener.Start();
    var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
    listener.Stop();

    builder.WebHost.UseUrls($"http://127.0.0.1:{port}");
    app = builder.Build();
    app.Run();
}
