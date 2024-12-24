using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryProject.Controllers
{
    public class BorrowingController : Controller
    {
        private readonly LibraryDbContext _context;

        public BorrowingController(LibraryDbContext context)
        {
            _context = context;
        }

        // Ödünç Alma Geçmişi Görünümü
        public async Task<IActionResult> LoanHistory()
        {
            // Borrowing tablosundan tüm verileri çekiyoruz
            var borrowings = await _context.Borrowing
                .Include(b => b.Book) // Kitap bilgilerini dahil et
                .Include(b => b.User) // Kullanıcı bilgilerini dahil et
                .ToListAsync();

            // Verileri görünüme gönder
            return View(borrowings);
        }
    }
}
