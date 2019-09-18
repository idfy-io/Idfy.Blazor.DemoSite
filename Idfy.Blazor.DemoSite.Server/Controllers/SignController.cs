using Idfy.Blazor.DemoSite.Server.Clients;
using Idfy.Signature;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Idfy.Blazor.DemoSite.Server.Controllers
{
    [Route("api/[controller]")]
    public class SignController : Controller
    {
        public SignatureServiceWrapper SignatureServiceWrapper { get; }

        public SignController(SignatureServiceWrapper signatureServiceWrapper)
        {
            SignatureServiceWrapper = signatureServiceWrapper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create([FromBody]DocumentCreateOptions request)
        {
            var env = SignatureServiceWrapper.SetEnvironment(Request.Headers);

            try
            { 
                var result = await SignatureServiceWrapper.GetService(env).CreateDocumentAsync(request);
                return Ok(result);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        [HttpPatch]
        [Route("[action]/{documentId}")]
        public async Task<IActionResult> Update(Guid documentId, [FromBody]DocumentUpdateOptions request)
        {
            var env = SignatureServiceWrapper.SetEnvironment(Request.Headers);

            try
            {
                var result = await SignatureServiceWrapper.GetService(env).UpdateDocumentAsync(documentId, request);
                return Ok(result);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("{documentId}/[action]")]
        public async Task<IActionResult> Attachment(Guid documentId, [FromBody]AttachmentOptions request, [FromQuery] Guid? id = null)
        {
            var env = SignatureServiceWrapper.SetEnvironment(Request.Headers);
            try
            {
                var service = SignatureServiceWrapper.GetService(env);
                var result = await (id == null ? service.CreateAttachmentAsync(documentId, request) : service.UpdateAttachmentAsync(documentId, id.Value, request));
                return Ok(result);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("{documentId}/[action]/{attachmentId}")]
        public async Task<IActionResult> Attachment(Guid documentId, Guid attachmentId)
        {
            var env = SignatureServiceWrapper.SetEnvironment(Request.Headers);

            try
            {
                var result = await SignatureServiceWrapper.GetService(env).GetAttachmentAsync(documentId, attachmentId);
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
            var env = SignatureServiceWrapper.SetEnvironment(Request.Headers);

            try
            {
                var result = await SignatureServiceWrapper.GetService(env).GetDocumentAsync(documentId);
                return Ok(result);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("{documentId}/[action]")]
        public async Task<IActionResult> Files(Guid documentId, [FromQuery] FileFormat fileFormat, [FromQuery] string env, [FromQuery] Guid? documentItemId = null)
        {
            env = SignatureServiceWrapper.SetEnvironment(Request.Headers, env);

            try
            {
                var result = documentItemId == null ? await SignatureServiceWrapper.GetService(env).GetFileAsync(documentId, fileFormat)
                    : await SignatureServiceWrapper.GetService(env).GetAttachmentFileAsync(documentId, documentItemId.Value, fileFormat);
                return Ok(result);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("{documentId}/[action]/{signerId}")]
        public async Task<IActionResult> DeleteSignature(Guid documentId, Guid signerId)
        {
            try
            {
                var env = SignatureServiceWrapper.SetEnvironment(Request.Headers);
                await this.SignatureServiceWrapper.GetFeaturesApiClient(env).Delete($"{IdfyConfiguration.BaseUrl}/signature/documents/{documentId}/signers/{signerId}/signature");
                return NoContent();
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        } 
        
        [HttpDelete]
        [Route("{documentId}/[action]/{signerId}")]
        public async Task<IActionResult> DeleteSigner(Guid documentId, Guid signerId)
        {
            try
            {
                var env = SignatureServiceWrapper.SetEnvironment(Request.Headers);
                await SignatureServiceWrapper.GetService(env).DeleteSignerAsync(documentId, signerId);
                return NoContent();
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("{documentId}/[action]")]
        public async Task<IActionResult> AddSigner(Guid documentId, [FromBody] SignerOptions signer, [FromQuery] Guid? id = null)
        {
            try
            {
                var env = SignatureServiceWrapper.SetEnvironment(Request.Headers);
                var service = SignatureServiceWrapper.GetService(env);
                var response = id == null ? await service.CreateSignerAsync(documentId, signer) : await service.UpdateSignerAsync(documentId, id.Value, signer);
                return Ok(response);
            }
            catch (IdfyException e)
            {
                return BadRequest(e);
            }
        }


    }
}