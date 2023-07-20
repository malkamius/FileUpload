using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Identity;
using FileUpload.Areas.Orders.Data;
using FileUpload.Areas.Identity.Data;

namespace FileUpload.Pages
{
    public class ViewFileModel : PageModel
    {
        private readonly OrdersDbContext _context;
        private readonly UserManager<FileUploadUser> _userManager;

        public ViewFileModel(OrdersDbContext context, UserManager<FileUploadUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

      //public Data.File File { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id, bool download = false)
        {
            if (id == null || _context.Files == null)
            {
                return NotFound();
            }

            var file = _context.Files.Where(m => m.FileId == id).FirstOrDefault();
            if (file == null || !System.IO.File.Exists(file.FilePath))
            {
                return NotFound();
            }
            else if(!download)
            {
                Response.Headers.Add("Content-Disposition", "inline; filename=" + file.Name);
                var reader = System.IO.File.OpenRead(file.FilePath);
                {
                    var result = File(reader, !string.IsNullOrEmpty(file.ContentType)?  file.ContentType : "application/octet-stream");
                    var audituser = await _userManager.GetUserAsync(User);

                    AuditRecord auditRecord = new AuditRecord()
                    {
                        EmailAddress = audituser?.Email ?? string.Empty,
                        UserId = audituser?.Id,
                        UserName = audituser?.UserName ?? string.Empty,
                        IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                        Description = "Downloaded file " + file.FileId,
                        DateTime = DateTime.Now
                    };
                    _context.AuditRecords.Add(auditRecord);
                    await _context.SaveChangesAsync();
                    return result;
                }
            }
            else
            {
                var audituser = await _userManager.GetUserAsync(User);

                AuditRecord auditRecord = new AuditRecord()
                {
                    EmailAddress = audituser?.Email ?? string.Empty,
                    UserId = audituser?.Id,
                    UserName = audituser?.UserName ?? string.Empty,
                    IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    Description = "Viewed file " + file.FileId,
                    DateTime = DateTime.Now
                };
                _context.AuditRecords.Add(auditRecord);
                await _context.SaveChangesAsync();
                var reader = System.IO.File.OpenRead(file.FilePath);
                {
                    var result = File(reader, !string.IsNullOrEmpty(file.ContentType) ? file.ContentType : "application/octet-stream", file.Name);
                    return result;
                }
            }

            
            return Page();
        }
    }

}
