namespace FileUpload.Areas.Orders.Data
{
    public class AuditRecord
    {
        public long AuditId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        
        public DateTime DateTime { get; set; }
        public AuditRecord() { }
    }
}
