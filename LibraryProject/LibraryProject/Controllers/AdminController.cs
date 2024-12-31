using LibraryProject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LibraryProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly LibraryDbContext _context;

        public AdminController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Eğer giriş yapmamışsa, giriş sayfasına yönlendir
            }
            // Kitap sayısını veritabanından çekiyoruz
            var bookCount = _context.Books.Count();

            // Üye sayısını veritabanından çekiyoruz
            var userCount = _context.Users.Count();
            var reservationCount = _context.Reservations.Count();

            // View'a veri gönderiyoruz
            ViewData["BookCount"] = bookCount;
            ViewData["UserCount"] = userCount;
            ViewData["reservationCount"] = reservationCount;
            return View();
        }
       

         public async Task<IActionResult> BookManagement()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Eğer giriş yapmamışsa, giriş sayfasına yönlendir
            }

            var books = await _context.Books.Include(b => b.Category).ToListAsync();
            return View(books);
        }
        public async Task<IActionResult> ReservationManagement()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Eğer giriş yapmamışsa, giriş sayfasına yönlendir
            }
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Book)
                .Select(r => new ReservationDto
                {
                    ReservationId = r.ReservationId,
                    UserName = r.User.Name,
                    BookTitle = r.Book.Title,
                    ReservationDate = r.ReservationDate.ToString("dd.MM.yyyy"),
                    ReservationStatus = r.ReservationStatus
                })
                .ToListAsync();

            // Eğer veriler null ise boş bir liste gönderiyoruz.
            if (reservations == null)
            {
                reservations = new List<ReservationDto>();
            }

            return View(reservations);
        }
        [HttpGet]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                var book = _context.Books.FirstOrDefault(b => b.BookId == id);
               

                _context.Books.Remove(book);
                _context.SaveChanges();

                return RedirectToAction("BookManagement");
            }
            catch (Exception ex)
            {
                // Hata mesajını kontrol edin
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

    }


}

