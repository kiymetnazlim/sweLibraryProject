using Microsoft.AspNetCore.Mvc;

namespace LibraryProject10.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
