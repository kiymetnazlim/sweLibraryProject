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
            var user = new User
            {
                Name = model.Name,
                Password = model.Password // Şifreleme yapmanız önerilir.
            };

            try
            {
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
                    return RedirectToAction("Index", "Admin"); // Admin sayfası
                }
                else
                {
                    return RedirectToAction("SearchPage", "Home"); // Kullanıcı sayfası
                    //NORMAL KULLANICI SAYFASINA GİDECEK
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
        return RedirectToAction("LogIn"); // LogIn sayfasına yönlendirir
    }

}