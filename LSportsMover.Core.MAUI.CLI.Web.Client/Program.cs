using LSportsMover.Core.MAUI.CLI.Shared.Services;
using LSportsMover.Core.MAUI.CLI.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the LSportsMover.Core.MAUI.CLI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
