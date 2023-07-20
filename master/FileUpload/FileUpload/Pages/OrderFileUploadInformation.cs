namespace FileUpload.Pages
{
    public class OrderFileUploadInformation
    {
        public long OrderId { get; set; } = 0;
        public long FileId { get; set; } = 0;
        public string FileName { get; set; } = string.Empty;
        public long Length { get; set; } = 0;
        public long Written { get; set; } = 0;
        public string ContentType { get; set; } = string.Empty;

        
    }
}
