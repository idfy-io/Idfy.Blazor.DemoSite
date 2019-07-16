using Idfy.Signature;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Idfy.Blazor.DemoSite.Server.Controllers
{
    [Route("api/[controller]")]
    public class SignController : Controller
    {
        private readonly AppSettings appSettings;

        public ISignatureService SignatureService { get; }

        public SignController(ISignatureService signatureService, AppSettings appSettings)
        {
            SignatureService = signatureService;
            this.appSettings = appSettings;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task <IActionResult> Create([FromBody]DocumentCreateOptions request)
        {
            SetEnvironment(Request.Headers);

            try
            {
                var result = await SignatureService.CreateDocumentAsync(request);
                return Ok(result);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }

       

        [HttpGet]
        [Route("{documentId}")]
        public async Task<IActionResult> Get(Guid documentId)
        {
            SetEnvironment(Request.Headers);

            try
            {
                var result = await SignatureService.GetDocumentAsync(documentId);
                return Ok(result);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("{documentId}/[action]")]
        public async Task<IActionResult> Files(Guid documentId, [FromQuery] FileFormat fileFormat, [FromQuery] Guid? documentItemId = null)
        {
            SetEnvironment(Request.Headers);

            try
            {
                var result = documentItemId == null ? await SignatureService.GetFileAsync(documentId, fileFormat)
                    : await SignatureService.GetAttachmentFileAsync(documentId, documentItemId.Value, fileFormat);
                return Ok(result);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }

        private void SetEnvironment(IHeaderDictionary headers)
        {
            string environmentName = "Test";
            if (headers.TryGetValue("Idfy-Environment", out var result) && result.Count > 0)
            {
                environmentName = result.ToArray()[0];
            }

            var environment = appSettings.Environments[environmentName];

            if (!string.IsNullOrWhiteSpace(environment.ApiBaseUrl))
            {
                IdfyConfiguration.BaseUrl = environment.ApiBaseUrl;
            }
            if (!string.IsNullOrWhiteSpace(environment.OauthBaseUrl))
            {
                IdfyConfiguration.OAuthBaseUrl = environment.OauthBaseUrl;
            }

            IdfyConfiguration.SetClientCredentials(
                environment.ClientId,
                environment.ClientSecret,
                new[] { OAuthScope.DocumentRead, OAuthScope.DocumentWrite, OAuthScope.DocumentFile });
        }
    }
}