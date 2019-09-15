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

        public static ValueTask SetLocalStorage(this IJSRuntime jSRuntime, string key, string value)
        {
            return jSRuntime.InvokeVoidAsync("blazorExtras.setLocalStorage", key, value);
        }

        public static ValueTask<string> GetLocalStorage(this IJSRuntime jSRuntime, string key)
        {
            return jSRuntime.InvokeAsync<string>("blazorExtras.getLocalStorage", key);
        }

        public static ValueTask ClearLocalStorage(this IJSRuntime jSRuntime)
        {
            return jSRuntime.InvokeVoidAsync("blazorExtras.clearLocalStorage");
        }
    }

    public class FileUpload
    {
        public string Content { get; set; }
        public string Name { get; set; }
    }


}
