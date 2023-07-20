using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FileUpload.Areas.Orders.Data
{

	public class OrdersDbContext : DbContext
	{
		public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
		: base(options)
		{
			
		}

		public DbSet<Order> Orders { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FileInformation> FileInformation { get; set; }

        public DbSet<AuditRecord> AuditRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			//modelBuilder.Entity<Order>().Property(property => property.DateCreated).HasDefaultValue(DateTime.Now);
			modelBuilder.Entity<Order>()
				.HasMany(order => order.Files).WithOne(file => file.Order)
				.HasPrincipalKey(order=> order.OrderId)
				.HasForeignKey(file => file.OrderId);

			modelBuilder.Entity<AuditRecord>()
				.HasKey(auditrecord => auditrecord.AuditId);

            modelBuilder.Entity<FileInformation>()
				.HasNoKey().ToView("FileInformation");

        }

	}
}
