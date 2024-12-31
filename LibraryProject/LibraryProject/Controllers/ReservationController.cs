using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using LibraryProject.DTO;
using LibraryProject.Models;
using System;

namespace LibraryProject.Controllers
{
    public class ReservationController : Controller
    {
        private readonly LibraryDbContext _context;

        public ReservationController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null && reservation.ReservationStatus == "Bekliyor")
            {
                reservation.ReservationStatus = "Onaylandı";
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Lend(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null && (reservation.ReservationStatus == "Bekliyor" || reservation.ReservationStatus == "Onaylandı"))
            {
                reservation.ReservationStatus = "Ödünç Verildi";
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // Yeni: Ödünç verilen kitapları Borrowing tablosuna aktaran metod
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

