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
        // Rezervasyon yönetim sayfası
        public async Task<IActionResult> ReservationManagement()
        {
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

        public IActionResult BookManagement()
        {
            return View(); // Ana sayfa görüntülenir
        }

        public IActionResult AddBook()
        {
            return View(); // Ana sayfa görüntülenir
        }
    }
}
