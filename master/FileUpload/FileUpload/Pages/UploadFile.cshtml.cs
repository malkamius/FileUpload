using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using FileUpload.Areas.Orders.Data;
using FileUpload.Areas.Identity.Data;
using System.Text.Json.Serialization;

namespace FileUpload.Pages
{
    [IgnoreAntiforgeryToken]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = Int32.MaxValue)]
    public class UploadFileModel : PageModel
    {
        private readonly OrdersDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<FileUploadUser> _userManager;
        private readonly IConfiguration _configuration;

        public UploadFileModel(OrdersDbContext context, IEmailSender emailSender, UserManager<FileUploadUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
        }

        [BindProperty]
        public int OrderId { get; set; } = default!;

        [BindProperty]
        public Guid UploadFileKey { get; set; } = default!;

        [BindProperty]
        public int FileId { get; set; } = default;

        [BindProperty]
        public IFormFile file { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (int.TryParse(Request.Form["OrderId"][0], out var tOrderId)) OrderId = tOrderId;
            if(Guid.TryParse(Request.Form["UploadFileKey"][0], out var tUploadFileKey)) UploadFileKey = tUploadFileKey;
            if(int.TryParse(Request.Form["FileId"][0], out var tFileId)) FileId = tFileId;
            
            if (OrderId != 0 && UploadFileKey != Guid.Empty)
            {
                var DbOrder = _context.Orders.Where(o => o.OrderId == OrderId && o.UploadFileKey == UploadFileKey).FirstOrDefault();
                var DbFile = _context.Files.Where(f => f.OrderId == OrderId && f.FileId == FileId).FirstOrDefault();

                if (DbOrder != null && DbFile != null)
                {
                    var filedata = Request.Form.Files[0];
                    
                    using (var filestream = System.IO.File.OpenWrite(DbFile.FilePath))
                    {
                        filestream.Position = DbFile.Written;
                        filedata.CopyTo(filestream);
                        DbFile.Written = filestream.Length;
                        filestream.Close();
                    }
                    await _context.SaveChangesAsync();

                    Debug.WriteLine("File {0} received {1} bytes", DbFile.Name, filedata.Length, DbFile.OrderId);

                    if (DbFile.Length == DbFile.Written)
                    {
                        if(_context.Files.Any(f => f.OrderId == OrderId && f.Length < f.Written))
                        {
                            return new JsonResult(new { message = $"File {DbFile.FileId} uploaded successfully." });
                        }
                        else
                            return new JsonResult(new { redirect = "OrderUploaded", message = "All files uploaded for this order." });
                    }
                    else
                        return new JsonResult(new { message = $"{filedata.Length} bytes appended to file {DbFile.FileId}" });
                    
                }
                else
                    return new JsonResult(new { message = "Order or File not found." });
            }
            else
                return new JsonResult(new { message = "Invalid Order ID" });
        }
    }
}
