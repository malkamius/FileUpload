using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace FileUpload.Pages
{
    public class TempPhysicalFileResult : PhysicalFileResult
    {
        // https://stackoverflow.com/questions/54901479/how-to-delete-a-file-after-it-was-streamed-in-asp-net-core

        public TempPhysicalFileResult(string path, string contentType, string downloadfilename)
                     : base(path, contentType) 
        { 
            this.FileDownloadName = downloadfilename;
        }
        
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            try
            {
                await base.ExecuteResultAsync(context);
            }
            finally
            {
                File.Delete(FileName);
            }
        }
    }
}
