// USERCONTROLLER
using LibraryProject.Models.ViewModels;
using LibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryProject;

public class UserController : Controller
{
    private readonly LibraryDbContext _context;

    public UserController(LibraryDbContext context)
    {
        _context = context;
    }

    // Register GET
    public IActionResult Register()
    {
        return View();
    }

    // Register POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Yeni kullanıcı nesnesi oluştur
            var user = new User
            {
                Name = model.Name,
                Password = model.Password, // Şifreleme yapılması önerilir
                Role = "User" // Varsayılan olarak 'User' rolü ekleniyor
            };

            try
            {
                // Mevcut en yüksek UserId'yi al
                var lastUserId = _context.Users.Max(u => (int?)u.UserId) ?? 0;

                // Yeni UserId değerini belirle (son UserId'nin bir fazlası)
                user.UserId = lastUserId + 1;

                // Kullanıcıyı veritabanına ekle
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw; // Hata mesajını loglayıp fırlatabilirsiniz
            }

            return RedirectToAction("LogIn", "User");
        }

        return View(model);
    }


    // LogIn GET
    public IActionResult LogIn()
    {
        return View();
    }

    // LogIn POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LogIn(LogInViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == model.Name && u.Password == model.Password);

            if (user != null)
            {
                // Session değerlerini ayarla
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.Name);

                if (user.Role == "Admin")
                {
                    HttpContext.Session.SetString("UserRole", "Admin"); // Rolü session'a ekle
                    return RedirectToAction("Index", "Admin");
                }
                else if (user.Role == "User")
                {
                    HttpContext.Session.SetString("UserRole", "User"); // Kullanıcı rolünü session'a ekle
                    return RedirectToAction("SearchPage", "Home");
                }

            }

            ModelState.AddModelError("", "Invalid name or password.");
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // Oturumu temizler
        return RedirectToAction("LogIn","User"); // LogIn sayfasına yönlendirir
    }

}