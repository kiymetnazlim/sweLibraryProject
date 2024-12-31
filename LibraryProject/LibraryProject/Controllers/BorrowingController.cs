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
            var borrowedBooks = _context.Borrowing
                .Where(b => b.UserId == userId) // UserId'ye göre filtreleme
                .Include(b => b.Book) // Kitap bilgilerini de include et
                .ToList();

            return View(borrowedBooks); // View'a ödünç alınan kitapları gönderiyoruz
        }


        [HttpPost]
        public async Task<IActionResult> TransferBorrowedBooks()
        {
            // Status değeri "Ödünç Verildi" olan Reservation kayıtlarını alın
            var reservations = await _context.Reservations
                .Where(r => r.ReservationStatus == "Ödünç Verildi")
                .Include(r => r.Book)  // Kitap bilgilerini dahil et
                .Include(r => r.User)  // Kullanıcı bilgilerini dahil et
                .ToListAsync();

            if (reservations.Any())
            {
                foreach (var reservation in reservations)
                {
                    // Borrowing nesnesi oluştur ve alanları doldur
                    var borrowing = new Borrowing
                    {
                        UserId = reservation.UserId,
                        BookId = reservation.BookId,
                        BookName = reservation.Book.Title, // Kitap başlığı
                        Author = reservation.Book.Author, // Yazar
                        BorrowDate = DateTime.Now, // Ödünç alınma tarihi
                        DueDate = DateTime.Now.AddDays(14), // 2 hafta sonrası
                        Status = "Borrowed" // Varsayılan durum
                    };

                    // Borrowing tablosuna ekle
                    _context.Borrowing.Add(borrowing);
                }

                // Veritabanını kaydet
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Veriler başarıyla aktarıldı." });
            }

            return Json(new { success = false, message = "Aktarılacak veri bulunamadı." });
        }

    }
}
