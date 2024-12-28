
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
            return View(); // Views/Admin/ReservationManagement.cshtml döndürür
        }
        

        public IActionResult Reservation()
        {
            return View(); // Ana sayfa görüntülenir
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


