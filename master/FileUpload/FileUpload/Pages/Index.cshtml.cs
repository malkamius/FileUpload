using FileUpload.Areas.Identity.Data;
using FileUpload.Areas.Orders.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FileUpload.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly OrdersDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<FileUploadUser> _userManager;
        private readonly IConfiguration _configuration;
        public IndexModel(OrdersDbContext context, IEmailSender emailSender, UserManager<FileUploadUser> userManager, IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }
        
        public void OnGet()
        {
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)// || _context.Orders == null || Order == null)
            {
                return Page();
            }
            
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddYears(1) };

            Response.Cookies.Append("Name", Order.Name ?? "", cookieOptions);
            Response.Cookies.Append("PhoneNumber", Order.PhoneNumber ?? "", cookieOptions);
            Response.Cookies.Append("EmailAddress", Order.EmailAddress ?? "", cookieOptions);
            Response.Cookies.Append("CompanyName", Order.CompanyName ?? "", cookieOptions);
            Response.Cookies.Append("Address1", Order.Address1 ?? "", cookieOptions);
            Response.Cookies.Append("Address2", Order.Address2 ?? "", cookieOptions);
            Response.Cookies.Append("City", Order.City ?? "", cookieOptions);
            Response.Cookies.Append("State", Order.State ?? "", cookieOptions);
            Response.Cookies.Append("ZipCode", Order.ZipCode ?? "", cookieOptions);

            Order.OrderId = 0;
            Order.DateCreated = DateTime.Now;
            Order.ViewOrderKey = Guid.NewGuid();
            Order.UploadFileKey = Guid.NewGuid();
            
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();
            
            try
            {
                var audituser = await _userManager.GetUserAsync(User);

                AuditRecord auditRecord = new AuditRecord()
                {
                    EmailAddress = audituser != null ? audituser.Email : string.Empty,
                    UserId = audituser != null ? audituser.Id : "",
                    UserName = audituser != null ? audituser.UserName : string.Empty,
                    IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    Description = "Added order record " + Order.OrderId,
                    DateTime = DateTime.Now
                };
                _context.AuditRecords.Add(auditRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            if (Order.OrderId != 0)
            {
                var result = new OrderInsertResult();
                List<OrderFileUploadInformation>? files;

                result.OrderId = Order.OrderId;
                result.ViewOrderKey = Order.ViewOrderKey ?? Guid.Empty;
                result.UploadFileKey = Order.UploadFileKey ?? Guid.Empty;

                if (Request.Form.ContainsKey("FileInformation") && Request.Form["FileInformation"].Count > 0 &&
                    (files = JsonSerializer.Deserialize<List<OrderFileUploadInformation>>(Request.Form["FileInformation"][0])) != null && files.Count > 0)
                {
                    Order.Status = "Waiting for files to upload.";
                    await _context.SaveChangesAsync();
                    foreach (var file in files)
                    {
                        var DbFile = new Areas.Orders.Data.File() { OrderId = Order.OrderId, FilePath = file.FileName, Name = file.FileName, ContentType = file.ContentType, Length = file.Length };
                        _context.Files.Add(DbFile);
                        await _context.SaveChangesAsync();
                        file.FileId = DbFile.FileId;
                        result.FileInformation.Add(file);

                        string filePath =
                        string.Format("{0}\\{1}\\{2}\\{2}_{3}.{4}",
                        System.IO.Path.GetDirectoryName(_configuration.GetValue<string>("AttachmentPath") ?? "C:\\data\\"),
                        Math.Floor((decimal)DbFile.OrderId / 1000).ToString().PadLeft(7, '0'),
                        DbFile.OrderId, DbFile.FileId, System.IO.Path.GetExtension(DbFile.Name).TrimStart('.'));
                        DbFile.FilePath = filePath;

                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    Order.Status = "Order complete, no files supplied.";
                    await _context.SaveChangesAsync();
                }

                // successfully added order/files
                return new JsonResult(result);// Content(JsonSerializer.Serialize(result));
            }
            else
                throw new Exception("Failed to add order.");
            //return JsonContent.Create(Content(Order.OrderId.ToString() + "," + Order.UploadFileKey.ToString() + "," + Order.ViewOrderKey.ToString()));
        }

        

        
    }

}