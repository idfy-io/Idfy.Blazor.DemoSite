using Idfy.Signature;
using Idfy.Blazor.DemoSite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Components;

namespace Idfy.Blazor.DemoSite.Client.Services
{
    public class DocumentService : IDisposable
    {
        public DemoDocument Document { get; set; }
        public List<DemoSite.Shared.DemoFile> Files { get; set; }

        private readonly EnvironmentService environmentService;
        private HttpClient httpClient;
        private bool uploadAttachments;


        public DocumentService(EnvironmentService environmentService, HttpClient httpClient)
        {
            this.environmentService = environmentService;
            this.httpClient = httpClient;
            Init();
        }

        public void Init()
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
                Signers = new System.Collections.Generic.List<DemoSigner>()
            };
            AddSigner("Serious", "Signer");
        }

        public void AddSigner(string firstName = null, string lastName = null)
        {
            Document.Signers.Add(new DemoSigner()
            {
                SignerInfo = new SignerInfo()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Mobile = new Mobile()
                    {
                        CountryCode = "+47",
                    }
                },
                RedirectSettings = new RedirectSettings()
                {
                    RedirectMode = RedirectMode.DonotRedirect
                },
                ExternalSignerId = Guid.NewGuid().ToString(),
                SignatureType = new SignatureType()
                {
                    Mechanism = SignatureMechanism.PkiSignature
                },
                Ui = new UI()
                {
                    Styling = new Styling()
                },
                Notifications = new Notifications()
                {
                    Setup = new NotificationSetup()
                    {
                        Canceled = NotificationSetting.Off,
                        Expired = NotificationSetting.Off,
                        FinalReceipt = NotificationSetting.Off,
                        Reminder = NotificationSetting.Off,
                        Request = NotificationSetting.Off,
                        SignatureReceipt = NotificationSetting.Off
                    },
                    MergeFields = new Dictionary<string, string>()
                }
            });
        }

        public DemoSite.Shared.DemoFile DefaultTxtFile => new DemoSite.Shared.DemoFile()
        {
            Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("This text can safely be signed. This is just a text file with convert to pdf sat to true")),
            ConvertToPdf = true,
            Description = "The bottom of the pizza, called the \"crust\", may vary widely according to style, thin as in a typical hand-tossed" +
                " Neapolitan pizza or thick as in a deep-dish Chicago-style. It is traditionally plain, " +
                "but may also be seasoned with garlic or herbs, or stuffed with cheese. The outer edge of the pizza is sometimes referred to as the cornicione.",
            FileName = "test.txt",
            Title = Static.DocNameGenerator.GenerateTitle(),
            Type = AttachmentType.Sign
        };


        public void UpdateFiles()
        {
            uploadAttachments = false;
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
                        Base64Content = file.Data,
                        ConvertToPDF = file.ConvertToPdf,
                        FileName = file.FileName,
                        Base64ContentStyleSheet = file.Base64ContentStyleSheet
                    };
                    mainDocPopulated = true;
                    file.Done = true;
                }
                else
                {
                    uploadAttachments = true;
                }
            }
        }

        public async Task GetDocument(string baseUrl, Guid? documentId = null)
        {
            var result = await httpClient.GetAsync($"{baseUrl}api/Sign/{documentId ?? Document.DocumentId}");
            var resultAsString = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"Status code: {result.StatusCode}");
            VerifySuccess(result, resultAsString);
            Document = Newtonsoft.Json.JsonConvert.DeserializeObject<DemoDocument>(resultAsString);
            Files = new List<DemoFile>();
            Files.Add(new DemoFile()
            {
                Title = Document.Title,
                Description = Document.Description,
                ConvertToPdf = Document.DataToSign.ConvertToPDF.GetValueOrDefault(false),
                Data = Document.DataToSign.Base64Content,
                FileName = Document.DataToSign.FileName,
                Id = Document.DocumentId,
                Type = AttachmentType.Sign,
                Base64ContentStyleSheet = Document.DataToSign.Base64ContentStyleSheet
            });
            await GetAttachments(baseUrl);
        }   

        public async Task<DemoDocument> CreateDocument(string baseUrl)
        {
            UpdateFiles();
            var doc = Newtonsoft.Json.JsonConvert.SerializeObject(this.Document);
            var result = await httpClient.PostAsync($"{baseUrl}api/Sign/Create", new StringContent(doc, Encoding.UTF8, "application/json"));
            var resultAsString = await result.Content.ReadAsStringAsync();

            VerifySuccess(result, resultAsString);
            Document = Newtonsoft.Json.JsonConvert.DeserializeObject<DemoDocument>(resultAsString);

            if (uploadAttachments)
            {
                await UploadAttachments(baseUrl);
            }
            return Document;            
        }

        private async Task UploadAttachments(string baseUrl)
        {
            foreach (var attachment in Files.Where(a => !a.Done))
            {
                HttpResponseMessage result;
                string resultAsString;
                int attempt = 1;
                do
                {
                    await Task.Delay(1000 * attempt);
                    result = await httpClient.PostAsync($"{baseUrl}api/Sign/{Document.DocumentId}/Attachment", 
                        new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(attachment), Encoding.UTF8, "application/json"));
                    resultAsString = await result.Content.ReadAsStringAsync();
                    attempt++;
                } while (attempt < 5 && !result.IsSuccessStatusCode); // Do some retries in case document not ready

                VerifySuccess(result, resultAsString);
            }
        }

        private async Task GetAttachments(string baseUrl)
        {
            if (Document == null) return;
            
            foreach (var attachment in Document.Status.AttachmentPackages)
            {
                HttpResponseMessage result;
                string resultAsString;
                int attempt = 1;
                do
                {
                    await Task.Delay(1000 * attempt);
                    result = await httpClient.GetAsync($"{baseUrl}api/Sign/{Document.DocumentId}/Attachment/{attachment.Key}");
                    resultAsString = await result.Content.ReadAsStringAsync();
                    attempt++;
                } while (attempt < 5 && !result.IsSuccessStatusCode); // Do some retries in case document not ready

                VerifySuccess(result, resultAsString);
                var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<Attachment>(resultAsString);
                Files.Add(new DemoFile()
                {
                    Data = parsed.Data,
                    ConvertToPdf = parsed.ConvertToPdf.GetValueOrDefault(false),
                    Description = parsed.Description,
                    FileName = parsed.FileName,
                    Id = parsed.Id,
                    Signers = parsed.Signers,
                    Title = parsed.Title,
                    Type = parsed.Type
                });
            }
        }

        private static void VerifySuccess(HttpResponseMessage result, string resultAsString)
        {
            if (!result.IsSuccessStatusCode)
            {
                if((int)result.StatusCode == 429)
                {
                    throw new Exception(resultAsString);
                }

                throw Newtonsoft.Json.JsonConvert.DeserializeObject<IdfyException>(resultAsString);
            }
        }

        public async Task DeleteSignature(string baseUrl, Signer signer)
        {
            var result = await httpClient.DeleteAsync($"{baseUrl}api/Sign/{Document.DocumentId}/DeleteSignature/{signer.Id}");
            var resultAsString = await result.Content.ReadAsStringAsync();

            VerifySuccess(result, resultAsString);
        }



        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}
