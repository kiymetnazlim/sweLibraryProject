using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LibraryProject.Controllers;
using LibraryProject.Models;
using LibraryProject;

[TestFixture]
public class BorrowingControllerTest
{
    private LibraryDbContext _context;
    private BorrowingController _controller;

    [SetUp]
    public void SetUp()
    {
        // InMemoryDatabase kullanarak bir DbContext oluşturuyoruz
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new LibraryDbContext(options);

        // Kontrolcü oluşturuyoruz
        _controller = new BorrowingController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        // Bellek içi veritabanını temizliyoruz
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _controller.Dispose();
    }

    [Test]
    public void LoanHistory_UserIsNotLoggedIn_RedirectsToLogin()
    {
        // Arrange: Session'da kullanıcı kimliği yok (giriş yapılmamış)
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession(); // Mock bir session kullanıyoruz

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        // Act: LoanHistory metodunu çağırıyoruz
        var result = _controller.LoanHistory();

        // Assert: Kullanıcı giriş yapmadığı için Login sayfasına yönlendirilmiş olmalı
        var redirectToActionResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectToActionResult);
        Assert.AreEqual("Login", redirectToActionResult.ActionName);
        Assert.AreEqual("User", redirectToActionResult.ControllerName);
    }

    [Test]
    public void LoanHistory_UserIsLoggedIn_ReturnsBorrowedBooks()
    {
        // Arrange: Kullanıcı için bir session oluşturuyoruz
        var userId = 1;
        var httpContext = new DefaultHttpContext();
        var session = new TestSession();
        session.SetInt32("UserId", userId);
        httpContext.Session = session;

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        // Kullanıcıya ait bir ödünç alınan kitap ekliyoruz
        var book = new Book
        {
            BookId = 1,
            Title = "Test Book",
            Author = "Test Author",
            Genre = "Test Genre",
            ISBN = "1234567890",
            ShelfLocation = "A1"
        };
        _context.Books.Add(book);

        var reservation = new Reservation
        {
            ReservationId = 1,
            UserId = userId,
            ReservationStatus = "Ödünç Verildi",
            Book = book // Önceden eklenen kitabı burada kullanıyoruz
        };
        _context.Reservations.Add(reservation);
        _context.SaveChanges();

        // Veritabanında eklenen verileri kontrol edelim
        Assert.AreEqual(1, _context.Reservations.Count(), "Reservations eklenemedi.");
        Assert.AreEqual(1, _context.Books.Count(), "Books eklenemedi.");

        // Act: LoanHistory metodunu çağırıyoruz
        var result = _controller.LoanHistory();

        // Assert: Sonuçları kontrol ediyoruz
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult, "LoanHistory ViewResult döndürmedi.");

        var model = viewResult.Model as List<Reservation>;
        Assert.IsNotNull(model, "Model null döndü.");

    }




    // Mock bir ISession sınıfı oluşturuyoruz



    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public bool IsAvailable => true;

        public string Id => "TestSession";

        public void Clear() => _sessionStorage.Clear();

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            // Testler için bu metot boş bırakılabilir
            return Task.CompletedTask;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            // Testler için bu metot boş bırakılabilir
            return Task.CompletedTask;
        }

    }
}


