using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Idfy.Blazor.DemoSite.Client.Services
{
    public static class JsInterop
    {

        public static async Task<FileUpload> GetFileData(IJSRuntime jSRuntime, string fileInputRef)
        {
            var result = await jSRuntime.InvokeAsync<FileUpload>("blazorExtras.readUploadedFileAsText", fileInputRef);

            result.Content = result.Content.Split(',')[1];
            return result;
        }
    }

    public class FileUpload
    {
        public string Content { get; set; }
        public string Name { get; set; }
    }


}
