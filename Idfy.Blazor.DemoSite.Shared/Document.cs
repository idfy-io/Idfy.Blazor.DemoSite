using Idfy.Signature;
using System.Collections.Generic;

namespace Idfy.Blazor.DemoSite.Shared
{
    public class DemoSigner: Signer
    {
        public bool GetSocialSecurityNumber { get; set; }
        public new bool Required { get; set; }
    }

    public class DemoDocument: Document
    {
        public new List<DemoSigner> Signers { get; set; }
        public new DemoAdvanced Advanced { get; set; }
    }

    public class DemoAdvanced: Advanced
    {
        public new bool GetSocialSecurityNumber { get; set; }
    }

    public class DemoFile : DataToSign
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public new bool ConvertToPDF { get; set; }
        public AttachmentType Type { get; set; }
    }
    
}
