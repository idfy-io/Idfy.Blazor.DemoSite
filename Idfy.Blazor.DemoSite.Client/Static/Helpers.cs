using Idfy.Blazor.DemoSite.Shared;

namespace Idfy.Blazor.DemoSite.Client.Static
{
    public static class Helpers
    {
        public static bool IsEditable(DemoDocument demoDocument)
        {
            return demoDocument?.Status?.DocumentStatus != null && demoDocument.Status.DocumentStatus != Signature.DocumentStatus.Signed;
        }

        public static string SignerName(DemoSigner signer)
        {
            var name = signer.SignerInfo.FirstName + " " + signer.SignerInfo.LastName;

            if (string.IsNullOrWhiteSpace(name))
                name = "Nameless signer";
            return name;
        }
    }
}
