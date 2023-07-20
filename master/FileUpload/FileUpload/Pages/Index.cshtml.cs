﻿using FileUpload.Areas.Orders.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FileUpload.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
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



            return Content(Order.OrderId.ToString() + "," + Order.UploadFileKey.ToString() + "," + Order.ViewOrderKey.ToString());
        }
    }
}