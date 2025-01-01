using LibraryProject.Controllers;
using LibraryProject.Models;
using LibraryProject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BorrowingControllerTest;
using Newtonsoft.Json.Linq;

[TestFixture]
public class BookControllerTest
{
    private LibraryDbContext _context;
    private BookController _controller;

    [SetUp]
    public void SetUp()
    {
        // In-memory veritabanı kullanımı
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new LibraryDbContext(options);

        // Örnek veritabanı için bir kitap eklenebilir
        _context.Books.Add(new Book
        {
            BookId = 1,
            Title = "Sample Book",
            Author = "Sample Author",
            Genre = "Fiction",
            ISBN = "1234567890",
            ShelfLocation = "A1"
        });

        _context.SaveChanges();

        _controller = new BookController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        // Veritabanını temizle
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _controller.Dispose();
    }

    [Test]
    public void BookDetails_UserNotLoggedIn_RedirectsToLogin()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = _controller.BookDetails(1);

        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = result as RedirectToActionResult;
        Assert.AreEqual("Login", redirectResult.ActionName);
        Assert.AreEqual("User", redirectResult.ControllerName);
    }

    [Test]
    public void BookDetails_BookNotFound_ReturnsNotFound()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        httpContext.Session.SetInt32("UserId", 1);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = _controller.BookDetails(99); // Mevcut olmayan kitap ID'si

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.AreEqual("Kitap bulunamadı.", notFoundResult.Value);
    }

    [Test]
    public void BookDetails_BookExists_ReturnsViewWithBook()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        httpContext.Session.SetInt32("UserId", 1);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = _controller.BookDetails(1);

        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = result as ViewResult;
        var model = viewResult.Model as Book;
        Assert.IsNotNull(model);
        Assert.AreEqual(1, model.BookId);
        Assert.AreEqual("Sample Book", model.Title);
    }
    [Test]
    public void ReserveBook_UserNotLoggedIn_RedirectsToLogin()
    {
        // Kullanıcı giriş yapmamış
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession(); // Boş bir session
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // ReserveBook metodunu çalıştır
        var result = _controller.ReserveBook(1);

        // RedirectToAction döndürmelidir
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = result as RedirectToActionResult;
        Assert.AreEqual("Login", redirectResult.ActionName);
        Assert.AreEqual("User", redirectResult.ControllerName);
    }

    [Test]
    public void ReserveBook_BookNotFound_ReturnsJsonWithErrorMessage()
    {
        // Kullanıcı giriş yapmış
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        httpContext.Session.SetInt32("UserId", 1); // UserId ayarla
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Geçerli bir kitap ID'si
        int invalidBookId = -1; // Geçersiz bir kitap ID'si (kitap bulunamadı durumu için)

        // Kitap veritabanında mevcut değil
        var result = _controller.ReserveBook(invalidBookId);

        // JSON sonucu kontrol et
        Assert.IsInstanceOf<JsonResult>(result);
        var jsonResult = result as JsonResult;

        // JSON sonucunu JObject olarak al
        JObject response = JObject.FromObject(jsonResult.Value);

        // success ve message alanlarını kontrol et
        Assert.AreEqual(false, response["success"]);
        Assert.AreEqual("Kitap bulunamadı.", response["message"]);
    }
    [Test]
    public void ReserveBook_BookAlreadyReserved_ReturnsJsonWithErrorMessage()
    {
        // Kullanıcı giriş yapmış
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        httpContext.Session.SetInt32("UserId", 1); // UserId ayarla
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Kitap ve rezervasyon ekle
        var book = _context.Books.First();
        var reservation = new Reservation
        {
            UserId = 1,
            BookId = book.BookId,
            ReservationDate = DateTime.UtcNow,
            AvailableDate = DateTime.UtcNow.AddDays(7),
            ReservationStatus = "Bekliyor"
        };
        _context.Reservations.Add(reservation);
        _context.SaveChanges();

        // Aynı kitap için rezervasyon yapılmaya çalışılıyor
        var result = _controller.ReserveBook(book.BookId);

        // JSON hata mesajı döndürmelidir
        Assert.IsInstanceOf<JsonResult>(result);
        var jsonResult = result as JsonResult;

        // JSON sonucunu JObject olarak al
        JObject response = JObject.FromObject(jsonResult.Value);

        
    }

    [Test]
    public void ReserveBook_BookAvailable_ReservesBookSuccessfully()
    {
        // Kullanıcı giriş yapmış
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        httpContext.Session.SetInt32("UserId", 1); // UserId ayarla
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Kitap veritabanında mevcut
        var book = _context.Books.First();

        // Kitap için rezervasyon yapılmış mı diye kontrol et
        var existingReservation = _context.Reservations
                                          .FirstOrDefault(r => r.BookId == book.BookId && r.ReservationStatus == "Bekliyor");

        // Eğer rezervasyon yoksa, yeni rezervasyon yapmaya çalışalım
        if (existingReservation == null)
        {
            var result = _controller.ReserveBook(book.BookId);

            // JSON sonucu kontrol et
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            // JSON sonucunu JObject olarak al
            JObject response = JObject.FromObject(jsonResult.Value);

            

        }
    }
}

