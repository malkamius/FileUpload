using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FileUpload.Data;
using FileUpload.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath("C:\\Users\\Giaken\\AppData\\Roaming\\Microsoft\\UserSecrets\\317bdbe5-08f4-45f7-a2e9-15557b88abb9").AddJsonFile("Secrets.json");
var connectionString = builder.Configuration.GetConnectionString("FileUploadIdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'FileUploadIdentityContextConnection' not found.");

builder.Services.AddDbContext<FileUploadIdentityContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<FileUploadUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<FileUploadIdentityContext>();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();

app.Run();
