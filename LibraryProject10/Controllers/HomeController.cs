
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
        // Rezervasyon yönetim sayfasý

        public HomeController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(); // Views/Admin/ReservationManagement.cshtml döndürür
        }
        public IActionResult LoanHistory()
        {
            // LoanHistory tablosundan tüm verileri çekiyoruz
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

            // Verileri View'a gönderiyoruz
            return View(loanHistoryData);
        }

        public IActionResult Reservation()
        {
            return View(); // Ana sayfa görüntülenir
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


