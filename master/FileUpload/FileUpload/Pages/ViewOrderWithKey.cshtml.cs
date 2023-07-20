using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileUpload.Areas.Orders.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FileUpload.Pages
{
    public class ViewOrderWithKeyModel : PageModel
    {
        private readonly OrdersDbContext _context;

        public ViewOrderWithKeyModel(OrdersDbContext context)
        {
            _context = context;
        }

      public Order Order { get; set; } = default!;
        public List<Areas.Orders.Data.File>? Files { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, Guid viewOrderKey)
        {
            if (id == null || viewOrderKey == Guid.Empty || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id && m.ViewOrderKey == viewOrderKey);
            if (order == null)
            {
                return NotFound();
            }
            else 
            {
                Order = order;
            }

            var files = _context.Files.Where(f => f.OrderId == id);
            if (files == null)
            {
                return NotFound();
            }
            else
            {
                Files = await files.ToListAsync();
            }

            return Page();
        }
    }
}
