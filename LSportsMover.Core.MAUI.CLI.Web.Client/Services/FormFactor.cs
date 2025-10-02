using LSportsMover.Core.MAUI.CLI.Shared.Services;

namespace LSportsMover.Core.MAUI.CLI.Web.Client.Services
{
    public class FormFactor : IFormFactor
    {
        public string GetFormFactor()
        {
            return "WebAssembly";
        }

        public string GetPlatform()
        {
            return Environment.OSVersion.ToString();
        }
    }
}
