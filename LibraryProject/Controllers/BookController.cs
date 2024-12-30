using LibraryProject;
using LibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using LibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace LibraryProject.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }
        [HttpGet]

        public IActionResult BookList()
        {
            return View();
        }
        public IActionResult BookDetails(int bookId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Eğer giriş yapmamışsa, giriş sayfasına yönlendir
            }
            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);

            if (book == null)
            {
                return NotFound("Kitap bulunamadı."); // Kitap yoksa hata mesajı döndür
            }

            return View(book);
        }
        [HttpPost]
        public IActionResult ReserveBook(int bookId)
        {
            // Oturumdan kullanıcı ID'sini al
            var userId = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine($"Oturumdaki UserId: {userId}");

            // Kullanıcı giriş yapmamışsa giriş sayfasına yönlendir
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Kitap veritabanında mevcut mu kontrol et
            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);

            if (book == null)
            {
                return Json(new { success = false, message = "Kitap bulunamadı." });
            }

            // Kitap için zaten aktif bir rezervasyon olup olmadığını kontrol et
            var existingReservation = _context.Reservations
                                              .FirstOrDefault(r => r.BookId == bookId && r.ReservationStatus == "Bekliyor");

            if (existingReservation != null)
            {
                return Json(new { success = false, message = "Bu kitap zaten rezerve edilmiştir." });
            }

            // Yeni rezervasyon nesnesi oluştur
            var reservation = new Reservation
            {
                UserId = userId.Value, // Oturumdan alınan kullanıcı ID'sini kullan
                BookId = bookId,
                ReservationDate = DateTime.UtcNow,
                AvailableDate = DateTime.UtcNow.AddDays(7), // 7 gün sonra kitap kullanılabilir
                ReservationStatus = "Bekliyor"
            };

            // Veritabanına kaydet
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return Json(new { success = true, message = "Rezervasyon başarılı!" });
        }
        public IActionResult AddBook()
        {
            return View();
        }
    }


    }



