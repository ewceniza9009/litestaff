using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.Security.Resources;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;
using whris.Application.Common;
using whris.Application.Library;
using whris.Data.Data;
using whris.Data.License;
using whris.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Data Protection with persistent keys
var keyDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys");
Directory.CreateDirectory(keyDir); // Ensures the folder exists

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyDir))
    .SetApplicationName("whris") // Optional: set a consistent name
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // Optional: control key expiration

// Add framework services.
builder.Services
    .AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".whris.session";
});

builder.Services
    .AddControllers();

var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName?.Split(',')[0].Trim() == "whris.Application");
//var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName?.Split(',')[0].Trim().Contains("whris") == true);

builder.Services.AddMediatR(assembly ?? Assembly.GetExecutingAssembly());

// Add Kendo UI services to the services container
builder.Services.AddKendo();

//Add Devexpress report services
builder.Services.AddDevExpressControls();

string dataPath = Lookup.WebRootPathPhoto = $@"{builder.Environment.WebRootPath}\photos\";
AccessSettings.StaticResources.SetRules(DirectoryAccessRule.Allow(dataPath));

builder.Services.AddMvc();
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});

var deviceIdToken = builder.Configuration["DeviceIdToken"] ?? "NA";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<HRISContext>();
builder.Services.AddScoped<GeminiService>();
builder.Services.AddScoped<IConversationService, ConversationService>();

builder.Services.Configure<FormOptions>(options => options.ValueCountLimit = int.MaxValue);
builder.Services.AddMvc(options =>
{
    options.MaxModelBindingCollectionSize = 5000;
});

Config.ConnectionString = connectionString;
Config.IsCustomComputeOvertimeAmountOnRestDay = builder.Configuration.GetValue<bool>("IsCustomComputeOvertimeAmountOnRestDay");
Config.GeminApiKey = builder.Configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey", "Gemini API Key is not configured.");

var app = builder.Build();

// ===============
// Token Validation Logic
// ===============

if (!TokenProcessor.ValidateToken(deviceIdToken))
{
    app.Use((context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/token/invalid") ||
            context.Request.Path == "/Developer")
        {
            return next(context);
        }

        context.Response.Redirect("/token/invalid");
        return Task.CompletedTask;
    });

    app.MapGet("/token/invalid", () =>
    {
        var html = $@"
            <html>
                <head>
                    <meta charset=""utf-8"">
                </head>
                <body style='font-family: sans-serif; padding: 2em; text-align: center;'>
                    <h1>⛔ Invalid Device Token!</h1>
                    <p>This application cannot run without a valid device token.</p>
                    <p>Please contact your administrator and ask for a token for this device.</p> 
                    <p><h2>[{Config.DeviceId}]</h2>.</p>
                </body>
            </html>";

        return Results.Content(html, "text/html");
    });
}
else
{
    // Configure localization middleware
    var supportedCultures = new[] { new CultureInfo("en-US") };

    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("en-US"),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts(); // See https://aka.ms/aspnetcore-hsts 
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // Global exception handler middleware
    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("Critical error: Token validation failed"))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "text/html";

            var errorPage = @"
            <html>
                <body style='font-family: sans-serif; padding: 2em; text-align: center;'>
                    <h1>Configuration Error</h1>
                    <p><strong>Do not remove:</strong> 'TokenProcessor.ValidateToken(Config.DeviceIdToken)'</p>
                    <p>This application cannot start without a proper token validation.</p>
                </body>
            </html>";

            await context.Response.WriteAsync(errorPage);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("<h1>Fatal Error</h1>");
            await context.Response.WriteAsync("<p>An unexpected error occurred.</p>");
            // Log the exception here if needed
        }
    });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSession();

    app.MapRazorPages();

    // For Devexpress reports
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.UseDevExpressControls();
    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

    // Developer info endpoint
    app.MapGet("/Developer", () => "Hi I am Erwin Wilson Ceniza, The developer of this site");

    app.MapGet("/", (HttpContext context) =>
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            return Results.Redirect("/Index");  // Authenticated users go to Index
        }
        else
        {
            return Results.Redirect("/LandingPage");  // Anonymous users go to LandingPage
        }
    });

}

Security.Services = app.Services;

app.Run();