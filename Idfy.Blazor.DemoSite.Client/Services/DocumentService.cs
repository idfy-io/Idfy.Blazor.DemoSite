using Idfy.Signature;
using Idfy.Blazor.DemoSite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idfy.Blazor.DemoSite.Client.Services
{
    public class DocumentService
    {
        public DemoDocument Document { get; set; }
        public List<DemoSite.Shared.DemoFile> Files { get; set; }

        public DocumentService()
        {
            Files = new List<DemoSite.Shared.DemoFile>
            {
                DefaultTxtFile
            };

            Document = new DemoDocument()
            {
                Title = "Test Document",
                Advanced = new DemoAdvanced()
                {
                    Tags = new List<string>
                    {
                        "Demo",
                        "Develop",
                        "Pizza"
                    },
                    TimeToLive = new TimeToLive()
                    {
                        Deadline = DateTime.UtcNow.AddHours(12),
                        DeleteAfterHours = 1
                    }
                },
                ContactDetails = new ContactDetails()
                {
                    Email = "support@idfy.io",
                    Name = "Idfy Norge",
                    Url = "https://idfy.io",                    
                },
                DataToSign = new DataToSign(),
                Description = "Demo site test document",
                ExternalId = System.Guid.NewGuid().ToString(),
                Notification = new Signature.Notification(),
                Signers = new System.Collections.Generic.List<DemoSigner>()
                {
                    new DemoSigner()
                    {
                        SignerInfo = new SignerInfo()
                        {
                            FirstName = "Serious",
                            LastName = "Signer",
                            Mobile = new Mobile()
                            {
                                CountryCode = "+47",
                            }
                        },
                        RedirectSettings = new RedirectSettings()
                        {
                            RedirectMode = RedirectMode.DonotRedirect
                        }
                    }
                }
            };
        }

        public void AddSigner()
        {
            Document.Signers.Add(new DemoSigner()
            {
                SignerInfo = new SignerInfo()
                {
                    Mobile = new Mobile()
                    {
                        CountryCode = "+47",
                    }
                },
                RedirectSettings = new RedirectSettings()
                {
                    RedirectMode = RedirectMode.DonotRedirect
                }
            });
        }

        public DemoSite.Shared.DemoFile DefaultTxtFile => new DemoSite.Shared.DemoFile()
        {
            Base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("This text can safely be signed. This is just a text file with convert to pdf sat to true")),
            ConvertToPDF = true,
            Description = "The bottom of the pizza, called the \"crust\", may vary widely according to style, thin as in a typical hand-tossed" +
                " Neapolitan pizza or thick as in a deep-dish Chicago-style. It is traditionally plain, " +
                "but may also be seasoned with garlic or herbs, or stuffed with cheese. The outer edge of the pizza is sometimes referred to as the cornicione.",
            FileName = "test.txt",
            Title = "Test Document",
            Type = AttachmentType.Sign
        };


        public void UpdateFiles()
        {
            var mainDocPopulated = false;
            Document.Advanced.Attachments = Files.Count() - 1;
            foreach (var file in Files)
            {
                if (file.Type == AttachmentType.Sign && !mainDocPopulated)
                {
                    Document.Title = file.Title;
                    Document.Description = file.Description;
                    Document.DataToSign = new DataToSign()
                    {
                        Base64Content = file.Base64Content,
                        ConvertToPDF = file.ConvertToPDF,
                        FileName = file.FileName,
                        Base64ContentStyleSheet = file.Base64ContentStyleSheet
                    };
                    mainDocPopulated = true;
                }
                else
                {
                    // Todo: Add attachments
                }
            }
        }

    }
}
