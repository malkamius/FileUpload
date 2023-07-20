namespace FileUpload.Areas.Orders.Data
{
    public class FileInformation
    {
        public int FileId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        
        public long Length { get; set;  }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        
        public FileInformation() { }
    }
}
