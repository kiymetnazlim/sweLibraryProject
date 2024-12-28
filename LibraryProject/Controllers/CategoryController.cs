using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
