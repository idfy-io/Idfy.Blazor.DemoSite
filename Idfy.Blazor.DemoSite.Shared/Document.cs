using Idfy.Signature;
using System;
using System.Collections.Generic;

namespace Idfy.Blazor.DemoSite.Shared
{
    public class DemoSigner: Signer
    {
        public new bool GetSocialSecurityNumber { get; set; }
        public new bool Required { get; set; }
        public new DemoSignerInfo SignerInfo { get; set; }
    }

    public class DemoSignerInfo : SignerInfo
    {
        public string Title { get; set; }
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

    public class DemoFile : Attachment
    {
        public new bool ConvertToPdf { get; set; }
        public bool Done { get; set; }
        public string Base64ContentStyleSheet { get; set; }
        public string StringType
        {
            get
            {
                return Type.ToString();
            }
            set
            {                    
                AttachmentType type;
                if (Enum.TryParse(value, out type))
                {
                    Type = type;
                }               
            }
        }

        public List<string> SignersExtId { get; set; }
    }
    
}
