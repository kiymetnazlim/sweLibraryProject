using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryProject;
using LibraryProject.Controllers;
using LibraryProject.DTO;
using LibraryProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace LibraryTestProject.Tests
{
    [TestFixture]
    public class AdminControllerTest
    {
        private Mock<LibraryDbContext> _mockContext;
        private AdminController _controller;
        private Mock<DbSet<Book>> _mockBooks;
        private Mock<DbSet<User>> _mockUsers;
        private Mock<DbSet<Reservation>> _mockReservations;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<LibraryDbContext>();

            // Mock DbSets
            _mockBooks = new Mock<DbSet<Book>>();
            _mockUsers = new Mock<DbSet<User>>();
            _mockReservations = new Mock<DbSet<Reservation>>();

            _mockContext.Setup(c => c.Books).Returns(_mockBooks.Object);
            _mockContext.Setup(c => c.Users).Returns(_mockUsers.Object);
            _mockContext.Setup(c => c.Reservations).Returns(_mockReservations.Object);

            _controller = new AdminController(_mockContext.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [TearDown]
        public void TearDown()
        {
            // Kullanılan nesneleri serbest bırak
            _controller?.Dispose();

            // Mock nesneleri için ek işlem yapmaya gerek yok, Moq bunları otomatik temizler.
        }

        [Test]
        public void Index_UserNotLoggedIn_ShouldRedirectToLogin()
        {
            // Arrange
            _controller.HttpContext.Session = new Mock<ISession>().Object;

            // Act
            var result = _controller.Index();

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual("Login", redirectResult.ActionName);
            Assert.AreEqual("User", redirectResult.ControllerName);
        }

        [Test]
        public void Index_UserLoggedIn_ShouldReturnCorrectCounts()
        {
            // Arrange
            _controller.HttpContext.Session = new MockHttpSession
            {
                ["UserId"] = 1
            };

            _mockBooks.Setup(m => m.Count()).Returns(10);
            _mockUsers.Setup(m => m.Count()).Returns(5);
            _mockReservations.Setup(m => m.Count()).Returns(3);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(10, result.ViewData["BookCount"]);
            Assert.AreEqual(5, result.ViewData["UserCount"]);
            Assert.AreEqual(3, result.ViewData["reservationCount"]);
        }

        [Test]
        public async Task BookManagement_UserLoggedIn_ShouldReturnBooks()
        {
            // Arrange
            _controller.HttpContext.Session = new MockHttpSession
            {
                ["UserId"] = 1
            };

            var books = new List<Book> { new Book { BookId = 1, Title = "Book 1" } }.AsQueryable();
            _mockBooks.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.Provider);
            _mockBooks.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.Expression);
            _mockBooks.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.ElementType);
            _mockBooks.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(books.GetEnumerator());

            // Act
            var result = await _controller.BookManagement() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, (result.Model as List<Book>).Count);
        }

        [Test]
        public async Task ReservationManagement_UserLoggedIn_ShouldReturnReservations()
        {
            // Arrange
            _controller.HttpContext.Session = new MockHttpSession
            {
                ["UserId"] = 1
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    ReservationId = 1,
                    User = new User { Name = "User 1" },
                    Book = new Book { Title = "Book 1" },
                    ReservationDate = System.DateTime.Now,
                    ReservationStatus = "Active"
                }
            }.AsQueryable();

            _mockReservations.As<IQueryable<Reservation>>().Setup(m => m.Provider).Returns(reservations.Provider);
            _mockReservations.As<IQueryable<Reservation>>().Setup(m => m.Expression).Returns(reservations.Expression);
            _mockReservations.As<IQueryable<Reservation>>().Setup(m => m.ElementType).Returns(reservations.ElementType);
            _mockReservations.As<IQueryable<Reservation>>().Setup(m => m.GetEnumerator()).Returns(reservations.GetEnumerator());

            // Act
            var result = await _controller.ReservationManagement() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, (result.Model as List<ReservationDto>).Count);
        }

        [Test]
        public void Delete_BookExists_ShouldRemoveBookAndRedirect()
        {
            // Arrange
            var book = new Book { BookId = 1 };
            _mockBooks.Setup(m => m.FirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>())).Returns(book);

            // Act
            var result = _controller.Delete(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("BookManagement", result.ActionName);
            _mockBooks.Verify(m => m.Remove(book), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Delete_BookDoesNotExist_ShouldReturnErrorView()
        {
            // Arrange
            _mockBooks.Setup(m => m.FirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<System.Func<Book, bool>>>())).Returns((Book)null);

            // Act
            var result = _controller.Delete(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }
    }

    public class MockHttpSession : ISession
    {
        private readonly Dictionary<string, object> _sessionStorage = new Dictionary<string, object>();

        public object this[string name]
        {
            get => _sessionStorage.ContainsKey(name) ? _sessionStorage[name] : null;
            set => _sessionStorage[name] = value;
        }

        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public bool IsAvailable => true; // Oturumun her zaman mevcut olduğunu varsayıyoruz.

        public string Id => "MockSessionId"; // Örnek bir oturum kimliği.

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public bool TryGetValue(string key, out byte[] value)
        {
            value = _sessionStorage.ContainsKey(key) ? System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(_sessionStorage[key]) : null;
            return _sessionStorage.ContainsKey(key);
        }

        public void Set(string key, byte[] value)
        {
            _sessionStorage[key] = System.Text.Json.JsonSerializer.Deserialize<object>(value);
        }

        public void Remove(string key)
        {
            _sessionStorage.Remove(key);
        }

        public void Clear()
        {
            _sessionStorage.Clear(); // Tüm anahtar-değer çiftlerini temizler.
        }
    }
}
