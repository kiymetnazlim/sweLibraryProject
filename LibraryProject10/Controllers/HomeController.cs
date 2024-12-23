
    //public IActionResult GoToReservation()
    //{
    //    return RedirectToAction("Index", "ReservationManagement");
    //}

using Microsoft.AspNetCore.Mvc;

namespace LibraryProject10.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;
        // Rezervasyon y�netim sayfas�

        public HomeController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(); // Views/Admin/ReservationManagement.cshtml d�nd�r�r
        }
        public IActionResult LoanHistory()
        {
            // LoanHistory tablosundan t�m verileri �ekiyoruz
            var loanHistoryData = _context.Borrowing
                .Select(l => new
                {
                    l.BorrowId,
                    l.Book.Title,
                    l.Book.Author,
                    l.BorrowDate,
                    l.ReturnDate,
                    l.Status,
                    l.OverdueFine
                })
                .ToList();

            // Verileri View'a g�nderiyoruz
            return View(loanHistoryData);
        }

        public IActionResult Reservation()
        {
            return View(); // Ana sayfa g�r�nt�lenir
        }
        public IActionResult Favorities()
        {
            return View();
        }
        public IActionResult Profiles()
        {
            return View();
        }

    }
}


