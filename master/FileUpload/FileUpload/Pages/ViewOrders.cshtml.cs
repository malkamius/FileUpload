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
    public class ViewOrdersModel : PageModel
    {
        private readonly OrdersDbContext _context;

        public ViewOrdersModel(OrdersDbContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Orders != null)
            {
                var query = (from order in _context.Orders orderby order.DateCreated descending select order);
                Order = query.ToList();
            }
        }
    }
}
