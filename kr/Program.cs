using Microsoft.EntityFrameworkCore;
using demo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=templinks.db"));

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "admin",
    defaults: new { controller = "Admin", action = "Index" });

app.MapControllerRoute(
    name: "create-link",
    pattern: "admin/createlink",
    defaults: new { controller = "Admin", action = "CreateLink" });

app.MapControllerRoute(
    name: "employee-signature",
    pattern: "employee/signature",
    defaults: new { controller = "Employee", action = "Signature" });

app.MapControllerRoute(
    name: "employee-download",
    pattern: "employee/download",
    defaults: new { controller = "Employee", action = "Download" });

app.MapControllerRoute(
    name: "address-book",
    pattern: "addressbook",
    defaults: new { controller = "AddressBook", action = "Index" });

app.MapControllerRoute(
    name: "address-book-vcard",
    pattern: "addressbook/vcard/{id}",
    defaults: new { controller = "AddressBook", action = "VCard" });

app.Run();