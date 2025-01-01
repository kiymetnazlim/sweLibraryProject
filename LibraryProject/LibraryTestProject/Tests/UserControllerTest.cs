using Moq;
using NUnit.Framework;
using LibraryProject.Controllers;
using LibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryProject.Models.ViewModels;
using System.Linq;
using LibraryProject;
using Microsoft.AspNetCore.Http;

[TestFixture]
public class UserControllerTest
{
    private LibraryDbContext _context;
    private UserController _controller;

    [SetUp]
    public void SetUp()
    {
        // Create an In-Memory database
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "LibraryTestDb")
            .Options;

        _context = new LibraryDbContext(options);

        // Initialize the controller
        _controller = new UserController(_context)
        {
            ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
            }
        };

        // Clear any existing data from the in-memory database to ensure tests run independently
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Test]
    public void Register_Post_ValidModel_RedirectsToLogin()
    {
        // Arrange
        var model = new RegisterViewModel { Name = "John", Password = "password123" };

        // Act
        var result = _controller.Register(model);

        // Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;
        Assert.AreEqual("LogIn", redirectResult.ActionName);
        Assert.AreEqual("User", redirectResult.ControllerName);

        // Verify that the user is added to the DbContext
        var user = _context.Users.FirstOrDefault(u => u.Name == "John");
        Assert.IsNotNull(user);
    }

    [Test]
    public void LogIn_Post_ValidModel_RedirectsToSearchPage()
    {
        // Arrange
        var model = new LogInViewModel { Name = "John", Password = "password123" };

        // Add the user to the in-memory database
        _context.Users.Add(new User { Name = "John", Password = "password123", Role = "User" });
        _context.SaveChanges();

        // Mock HttpContext and Session
        var mockHttpContext = new Mock<HttpContext>();
        var mockSession = new Mock<ISession>();
        mockHttpContext.Setup(_ => _.Session).Returns(mockSession.Object);

        // Set ControllerContext with the mock HttpContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };

        // Act
        var result = _controller.LogIn(model);

        // Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result);  // Check if it's a redirect
        var redirectResult = (RedirectToActionResult)result;
        Assert.AreEqual("SearchPage", redirectResult.ActionName); // Correct action
        Assert.AreEqual("Home", redirectResult.ControllerName); // Correct controller
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up any resources after each test

        // Dispose the context to release the in-memory database resources
        _context?.Dispose();
        _controller.Dispose();

        // Since _controller does not need to be explicitly disposed, just nullify it
        _controller = null;
    }
    [Test]
    public void Logout_Post_ClearsSession_AndRedirectsToLogIn()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        var mockSession = new Mock<ISession>();

        // Set up the session mock to ensure it is cleared
        mockHttpContext.Setup(_ => _.Session).Returns(mockSession.Object);

        // Set ControllerContext with the mock HttpContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };

        // Act
        var result = _controller.Logout();

        // Assert: Check that session was cleared
        mockSession.Verify(s => s.Clear(), Times.Once);  // Verify that Session.Clear() was called exactly once

        // Assert: Check that the result is a redirect to the LogIn action
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;

        // Assert: Check if the correct action and controller are being used
        Assert.AreEqual("LogIn", redirectResult.ActionName); // Correct action
        Assert.AreEqual("User", redirectResult.ControllerName); // Correct controller
    }
}
