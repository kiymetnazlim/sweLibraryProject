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
        

          
        }

       

        
        }
   

