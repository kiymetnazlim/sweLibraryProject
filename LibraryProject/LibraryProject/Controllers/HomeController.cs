
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
            return View(); // Views/Admin/ReservationManagement.cshtml döndürür
        }
        

       
        public IActionResult Favorities()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Eðer giriþ yapmamýþsa, giriþ sayfasýna yönlendir
            }
            return View();
        }
        public IActionResult Profiles()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Eðer giriþ yapmamýþsa, giriþ sayfasýna yönlendir
            }
            return View();
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



