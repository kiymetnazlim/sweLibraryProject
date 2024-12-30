using Microsoft.EntityFrameworkCore;
using LibraryProject;
using LibraryProject.Models;// MyDbContext s�n�f�n� dahil et

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext'i do�ru �ekilde ekle

// Veritabanı bağlama
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryDbContext")));

// Add distributed memory cache and session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession(); // Add this line to enable session


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Rota yap�land�rmas�
app.MapControllerRoute(
    name: "loginpage",
    pattern: "login",
    defaults: new { controller = "User", action = "LogIn" });

app.MapControllerRoute(
    name: "registerpage",
    pattern: "register",
    defaults: new { controller = "User", action = "Register" });

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
    name: "bookdetails",
    pattern: "home/book-details",
    defaults: new { controller = "Book", action = "BookDetails" });

app.MapControllerRoute(
    name: "admin",
    pattern: "admin",
    defaults: new { controller = "Admin", action = "Index" });

app.MapControllerRoute(
    name: "bookmanagement",

    pattern: "admin/book-management",
    defaults: new { controller = "Admin", action = "BookManagement" });

app.MapControllerRoute(
    name: "searchpage",
    pattern: "home/search-page",
    defaults: new { controller = "Home", action = "SearchPage" });

app.MapControllerRoute(
    name: "addbook",
    pattern: "admin/add-book",
    defaults: new { controller = "Book", action = "AddBook" });





app.Run();
