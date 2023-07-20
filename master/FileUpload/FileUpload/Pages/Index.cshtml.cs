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

            var files = Request.Form["FileInformation"][0];

            var files1 = JsonSerializer.Deserialize<List<FileData>>(files);
            return Content("");
            //return JsonContent.Create(Content(Order.OrderId.ToString() + "," + Order.UploadFileKey.ToString() + "," + Order.ViewOrderKey.ToString()));
        }


        public class FileData
        {
            public string FileName { get; set; }
            public int Length { get; set; }
        }
    }

}