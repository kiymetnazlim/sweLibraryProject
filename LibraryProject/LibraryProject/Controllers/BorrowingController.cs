using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryProject.Models;

namespace LibraryProject.Controllers
{
    public class BorrowingController : Controller
    {
        private readonly LibraryDbContext _context;


        public BorrowingController(LibraryDbContext context)
        {
            _context = context;

        }



        public IActionResult LoanHistory()
        {
            // Kullanıcı kimliğini session'dan alıyoruz
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Eğer giriş yapmamışsa, giriş sayfasına yönlendir
            }

            // Kullanıcının ödünç aldığı kitapları sorguluyoruz
            var borrowedBooks = _context.Reservations
            .Where(b => b.UserId == userId && b.ReservationStatus == "Ödünç Verildi") // Koşul birleşimi tek bir lambda fonksiyonu içinde
            .Include(b => b.Book) // Kitap bilgilerini de include et
            .ToList();

            return View(borrowedBooks); // View'a ödünç alınan kitapları gönderiyoruz

        }
    }
}