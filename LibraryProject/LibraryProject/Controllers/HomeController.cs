
    //public IActionResult GoToReservation()
    //{
    //    return RedirectToAction("Index", "ReservationManagement");
    //}

using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.Controllers
{
    
    public class HomeController : Controller
    {
       
       
        public IActionResult Index()
        {
            return View(); // Views/Admin/ReservationManagement.cshtml d�nd�r�r
        }
        

        public IActionResult Reservation()
        {
            return View(); // Ana sayfa g�r�nt�lenir
        }
        public IActionResult Favorities()
        {
            return View();
        }
        public IActionResult Profiles()
        {
            return View();
        }

        public IActionResult SearchPage()
        {
            return View();
        }

    }
}


