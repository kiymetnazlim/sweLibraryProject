
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly LibraryDbContext _context;

        public HomeController(LibraryDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View(); 
        }



        public IActionResult Favorities()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
               
            }
            return View();
        }
        public IActionResult Profiles()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); 
            }

            // Retrieve the username for the logged-in user based on the userId
            var username = GetUsernameById(userId.Value); // Assuming GetUsernameById is a method that fetches the username from the database

            ViewBag.Username = username;

            return View();
        }

        private string GetUsernameById(int userId)
        {
           
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            return user?.Name ?? "Guest"; // If user is not found, return "Guest"
        }

        public IActionResult SearchPage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Eðer giriþ yapmamýþsa, giriþ sayfasýna yönlendir
            }
            // Veritabanýndan kitaplarý al
            var books = _context.Books.ToList();

            // Veriyi View'e gönder
            return View(books);
        }




    }




}



