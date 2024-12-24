using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
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