using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Xml.Linq;
using System.Configuration;
using FileUpload.Areas.Orders.Data;
using FileUpload.Areas.Identity.Data;

namespace FileUpload.Pages
{
    public class OrderSuccessfulModel : PageModel
    {
        private readonly OrdersDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<FileUploadUser> _userManager;
        private readonly IConfiguration _configuration;

        public OrderSuccessfulModel(OrdersDbContext context, IEmailSender emailSender, UserManager<FileUploadUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
        }

        public Order Order { get; set; } = default!;
        public List<Areas.Orders.Data.File> Files { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id, Guid? viewOrderGuid)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id && m.ViewOrderKey == viewOrderGuid);
            if (order == null)
            {
                return NotFound();
            }
            else
            {
                if (order.Status == "Waiting for files to upload.")
                    order.Status = "Awaiting file retrieval.";
                await _context.SaveChangesAsync();

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
            if (_emailSender is EmailService)
            {
                var detailsString = "OrderDetails<br />\n" +
                                        "OrderId = " + Order.OrderId + "<br />\n" +
                        "ContactName = " + Order.Name + "<br />\n" +
                        "PhoneNumber = " + Order.PhoneNumber + "<br />\n" +
                        "EmailAddress = " + Order.EmailAddress + "<br />\n" +
                        "CompanyName = " + Order.CompanyName + "<br />\n" +
                        "Address1 = " + Order.Address1 + "<br />\n" +
                        "Address2 = " + Order.Address2 + "<br />\n" +
                        "City = " + Order.City + "<br />\n" +
                        "State = " + Order.State + "<br />\n" +
                        "ZipCode = " + Order.ZipCode + "<br />\n" +
                        //"Source = " + Order.Source + "<br />\n" +
                        "DueDate = " + Order.DateDue + "<br />\n" +
                        "DueTime = " + Order.LatestTimeDue + "<br />\n" +
                        "SubmissionDate = " + Order.DateCreated + "<br />\n" +
                        "ProjectNumber = " + Order.ProjectNumber + "<br />\n" +
                        "ProjectName = " + Order.ProjectName + "<br />\n" +
                        "PONumber = " + Order.PONumber + "<br />\n" +
                        //"Delivery = " + Order.Delivery + "<br />\n" +
                        //"Department = " + Order.Department + "<br />\n" +
                        //"NumberOfSets = " + Order.NumberOfSets + "<br />\n" +
                        //"Size = " + Order.Size + "<br />\n" +
                        //"Binding = " + Order.Binding + "<br />\n" +
                        "Notes = " + Order.Notes + "<br />\n";
                detailsString = $"<table>\r\n        " +
                    "<tr><td colspan=\"2\" style=\"color: #fff;background: #1018CC;border: 1px solid #781351; padding: 2px 6px; font: 10px verdana,'trebuchet MS',helvetica,sans-serif;font-weight: bold;margin-bottom: 4px;margin-top: 4px;margin-left: 0px;text-align: center!important;\">Contact Information</td></tr>\r\n" +
                    $"<tr><td>\r\n            OrderId\r\n        </td>\r\n        <td>\r\n            {Order.OrderId}\r\n        </td></tr>\r\n" +
                    $"<tr><td>\r\n            DateCreated\r\n        </td>\r\n        <td>\r\n            {Order.DateCreated}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            Name\r\n        </td>\r\n        <td>\r\n            {Order.Name}\r\n        </td></tr>\r\n" +
                    $"<tr><td>\r\n            PhoneNumber\r\n        </td>\r\n        <td>\r\n            {Order.PhoneNumber}\r\n        </td></tr>\r\n" +
                    $"<tr><td>\r\n            EmailAddress\r\n        </td>\r\n        <td>\r\n            <a href=\"mailto:{Order.EmailAddress}\">{Order.EmailAddress}</a>\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            CompanyName\r\n        </td>\r\n        <td>\r\n            {Order.CompanyName}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            Address1\r\n        </td>\r\n        <td>\r\n            {Order.Address1}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            Address2\r\n        </td>\r\n        <td>\r\n            {Order.Address2}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            City\r\n        </td>\r\n        <td>\r\n            {Order.City}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            State\r\n        </td>\r\n        <td>\r\n            {Order.State}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            ZipCode\r\n        </td>\r\n        <td>\r\n            {Order.ZipCode}\r\n        </td></tr>\r\n        " +


                    "<tr><td colspan=\"2\" style=\"color: #fff;background: #1018CC;border: 1px solid #781351; padding: 2px 6px; font: 10px verdana,'trebuchet MS',helvetica,sans-serif;font-weight: bold;margin-bottom: 4px;margin-top: 4px;margin-left: 0px;text-align: center!important;\">Due</td></tr>\r\n" +
                    $"<tr><td>\r\n            DateDue\r\n        </td>\r\n        <td>\r\n            {Order.DateDue}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            LatestTimeDue\r\n        </td>\r\n        <td>\r\n            {Order.LatestTimeDue}\r\n        </td></tr>\r\n        " +

                    "<tr><td colspan=\"2\" style=\"color: #fff;background: #1018CC;border: 1px solid #781351; padding: 2px 6px; font: 10px verdana,'trebuchet MS',helvetica,sans-serif;font-weight: bold;margin-bottom: 4px;margin-top: 4px;margin-left: 0px;text-align: center!important;\">Accounting</td></tr>\r\n" +
                    $"<tr><td>\r\n            ProjectNumber\r\n        </td>\r\n        <td>\r\n            {Order.ProjectNumber}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            PONumber\r\n        </td>\r\n        <td>\r\n            {Order.PONumber}\r\n        </td></tr>\r\n        " +
                    $"<tr><td>\r\n            ProjectName\r\n        </td>\r\n        <td>\r\n            {Order.ProjectName}\r\n        </td></tr>\r\n        " +

                    "<tr><td colspan=\"2\" style=\"color: #fff;background: #1018CC;border: 1px solid #781351; padding: 2px 6px; font: 10px verdana,'trebuchet MS',helvetica,sans-serif;font-weight: bold;margin-bottom: 4px;margin-top: 4px;margin-left: 0px;text-align: center!important;\">Production Specifics</td></tr>\r\n" +
                    $"<tr><td>\r\n            Instructions\r\n        </td>\r\n        <td>\r\n            {Order.Notes}\r\n        </td></tr>\r\n";


                if (Files.Count > 0)
                {
                    detailsString += $"<tr><td colspan=\"2\" style=\"color: #fff;background: #1018CC;border: 1px solid #781351; padding: 2px 6px; font: 10px verdana,'trebuchet MS',helvetica,sans-serif;font-weight: bold;margin-bottom: 4px;margin-top: 4px;margin-left: 0px;text-align: center!important;\">Attached Files {Files.Count}</td></tr>\r\n";

                    foreach (var file in Files)
                    {
                        detailsString += $"<tr><td colspan=\"2\">\r\n            {file.Name} - {file.Length}\r\n        </td></tr>\r\n";
                    }
                }
                detailsString += "    </table>";

                var service = (EmailService)_emailSender;
                foreach (var user in _userManager.Users)
                {
                    if ((await _userManager.IsInRoleAsync(user, "ADMINISTRATOR") || await _userManager.IsInRoleAsync(user, "BROWSE")))
                    {
                        

                        var details = new XElement("OrderDetails",
                        new XAttribute("OrderId", Order.OrderId),
                        new XElement("ContactName", Order.Name),
                        new XElement("PhoneNumber", Order.PhoneNumber),
                        new XElement("EmailAddress", Order.EmailAddress),
                        new XElement("CompanyName", Order.CompanyName),
                        new XElement("Address1", Order.Address1),
                        new XElement("Address2", Order.Address2),
                        new XElement("City", Order.City),
                        new XElement("State", Order.State),
                        new XElement("ZipCode", Order.ZipCode),
                        new XElement("Source", Order.Source),
                        new XElement("DueDate", Order.DateDue),
                        new XElement("DueTime", Order.LatestTimeDue),
                        new XElement("SubmissionDate", Order.DateCreated),
                        new XElement("ProjectNumber", Order.ProjectNumber),
                        new XElement("ProjectName", Order.ProjectName),
                        new XElement("PONumber", Order.PONumber),
                        new XElement("Delivery", Order.Delivery),
                        new XElement("Department", Order.Department),
                        new XElement("NumberOfSets", Order.NumberOfSets),
                        new XElement("Size", Order.Size),
                        new XElement("Binding", Order.Binding),
                        new XElement("Notes", Order.Notes));

                        using (var entrystream = new MemoryStream())
                        {
                            details.Save(entrystream);

                            var attachments = new MimeKit.AttachmentCollection();
                            var attachment = new MimePart("text", "plain")
                            {
                                Content = new MimeContent(entrystream, ContentEncoding.Default),
                                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                                ContentTransferEncoding = ContentEncoding.Base64,
                                FileName = "Order " + Order.OrderId + " details.xml"
                            };
                            attachments.Add(attachment);

                            
                            await service.SendEmailAsync(user.Email, "New Order Submitted", "Order successfully placed. OrderID " + Order.OrderId + " - " + Files.Count + " files uploaded. <br /><a href=\"https://adtech.kbs-cloud.com/ViewOrder?id=" + Order.OrderId + "\">View Order</a><br />\n" + detailsString, attachments);

                        }
                    }
                    else
                        await _emailSender.SendEmailAsync(user.Email, "New Order Submitted", "Order successfully placed. OrderID " + Order.OrderId + " - " + Files.Count + " files uploaded. <br /><a href=\"https://adtech.kbs-cloud.com/ViewOrder?id=" + Order.OrderId + "\">View Order</a>");
                }

                if (Order != null && Order.EmailAddress != null)
                    await service.SendEmailAsync(Order.EmailAddress, "New Order Submitted", "Order successfully placed. OrderID " + Order.OrderId + " - " + Files.Count + " files uploaded. <br /><a href=\"https://adtech.kbs-cloud.com/ViewOrderWithKey?id=" + Order.OrderId + "&viewOrderKey=" + Order.ViewOrderKey.ToString() + "\">View Order</a><br />\n" + detailsString);
            }
            return Page();
        }
    }
}
