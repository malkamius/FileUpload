using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FileUpload.Pages
{
    public class EditUsersModel : PageModel
    {
        private readonly ILogger<EditUsersModel> _logger;

        public EditUsersModel(ILogger<EditUsersModel> logger)
        {
            _logger = logger;
            
        }

        public void OnGet()
        {
          
        }

        public void OnPost()
        {
            RedirectToPage("Index");
        }

        public void OnPostEdit(int id)
        {
            RedirectToPage("Index");
        }
    }
}