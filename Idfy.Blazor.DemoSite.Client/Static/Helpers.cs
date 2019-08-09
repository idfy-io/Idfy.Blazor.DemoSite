using Idfy.Blazor.DemoSite.Shared;

namespace Idfy.Blazor.DemoSite.Client.Static
{
    public static class Helpers
    {
        public static bool IsEditable(DemoDocument demoDocument)
        {
            return demoDocument?.Status?.DocumentStatus != null && demoDocument.Status.DocumentStatus != Signature.DocumentStatus.Signed;
        } 
    }
}
