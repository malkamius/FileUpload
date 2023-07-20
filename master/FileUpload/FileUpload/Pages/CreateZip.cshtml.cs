using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using FileUpload.Areas.Orders.Data;
using FileUpload.Areas.Identity.Data;

namespace FileUpload.Pages
{
    public class CreateZipModel : PageModel
    {
        private readonly OrdersDbContext _context;
        private readonly UserManager<FileUploadUser> _userManager;

        public CreateZipModel(OrdersDbContext context, UserManager<FileUploadUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

      public Order Order { get; set; } = default!;
        public List<Areas.Orders.Data.File>? Files { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            else 
            {
                Order = order;
                order.Status = "Files retrieved.";
                await _context.SaveChangesAsync();
            }

            var files = _context.Files.Where(f => f.OrderId == id);
            var fileInformation = _context.Files.Where(f => f.OrderId == id);

            if (files == null)
            {
                return NotFound();
            }
            else
            {
                //Files = await files.ToListAsync();
            }

            using (var zipstream = new MemoryStream((int) fileInformation.Sum(f => f.Length)))
            {
                using (var archive = new System.IO.Compression.ZipArchive(zipstream, ZipArchiveMode.Create))
                {
                    
                    foreach (var file in files)
                    {
                        var entry = archive.CreateEntry(file.OrderId + "_" + file.FileId + "_" + file.Name);
                        using (var entrystream = entry.Open())
                        {
                            if(System.IO.File.Exists(file.FilePath))
                            using (var reader = System.IO.File.OpenRead(file.FilePath))
                            {
                                reader.CopyTo(entrystream);
                            }
                        }
                    }
                    var detailsentry = archive.CreateEntry(order.OrderId + "_details.xml");
                    var details = new XElement("OrderDetails",
                        new XAttribute("OrderId", order.OrderId),
                        new XElement("ContactName", order.Name),
                        new XElement("PhoneNumber", order.PhoneNumber),
                        new XElement("EmailAddress", order.EmailAddress),
                        new XElement("CompanyName", order.CompanyName),
                        new XElement("Address1", order.Address1),
                        new XElement("Address2", order.Address2),
                        new XElement("City", order.City),
                        new XElement("State", order.State),
                        new XElement("ZipCode", order.ZipCode),
                        new XElement("Source", order.Source),
                        new XElement("DueDate", order.DateDue),
                        new XElement("DueTime", order.LatestTimeDue),
                        new XElement("SubmissionDate", order.DateCreated),
                        new XElement("ProjectNumber", order.ProjectNumber),
                        new XElement("ProjectName", order.ProjectName),
                        new XElement("PONumber", order.PONumber),
                        new XElement("Delivery", order.Delivery),
                        new XElement("Department", order.Department),
                        new XElement("NumberOfSets", order.NumberOfSets),
                        new XElement("Size", order.Size),
                        new XElement("Binding", order.Binding),
                        new XElement("Notes", order.Notes));
                    using (var entrystream = detailsentry.Open())
                    {
                        details.Save(entrystream);
                    }
                }

                var audituser = await _userManager.GetUserAsync(User);

                AuditRecord auditRecord = new AuditRecord()
                {
                    EmailAddress = audituser?.Email ?? string.Empty,
                    UserId = audituser?.Id,
                    UserName = audituser?.UserName ?? string.Empty,
                    IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    Description = "Downloaded order " + Order.OrderId + " zip file",
                    DateTime = DateTime.Now
                };
                _context.AuditRecords.Add(auditRecord);
                await _context.SaveChangesAsync();

                return File(zipstream.ToArray(), "application/x-zip-compressed", Order.OrderId + "_files.zip");
            }
            
            return Page();
        }
    }
}
