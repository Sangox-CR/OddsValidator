using BettorsDenMover.Core.Lib.Services.API.Authentication;
using BettorsDenMover.Core.Lib.Services.API.Email;
using BettorsDenMover.Core.Lib.Setup;
using LSportsMover.Core.Lib.Config;
using LSportsMover.Core.Lib.Const;
using LSportsMover.Core.Lib.Services;
using LSportsMover.Core.Lib.Services.Monitors;
using LSportsMover.Core.Lib.Services.PackageManager;
using LSportsMover.Core.Lib.Services.RabbitManager;
using LSportsMover.Core.Lib.Services.ServiceManager;
using LSportsMover.Core.MAUI.CLI.Shared.Services;
using LSportsMover.Core.MAUI.CLI.Web.Components;
using LSportsMover.Core.MAUI.CLI.Web.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using ProtoBuf.Meta;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the LSportsMover.Core.MAUI.CLI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Add Json options
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Bind AppSettings section to AppSettings class
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

// Add services for the LSportsMover.Core.MAUI.CLI.Web project
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
builder.Services.AddSingleton<ISettings>(provider =>
{
    var _settings = provider.GetRequiredService<AppSettings>();
    var _commonUtils = provider.GetRequiredService<LSportsMover.Core.Lib.Utils.CommonUtils>();
    IntegrationType integrationType = _commonUtils.GetIntegrationType();
    var _connstr = "Data Source=" + _settings.HostnameDb +
    ";Initial Catalog=" + _settings.Database + ";Persist Security Info=True;TrustServerCertificate=True;User ID=" + _settings.DbUser +
    ";Password=" + _settings.DbPass + ";Application Name=LSports Automover " + integrationType.ToString();
    _settings.ConnectionString = _connstr;
    return _settings;
});

//custom logs
builder.Services.Configure<ConsoleFormatterOptions>(options => { });
builder.Services.AddSingleton<ConsoleFormatter, LSportsMover.Core.Lib.Utils.MinimalConsoleFormatter>();


//startup services to determine if the RabbitMQ consumer/outright background service should start
builder.Services.AddSingleton<IStartupRabbitSync, StartupRabbitSync>();
builder.Services.AddSingleton<IStartupOutrightSync, StartupOutrightSync>();


builder.Services.AddSingleton<BettorsDenMover.Core.Lib.Util.CommonUtils>();
builder.Services.AddSingleton<BettorsDenMover.Core.Lib.Data.DataProcess>();
builder.Services.AddSingleton<BettorsDenMover.Core.Lib.Data.DataBaseHandler>();
builder.Services.AddSingleton<BettorsDenMover.Core.Lib.Util.TransformationUtils>();
builder.Services.AddSingleton<LSportsMover.Core.Lib.Utils.CommonUtils>();
builder.Services.AddSingleton<LSportsMover.Core.Lib.Utils.DataProcess>();

//token services for every API, must create a TokenHandler for each API that requires authentication
//builder.Services.AddSingleton<ITokenService, TokenService>();

// Handlers
builder.Services.AddTransient<EmailTokenHandler>();

// Email API Client
builder.Services.AddHttpClient<EmailAPIClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<ISettings>();
    client.BaseAddress = new Uri(settings.Email.ApiURL);
}).AddHttpMessageHandler<EmailTokenHandler>();


//handles package management
builder.Services.AddHttpClient<IPackageManager, PackageManager>();
builder.Services.AddScoped<IPackageManager, PackageManager>();

//handles http requests to the LSports API
builder.Services.AddHttpClient<IHTTPServiceManager, HTTPServiceManager>();

//register the implementations but as scope not as hosted services
builder.Services.AddScoped<PackageService>();
//monitors
builder.Services.AddScoped<MonitorPrematchService>();
builder.Services.AddScoped<MonitorInplayService>();
builder.Services.AddScoped<MonitorSTMPrematchService>();
builder.Services.AddScoped<MonitorSTMInplayService>();
builder.Services.AddScoped<MonitorOutrightPrematchService>();


//services
builder.Services.AddScoped<SnapshotService>();
builder.Services.AddScoped<ISnapshotService, SnapshotService>();
builder.Services.AddScoped<OutrightService>();
builder.Services.AddScoped<ISnapshotService, OutrightService>();

//Outright services
builder.Services.AddTransient<OutrighFixtureWorker>();

//RabbitMQ Services used as Singleton

builder.Services.AddSingleton<RabbitMQConnection>(provider =>
{
    var startupSync = provider.GetRequiredService<IStartupRabbitSync>();
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    var commonUtils = provider.GetRequiredService<LSportsMover.Core.Lib.Utils.CommonUtils>();
    var credentials = commonUtils.GetIntegrationCredentials();
    var maxRetries = commonUtils.GetMaxErrorReqAllowed();
    return new RabbitMQConnection(credentials, maxRetries, loggerFactory, startupSync);
});
builder.Services.AddSingleton<RabbitMQConsumer>(provider =>
{
    var rabbitConnection = provider.GetRequiredService<RabbitMQConnection>();
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    var startupSync = provider.GetRequiredService<IStartupRabbitSync>();
    var appSettings = provider.GetRequiredService<AppSettings>();
    var dataBaseHandler = provider.GetRequiredService<BettorsDenMover.Core.Lib.Data.DataBaseHandler>();
    var dataProcess = provider.GetRequiredService<LSportsMover.Core.Lib.Utils.DataProcess>();
    var commonUtils = provider.GetRequiredService<BettorsDenMover.Core.Lib.Util.CommonUtils>();
    var commonUtilsLS = provider.GetRequiredService<LSportsMover.Core.Lib.Utils.CommonUtils>();
    return new RabbitMQConsumer(rabbitConnection, loggerFactory, startupSync, appSettings, dataBaseHandler, dataProcess, commonUtils, commonUtilsLS);
});

//builder.Services.AddHostedService<CleanupService>(); //cleanup service to handle application shutdown and cleanup tasks
//builder.Services.AddHostedService<StartupService>();
//builder.Services.AddHostedService<RabbitMQConsumer>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(LSportsMover.Core.MAUI.CLI.Shared._Imports).Assembly,
        typeof(LSportsMover.Core.MAUI.CLI.Web.Client._Imports).Assembly);

app.Run();
