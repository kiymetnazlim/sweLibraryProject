using LibraryProject;
using LibraryProject.Controllers;
using LibraryProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryTestProject.Tests
{
    [TestFixture]
    public class BookControllerTest
    {
        private LibraryDbContext _context;
        private BookController _controller;

        [SetUp]
        public void Setup()
        {
            // Bellek içi veritabanı ayarları
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryTestDb")
                .Options;

            _context = new LibraryDbContext(options);
            _controller = new BookController(_context);

            // Mock HttpContext (Oturum için)
            var httpContextMock = new Mock<HttpContext>();
            var sessionMock = new Mock<ISession>();

            // Mock oturum verisi
            sessionMock.Setup(x => x.GetInt32("UserId")).Returns(1);
            httpContextMock.Setup(x => x.Session).Returns(sessionMock.Object);

            // Controller'a oturum bağlanıyor
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TearDown]
        public void TearDown()
        {
            // Veritabanını temizleme
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void BookList_ReturnsViewResult()
        {
            // Act
            var result = _controller.BookList();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsNull(viewResult.ViewName); // Varsayılan görünüm döndürülmeli
        }

        [Test]
        public void BookDetails_RedirectsToLogin_IfUserNotLoggedIn()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var sessionMock = new Mock<ISession>();

            sessionMock.Setup(x => x.GetInt32("UserId")).Returns((int?)null);
            httpContextMock.Setup(x => x.Session).Returns(sessionMock.Object);
            _controller.ControllerContext.HttpContext = httpContextMock.Object;

            // Act
            var result = _controller.BookDetails(1);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Login", redirectResult.ActionName);
            Assert.AreEqual("User", redirectResult.ControllerName);
        }

        [Test]
        public void BookDetails_ReturnsNotFound_IfBookNotExists()
        {
            // Act
            var result = _controller.BookDetails(999); // Geçersiz kitap ID'si

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.AreEqual("Kitap bulunamadı.", notFoundResult.Value);
        }

        [Test]
        public async Task AddBook_ReturnsView_WithSuccessMessage()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "Title", "Test Kitap" },
                { "Author", "Test Yazar" },
                { "Genre", "Roman" },
                { "ISBN", "1234567890" },
                { "PublicationYear", "2024" },
                { "ShelfLocation", "A-1" }
            });

            // Act
            var result = await _controller.AddBook(form);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.AreEqual("Kitap başarıyla eklendi!", viewResult.ViewData["Message"]);
        }

        [Test]
        public void ReserveBook_ReturnsJson_WithErrorIfBookExists()
        {
            // Arrange
            _context.Books.Add(new Book { BookId = 1, Title = "Test Kitap" });
            _context.Reservations.Add(new Reservation { BookId = 1, ReservationStatus = "Bekliyor" });
            _context.SaveChanges();

            // Act
            var result = _controller.ReserveBook(1);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = (JsonResult)result;
            var data = (Dictionary<string, object>)jsonResult.Value;
            Assert.IsFalse((bool)data["success"]);
            Assert.AreEqual("Bu kitap zaten rezerve edilmiştir.", data["message"]);
        }
    }
}
