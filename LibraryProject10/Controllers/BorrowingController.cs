using Microsoft.AspNetCore.Mvc;

namespace LibraryProject10.Controllers
{
    public class BorrowingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
