using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OddsValidator.MAUI.Core.Shared.Services;
using OddsValidator.MAUI.Core.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the OddsValidator.MAUI.Core.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
