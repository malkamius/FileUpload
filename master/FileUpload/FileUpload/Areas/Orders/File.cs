namespace FileUpload.Areas.Orders.Data
{
	public class File
	{
		public int FileId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;

		public long Length { get; set; } = 0;
        public int OrderId { get; set; }
		public Order Order { get; set; }
		public File(string Name, string FilePath) { 
			this.Name = Name;
			this.FilePath = FilePath;
		}

		public File() { }
	}
}
