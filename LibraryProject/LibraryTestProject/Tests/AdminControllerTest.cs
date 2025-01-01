using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryProject.Controllers;
using LibraryProject.Models;
using LibraryProject;
using LibraryProject.DTO;

[TestFixture]
public class AdminControllerTests
{
    private Mock<LibraryDbContext> _mockContext;
    private AdminController _controller;
    private Mock<HttpContext> _mockHttpContext;
    private Mock<ISession> _mockSession;
    private LibraryDbContext _context;
    private Mock<LibraryDbContext> _dbContextMock;


   
        [SetUp]
    public void SetUp()
    {
        // In-memory database
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase("LibraryTestDb")
            .Options;

        _context = new LibraryDbContext(options);

        // Mock HttpContext and Session
        _mockHttpContext = new Mock<HttpContext>();
        _mockSession = new Mock<ISession>();

        _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);

        // Initialize AdminController
        _controller = new AdminController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            }
        };
    }

    
    


    [Test]
    public async Task BookManagement_WhenUserIsNotLoggedIn_RedirectsToLogin()
    {
        // Arrange
        var sessionMock = new Mock<ISession>();
        var key = "UserId";
        var sessionValues = new Dictionary<string, byte[]>();

        sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                   .Returns((string k, out byte[] value) =>
                   {
                       return sessionValues.TryGetValue(k, out value);
                   });

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.SetupGet(h => h.Session).Returns(sessionMock.Object);

        var controllerContext = new ControllerContext
        {
            HttpContext = httpContextMock.Object
        };

        _controller.ControllerContext = controllerContext;

        // Act
        var result = await _controller.BookManagement();

        // Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = result as RedirectToActionResult;
        Assert.AreEqual("Login", redirectResult.ActionName);
        Assert.AreEqual("User", redirectResult.ControllerName);
    }


    [Test]
    public async Task BookManagement_WhenUserIsLoggedIn_ReturnsViewWithBooks()
    {
        // Arrange
        var sessionMock = new Mock<ISession>();
        var key = "UserId";
        var userId = 1;
        var sessionValues = new Dictionary<string, byte[]>
    {
        { key, BitConverter.GetBytes(userId) }
    };

        sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                   .Returns((string k, out byte[] value) =>
                   {
                       return sessionValues.TryGetValue(k, out value);
                   });

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.SetupGet(h => h.Session).Returns(sessionMock.Object);

        var controllerContext = new ControllerContext
        {
            HttpContext = httpContextMock.Object
        };

        _controller.ControllerContext = controllerContext;

        // Add some books to the in-memory database
        var category = new Category
        {
            Name = "Category 1",
            Description = "Test Category"
        };

        var book = new Book
        {
            Title = "Test Book",
            Author = "Author Name",
            Genre = "Fiction",
            ISBN = "123-4567890123",
            ShelfLocation = "A1",
            Category = category
        };

        _context.Categories.Add(category);
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.BookManagement();

        // Assert
        Assert.IsInstanceOf<ViewResult>(result);
        var viewResult = result as ViewResult;
        var model = viewResult.Model as List<Book>;
        Assert.IsNotNull(model);
        Assert.AreEqual(1, model.Count);
        Assert.AreEqual("Test Book", model[0].Title);
    }
    [Test]
    public async Task ReservationManagement_WhenUserIsNotLoggedIn_RedirectsToLogin()
    {
        // Arrange
        var sessionMock = new Mock<ISession>();
        var httpContextMock = new Mock<HttpContext>();
        sessionMock.Setup(s => s.TryGetValue("UserId", out It.Ref<byte[]>.IsAny))
                   .Returns(false); // Session'da UserId yok

        httpContextMock.SetupGet(h => h.Session).Returns(sessionMock.Object);

        var controllerContext = new ControllerContext
        {
            HttpContext = httpContextMock.Object
        };

        _controller.ControllerContext = controllerContext;

        // Act
        var result = await _controller.ReservationManagement();

        // Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = result as RedirectToActionResult;
        Assert.AreEqual("Login", redirectResult.ActionName);
        Assert.AreEqual("User", redirectResult.ControllerName);
    }

    [Test]
    public void Delete_WhenBookDoesNotExist_ReturnsErrorView()
    {
        // Arrange
        int nonExistentBookId = 999; // ID of a book that does not exist

        // Act
        var result = _controller.Delete(nonExistentBookId);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual("Error", viewResult.ViewName);
    }
   

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _controller.Dispose();
    }
}
public static class DbSetMockExtensions
{
    public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> source) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(source.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());

        return mockSet;
    }
}
