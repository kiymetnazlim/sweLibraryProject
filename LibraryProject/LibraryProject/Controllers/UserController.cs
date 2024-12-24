using Microsoft.AspNetCore.Mvc;

namespace LibraryProject10.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
