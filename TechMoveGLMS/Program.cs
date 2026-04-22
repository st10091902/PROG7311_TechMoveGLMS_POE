using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories;
using TechMoveGLMS.Repositories.Interfaces;
using TechMoveGLMS.Services;
using TechMoveGLMS.Services;
using TechMoveGLMS.Services.Interfaces;
using TechMoveGLMS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddScoped<IContractRepository, ContractRepository>();

builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();

builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();

builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

builder.Services.AddScoped<IFileValidationService, FileValidationService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.Clients.Any())
    {
        var client1 = new Client { Name = "ABC Logistics", ContactDetails = "abc@mail.com", Region = "Gauteng" };
        var client2 = new Client { Name = "Global Freight", ContactDetails = "global@mail.com", Region = "Western Cape" };

        context.Clients.AddRange(client1, client2);
        context.SaveChanges();

        context.Contracts.AddRange(
            new Contract
            {
                ClientId = client1.ClientId,
                StartDate = DateTime.Today.AddMonths(-2),
                EndDate = DateTime.Today.AddMonths(2),
                Status = ContractStatus.Active,
                ServiceLevel = "Premium"
            },
            new Contract
            {
                ClientId = client2.ClientId,
                StartDate = DateTime.Today.AddMonths(-6),
                EndDate = DateTime.Today.AddMonths(-1),
                Status = ContractStatus.Expired,
                ServiceLevel = "Standard"
            }
        );

        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();