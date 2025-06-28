
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Operational_Risk_Management.Models;
using Operational_Risk_Management.Models.Common;
using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Enums;
using Operational_Risk_Management.Models.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventLog(eventLogSettings =>
{
    eventLogSettings.SourceName = "OperationalRiskManagement";
});
// Add services to the container.
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();
builder.Services.AddControllersWithViews(options =>
{
    // Insert your custom model binder provider
    options.ModelBinderProviders.Insert(0, new CommaDelimitedModelBinderProvider());
})
.AddJsonOptions(options =>
{
    // Configure JSON serialization
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddRazorPages();
builder.Services.AddSignalR();
//builder.Services.AddAuthorization(options =>
//{
//    Enum.GetValues(typeof(UserRoles)).Cast<UserRoles>().ToList().ForEach(x =>
//           options.AddPolicy(x.ToString(), a => a.Requirements.Add(new DefaultWindowsAuthorization(x.ToString())))
//       );
//    options.DefaultPolicy = options.GetPolicy(UserRoles.Default.ToString());
//    // By default, all incoming requests will be authorized according to the default policy.
//    options.FallbackPolicy = options.DefaultPolicy;
//});
var environment = builder.Environment.EnvironmentName;
//environment = "Production";
builder.Configuration
       .AddJsonFile($"appsettings.{environment}.json", true, true)
       .AddEnvironmentVariables();
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
builder.Services.AddDbContext<ApplicationDBContext>(opt =>
{
    opt.UseSqlServer(connectionString);
    opt.EnableDetailedErrors();
});
builder.AddServices();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();
app.MapHub<OverrideHub>("/overrideHub");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
