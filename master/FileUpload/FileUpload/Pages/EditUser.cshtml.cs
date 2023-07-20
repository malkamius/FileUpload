using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FileUpload.Data;
using FileUpload.Areas.Identity.Data;

namespace FileUpload.Pages
{
    public class EditUserModel : PageModel
    {
        public string[] Roles = new[] { "Administrator", "Browse" };

        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Locked { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool IsAdministrator { get; set; }
        public bool IsBrowse { get; set; }

        private readonly UserManager<FileUploadUser> _userManager;
        public EditUserModel(UserManager<FileUploadUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            //var options = new DbContextOptions<Data.ApplicationDbContext>(new {});

            //var context = new Data.ApplicationDbContext(options);
            //UserStore<IdentityUser> userStore = new UserStore<IdentityUser>(context);

            //if (Request.Query.ContainsKey("id"))
            //{
            //    //var user = userStore.FindByIdAsync(Request.Query["id"]).Result;
            //    //Email =user.Email;
            //}
        }

        public async Task<IActionResult> OnPost()
        {
            var form = Request.Form;
            var user = _userManager.FindByIdAsync(form["UserId"]).Result;
            user.UserName = form["Name"];
            user.Email = form["Email"];
            bool.TryParse(form["ReceiveNotifications"], out var ReceiveNotifications);
            
            if (form.ContainsKey("Password") && form["Password"].ToString() != "")
            {
                var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                var result = _userManager.ResetPasswordAsync(user, token, form["Password"]).Result;
            }
            await _userManager.UpdateAsync(user);
            await _userManager.UpdateNormalizedUserNameAsync(user);
            await _userManager.UpdateNormalizedEmailAsync(user); ;
            try
            {
                await _userManager.RemoveFromRoleAsync(user, "ADMINISTRATOR");
            }
            catch { }
            try
            {
                await _userManager.RemoveFromRoleAsync(user, "BROWSE");
            }
            catch { }
            if (form["Role"] == "Administrator")
                await _userManager.AddToRoleAsync(user, "ADMINISTRATOR");
            else if (form["Role"] == "Browse")
                await _userManager.AddToRoleAsync(user, "BROWSE");
            return Page();
        }
    }
}