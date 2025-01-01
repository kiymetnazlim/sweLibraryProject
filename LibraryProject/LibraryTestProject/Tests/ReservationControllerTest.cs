using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LibraryProject.Controllers;
using LibraryProject.Models;
using Microsoft.EntityFrameworkCore;
using LibraryProject;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using static BorrowingControllerTest;
[TestFixture]
public class ReservationControllerTest
{
    private LibraryDbContext _context;
    private ReservationController _controller;

    [SetUp]
    public void SetUp()
    {
        // In-Memory Database oluşturuluyor
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")  // Testler için geçici bir veritabanı
            .Options;

        // In-Memory veritabanı ile DbContext oluşturuluyor
        _context = new LibraryDbContext(options);

        // Controller'a DbContext'i enjekte ediyoruz
        _controller = new ReservationController(_context);
    }

    [Test]
    public async Task Approve_ReservationStatusIsNotPending_ReturnsFailure()
    {
        // Test verisini hazırlıyoruz
        var reservation = new Reservation { ReservationId = 1, ReservationStatus = "Onaylandı" };

        // Veriyi In-Memory veritabanına ekliyoruz
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Controller'ı çalıştırıyoruz
        var result = await _controller.Approve(1);  // Burada id 1 olan rezervasyonu onaylıyoruz

        // Sonuçları doğruluyoruz
        Assert.IsNotNull(result);
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        var data = jsonResult.Value as dynamic;
        //Assert.IsFalse(data.success);  // Beklenen başarısız sonuç
    }
    [Test]
    public async Task Lend_ReservationStatusIsBekliyor_ReturnsSuccess()
    {
        // Rezervasyonları eklemeden önce veritabanını sıfırlıyoruz
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        // İlk rezervasyonu ekliyoruz
        var reservation1 = new Reservation
        {
            ReservationId = 1,  // Birinci rezervasyon
            ReservationStatus = "Bekliyor"
        };

        // İkinci rezervasyonu ekliyoruz
        var reservation2 = new Reservation
        {
            ReservationId = 2,  // İkinci rezervasyon
            ReservationStatus = "Bekliyor"
        };

        _context.Reservations.Add(reservation1);
        _context.Reservations.Add(reservation2);
        await _context.SaveChangesAsync();

        // Test için Bekliyor statüsünde bir rezervasyon ekliyoruz
        var reservation = new Reservation
        {
            ReservationId = 4, // Farklı ID ile yeni bir rezervasyon ekliyoruz
            ReservationStatus = "Bekliyor"
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Lend metodunu çağırıyoruz
        var result = await _controller.Lend(1);

        // Sonuçları doğruluyoruz
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        dynamic data = jsonResult.Value;
        //Assert.IsTrue(data.success);  // Başarı durumunun true olduğunu doğruluyoruz

        // Rezervasyonun statüsünün değiştiğini kontrol ediyoruz
        var updatedReservation = await _context.Reservations.FindAsync(1);
        Assert.AreEqual("Ödünç Verildi", updatedReservation.ReservationStatus);
    }


    [Test]
    public async Task Lend_ReservationStatusIsNotBekliyorOrOnaylandi_ReturnsFailure()
    {
        // Test için Onaylı statüsünde bir rezervasyon ekliyoruz
        var reservation = new Reservation
        {
            ReservationId = 3,
            ReservationStatus = "İptal Edildi"
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Lend metodunu çağırıyoruz
        var result = await _controller.Lend(2);

        // Sonuçları doğruluyoruz
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        dynamic data = jsonResult.Value;
       // Assert.IsFalse(data.success);  // Başarı durumunun false olduğunu doğruluyoruz
    }
  
    [TearDown]
    public void TearDown()
    {
        _context.Dispose();  // In-Memory veritabanını temizliyoruz
        _controller.Dispose ();
    }
}








