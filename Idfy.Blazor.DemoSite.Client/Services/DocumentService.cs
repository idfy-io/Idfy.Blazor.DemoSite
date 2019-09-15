using Idfy.Signature;
using Idfy.Blazor.DemoSite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Idfy.Blazor.DemoSite.Client.Services
{
    public class DocumentService : IDisposable
    {
        public DemoDocument Document { get; set; }
        public List<DemoSite.Shared.DemoFile> Files { get; set; }

        private readonly EnvironmentService environmentService;
        private HttpClient httpClient;
        private readonly NavigationManager uriHelper;
        private bool uploadAttachments;


        public DocumentService(EnvironmentService environmentService, HttpClient httpClient, NavigationManager uriHelper)
        {
            this.environmentService = environmentService;
            this.httpClient = httpClient;
            this.uriHelper = uriHelper;
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
            AddSigner("Serious", "Signer").GetAwaiter().GetResult();
        }

        public async Task AddSigner(string firstName = null, string lastName = null)
        {
            var signer = new DemoSigner()
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
                },
                Tags = new List<string>()
            };

            if(Document?.Status?.DocumentStatus != null)
            {
                signer = await AddOrUpdateSigner(signer);
            }

            Document.Signers.Add(signer);
        }

        public async Task<DemoSigner> AddOrUpdateSigner(DemoSigner signer)
        {
            var uri = $"{uriHelper.BaseUri}api/Sign/{Document.DocumentId}/AddSigner";

            if (!signer.Id.Equals(Guid.Empty))
                uri += $"?id={signer.Id}";
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(signer);
            var result = await httpClient.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
            var resultAsString = await result.Content.ReadAsStringAsync();

            VerifySuccess(result, resultAsString);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DemoSigner>(resultAsString, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public DemoSite.Shared.DemoFile DefaultTxtFile => new DemoSite.Shared.DemoFile()
        {
            Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("This text can safely be signed. This is just a text file with convert to pdf sat to true")),
            ConvertToPdf = true,
            Description = "The bottom of the pizza, called the \"crust\", may vary widely according to style, thin as in a typical hand-tossed" +
                " Neapolitan pizza or thick as in a deep-dish Chicago-style.",
            FileName = "test.txt",
            Title = Static.DocNameGenerator.GenerateTitle(),
            Type = AttachmentType.Sign
        };


        public void UpdateFiles(bool onlyUpdateMainFile = false)
        {
            if (Files.Any(f => string.IsNullOrWhiteSpace(f.Data))) Files = new List<DemoFile>() { DefaultTxtFile };

            uploadAttachments = false;
            var mainDocPopulated = false;
            Document.Advanced.Attachments = Files.Count() - 1;
            foreach (var file in Files)
            {
                if (file.Type == AttachmentType.Sign && !mainDocPopulated)
                {
                    MapMainSignFile(file);
                    mainDocPopulated = true;
                    if (onlyUpdateMainFile)
                        break;
                }
                else
                {
                    uploadAttachments = true;
                }
            }
        }

        private void MapMainSignFile(DemoFile file)
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
            file.Done = true;
        }

        public async Task UpdateAttachment(DemoFile file)
        {
            Console.WriteLine("Updating file");
            if (file.Id != null && file.Id != Document.DocumentId)
            {
                await UploadAttachment(file);
            }
            else
            {
                await UpdateDocument();
            }
        }

        public async Task GetDocument(Guid? documentId = null)
        {
            var result = await httpClient.GetAsync($"{uriHelper.BaseUri}api/Sign/{documentId ?? Document.DocumentId}");
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
            await GetAttachments();
        }

        public async Task UpdateDocument()
        {
            Console.WriteLine("Updating main document");

            UpdateFiles(true);
            var doc = Newtonsoft.Json.JsonConvert.SerializeObject(this.Document);

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{uriHelper.BaseUri}api/Sign/Update/{Document.DocumentId}")
            {
                Content = new StringContent(doc, Encoding.UTF8, "application/json")
            };

            var result = await httpClient.SendAsync(request);
            var resultAsString = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"Status code: {result.StatusCode}");
            VerifySuccess(result, resultAsString);

            //TODO: use response when new version including full response is released 
            await GetDocument();
        }

        public async Task<DemoDocument> CreateDocument()
        {
            UpdateFiles();
            var doc = Newtonsoft.Json.JsonConvert.SerializeObject(this.Document);
            var result = await httpClient.PostAsync($"{uriHelper.BaseUri}api/Sign/Create", new StringContent(doc, Encoding.UTF8, "application/json"));
            var resultAsString = await result.Content.ReadAsStringAsync();

            VerifySuccess(result, resultAsString);
            Document = Newtonsoft.Json.JsonConvert.DeserializeObject<DemoDocument>(resultAsString);

            if (uploadAttachments)
            {
                await UploadAttachments();
            }
            return Document;            
        }

        private async Task UploadAttachments()
        {
            foreach (var attachment in Files.Where(a => !a.Done))
            {
                await UploadAttachment(attachment);
            }
        }

        private async Task UploadAttachment(DemoFile attachment)
        {
            HttpResponseMessage result;
            string resultAsString;
            int attempt = 0;
            var url = $"{uriHelper.BaseUri}api/Sign/{Document.DocumentId}/Attachment";

            if(attachment.Id != null && attachment.Id !=  Document.DocumentId)
                url += $"?id={attachment.Id}";
            do
            {              
                await Task.Delay(1000 * attempt);
                result = await httpClient.PostAsync(url, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(attachment), Encoding.UTF8, "application/json"));
                resultAsString = await result.Content.ReadAsStringAsync();
                attempt++;
            } while (attempt < 5 && !result.IsSuccessStatusCode); // Do some retries in case document not ready

            VerifySuccess(result, resultAsString);
            var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<Attachment>(resultAsString);
            attachment.Id = deserialized.Id;
        }

        private async Task GetAttachments()
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
                    result = await httpClient.GetAsync($"{uriHelper.BaseUri}api/Sign/{Document.DocumentId}/Attachment/{attachment.Key}");
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

        public async Task DeleteSignature(Signer signer)
        {
            var result = await httpClient.DeleteAsync($"{uriHelper.BaseUri}api/Sign/{Document.DocumentId}/DeleteSignature/{signer.Id}");
            var resultAsString = await result.Content.ReadAsStringAsync();

            VerifySuccess(result, resultAsString);
        }



        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}
