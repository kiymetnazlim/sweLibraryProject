using Microsoft.AspNetCore.Mvc;

namespace LibraryProject10.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Views/Admin/ReservationManagement.cshtml döndürür
        }
        // Rezervasyon yönetim sayfası
        public IActionResult ReservationManagement()
        {
            return View(); // Views/Admin/ReservationManagement.cshtml döndürür
        }

        public IActionResult BookManagement()
        {
            return View(); // Ana sayfa görüntülenir
        }
    }
}
