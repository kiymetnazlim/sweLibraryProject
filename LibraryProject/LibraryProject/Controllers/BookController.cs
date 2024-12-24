using Microsoft.AspNetCore.Mvc;

namespace LibraryProject10.Controllers
{
    public class BookController : Controller
    {
        public IActionResult BookList()
        {
            return View();
        }
        public IActionResult BookDetails()
        {
            return View();
        }
    }
}
