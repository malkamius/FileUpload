using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace FileUpload.Areas.Orders.Data
{
	public class Order
	{
		public long OrderId { get; set; }
		[Required]
		public string? Name { get; set; }
		[Required]
		public string? PhoneNumber { get; set; }
		[Required]
        [EmailAddress]
        public string? EmailAddress { get; set; }
		[Required]
		public string? CompanyName { get; set; }
		[Required]
		public string? Address1 { get; set; }
		public string? Address2 { get; set; }
		[Required]
		public string? City { get; set; }
		[Required]
		public string? State { get; set; }
		[Required]
		public string? ZipCode { get; set; }
		public string? Source { get; set; }
		
		public string? DateDue { get;set; }
		public DateTime? DateCreated { get;set; }
        
        public string? LatestTimeDue { get;set; }
		public string? ProjectNumber { get; set; }
		public string? PONumber { get; set; }
		public string? ProjectName { get; set; }
		public string? Delivery { get; set; }
		public string? Department { get; set; }
		public int? NumberOfSets { get; set; }
		public string? Size { get; set; }
		public string? Binding { get; set; }
		public string? Notes { get; set; }

		public string? Status { get; set; } = default(string);

		public Guid? ViewOrderKey { get; set; }
		public Guid? UploadFileKey { get; set; }


		public List<File>? Files { get; set; }

		public Order (string Name, string PhoneNumber, string EmailAddress, 
			string Address1, string Address2, string City, string State, string ZipCode,
			string Source, string DateDue, string LatestTimeDue, 
			string ProjectNumber, string PONumber, string ProjectName,
			string Department, int NumberOfSets, string Size, string Binding, 
			string Notes)
		{
			this.Name = Name;
			this.PhoneNumber = PhoneNumber;
			this.EmailAddress = EmailAddress;
				
			this.Address1 = Address1;
			this.Address2 = Address2;
			this.City = City;
			this.State = State;
			this.ZipCode = ZipCode;

			this.Source = Source;
			this.DateDue = DateDue;
			this.LatestTimeDue = LatestTimeDue;
			this.ProjectNumber = ProjectNumber;
			this.PONumber = PONumber;
			this.ProjectName = ProjectName;
			this.Department = Department;
			this.NumberOfSets = NumberOfSets;
			this.Size = Size;
			this.Binding = Binding;
			this.Notes = Notes;
			
			this.DateCreated = DateTime.Now;

			Files = new List<File> ();
		}

		public Order()
		{
			
		}
	}
}
