using LibraryProject.Models;
using System;
using System.Linq;

namespace LibraryProject.Services
{
    public class ReservationService
    {
        private readonly LibraryDbContext _context;

        public ReservationService(LibraryDbContext context)
        {
            _context = context;
        }

        public void TransferReservationsToBorrowing()
        {
            // ReservationStatus "Ödünç Verildi" olan rezervasyonları al
            var reservationsToBorrow = _context.Reservations
                .Where(r => r.ReservationStatus == "Ödünç Verildi")
                .ToList();

            foreach (var reservation in reservationsToBorrow)
            {
                // Borrowing nesnesi oluştur
                var borrowing = new Borrowing
                {
                    BookName = reservation.Book.Title, // Kitap adını al
                    Author = reservation.Book.Author, // Yazar adını al
                    UserId = reservation.UserId,
                    BookId = reservation.BookId,
                    Status = "Active", // Durumu belirleyin
                    BorrowDate = DateTime.Now, // Bugünün tarihi ödünç tarihi
                    DueDate = DateTime.Now.AddDays(14), // Örneğin 14 gün sonra iade tarihi
                    ReturnDate = null,
                    OverdueFine = null
                };

                // Borrowing tablosuna ekle
                _context.Borrowing.Add(borrowing);

               
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın veya daha detaylı işleme yapın
                Console.WriteLine($"Hata: {ex.Message}");
            }

        }
    }
}

