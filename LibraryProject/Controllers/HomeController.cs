
    //public IActionResult GoToReservation()
    //{
    //    return RedirectToAction("Index", "ReservationManagement");
    //}

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
            return View(); // Views/Admin/ReservationManagement.cshtml d�nd�r�r
        }
        

       
        public IActionResult Favorities()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // E�er giri� yapmam��sa, giri� sayfas�na y�nlendir
            }
            return View();
        }
        public IActionResult Profiles()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // E�er giri� yapmam��sa, giri� sayfas�na y�nlendir
            }
            return View();
        }

        public IActionResult SearchPage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // E�er giri� yapmam��sa, giri� sayfas�na y�nlendir
            }
            // Veritaban�ndan kitaplar� al
            var books = _context.Books.ToList();

            // Veriyi View'e g�nder
            return View(books);
        }

    }

    }



