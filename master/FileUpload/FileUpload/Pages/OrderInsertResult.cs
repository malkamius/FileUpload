using static FileUpload.Pages.IndexModel;

namespace FileUpload.Pages
{
    public class OrderInsertResult
    {
        public long OrderId { get; set; } = 0;
        public Guid UploadFileKey { get; set; } = Guid.Empty;

        public List<OrderFileUploadInformation> FileInformation { get; set; } = new List<OrderFileUploadInformation>();
    }
}
