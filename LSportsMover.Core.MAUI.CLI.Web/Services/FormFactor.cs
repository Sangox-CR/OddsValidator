using LSportsMover.Core.MAUI.CLI.Shared.Services;

namespace LSportsMover.Core.MAUI.CLI.Web.Services
{
    public class FormFactor : IFormFactor
    {
        public string GetFormFactor()
        {
            return "Web";
        }

        public string GetPlatform()
        {
            return Environment.OSVersion.ToString();
        }
    }
}
