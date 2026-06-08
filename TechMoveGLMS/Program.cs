using TechMoveGLMS.Services;
using TechMoveGLMS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services.
builder.Services.AddControllersWithViews();

// Register HttpClient for the backend API.
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

var apiKey = builder.Configuration["ApiSettings:ApiKey"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("ApiSettings:BaseUrl is missing from appsettings.json.");
}

if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException("ApiSettings:ApiKey is missing from appsettings.json.");
}

builder.Services.AddHttpClient("TechMoveApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
});

// Used by MVC frontend to validate uploaded contract PDF files.
builder.Services.AddScoped<IFileValidationService, FileValidationService>();

var app = builder.Build();

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