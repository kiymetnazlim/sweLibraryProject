using Microsoft.EntityFrameworkCore;
using LibraryProject; // MyDbContext s�n�f�n� dahil et

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext'i do�ru �ekilde ekle

// Veritabanı bağlama
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryDbContext")));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Rota yap�land�rmas�
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "reservationmanagement",
    pattern: "admin/reservation-management",
    defaults: new { controller = "Admin", action = "ReservationManagement" });

app.MapControllerRoute(
    name: "loanHistory",
    pattern: "home/loan-history",
    defaults: new { controller = "Borrowing", action = "LoanHistory" });

app.MapControllerRoute(
    name: "reservation",
    pattern: "home/reservation",
    defaults: new { controller = "Home", action = "Reservation" });

app.MapControllerRoute(
    name: "favorities",
    pattern: "home/favorities",
    defaults: new { controller = "Home", action = "Favorities" });

app.MapControllerRoute(
    name: "profiles",
    pattern: "home/profiles",
    defaults: new { controller = "Home", action = "Profiles" });

app.MapControllerRoute(
    name: "booklist",
    pattern: "booklist",
    defaults: new { controller = "Book", action = "BookList" });

app.MapControllerRoute(
    name: "bookdetails",
    pattern: "booklist/book-details",
    defaults: new { controller = "Book", action = "BookDetails" });

app.MapControllerRoute(
    name: "admin",
    pattern: "admin",
    defaults: new { controller = "Admin", action = "Index" });

app.MapControllerRoute(
    name: "bookmanagement",
    pattern: "admin/book-management",
    defaults: new { controller = "Admin", action = "BookManagement" });





app.Run();
