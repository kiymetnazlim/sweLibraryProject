using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibraryProject.Controllers;
using System.Text;
using LibraryProject;
using Microsoft.EntityFrameworkCore;
using LibraryProject.Models;

[TestFixture]
public class HomeControllerTests
{
    private HomeController _controller;
    private Mock<HttpContext> _mockHttpContext;
    private Mock<ISession> _mockSession;
    private LibraryDbContext _context;  // In-memory context

    [SetUp]
    public void SetUp()
    {
        // Mock HttpContext ve Session
        _mockHttpContext = new Mock<HttpContext>();
        _mockSession = new Mock<ISession>();

        // HttpContext'in Session'ını mock'a yönlendir
        _mockHttpContext.Setup(c => c.Session).Returns(_mockSession.Object);



        // HomeController'ı oluştur
        _controller = new HomeController(null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            }
        };

    }


    [Test]
    public void Favorities_WhenUserIsNotLoggedIn_RedirectsToLogin()
    {
        // Arrange: Session'da UserId yok
        _mockSession.Setup(s => s.TryGetValue("UserId", out It.Ref<byte[]>.IsAny)).Returns(false);

        // Act: Favorities metodunu çağır
        var result = _controller.Favorities();

        // Assert: Login sayfasına yönlendirme yapılmalı
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;
        Assert.AreEqual("Login", redirectResult.ActionName);
        Assert.AreEqual("User", redirectResult.ControllerName);
    }

    [Test]
    public void Favorities_WhenUserIsLoggedIn_ReturnsViewResult()
    {
        // Arrange: Session'da UserId mevcut
        var userIdBytes = BitConverter.GetBytes(1); // 1 değerini byte[] olarak ayarla
        _mockSession.Setup(s => s.TryGetValue("UserId", out It.Ref<byte[]>.IsAny))
                    .Callback(new TryGetValueCallback((string key, out byte[] value) =>
                    {
                        value = userIdBytes; // Session'dan dönecek değer
                    }))
                    .Returns(true);

        // Act: Favorities metodunu çağır
        var result = _controller.Favorities();

        // Assert: ViewResult döndürülmeli
        Assert.IsInstanceOf<ViewResult>(result);
    }

    [TearDown]
    public void TearDown()
    {
        // Controller'ın dispose edilmesi
        if (_controller is IDisposable disposableController)
        {
            disposableController.Dispose();
        }

        _controller = null; // Nesneyi null yaparak çöp toplayıcının almasına izin veriyoruz
    }
    [Test]
    public void Profiles_WhenUserIsLoggedIn_ReturnsViewResult()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase("LibraryTestDb") // In-memory database kullanıyoruz
            .Options;

        var dbContextMock = new LibraryDbContext(options);

        // Session mock
        var sessionMock = new Mock<ISession>();
        sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                   .Returns(true) // başarılı dönüş
                   .Callback((string key, out byte[] value) =>
                   {
                       value = BitConverter.GetBytes(1); // "UserId" için değer olarak 1
                   });

        // HttpContext mock
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(h => h.Session).Returns(sessionMock.Object);

        // ControllerContext ayarlama
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContextMock.Object
        };

        // HomeController'ı mock context ile başlatma
        var controller = new HomeController(dbContextMock)
        {
            ControllerContext = controllerContext
        };

        // Act
        var result = controller.Profiles();

        // Assert
        Assert.IsInstanceOf<ViewResult>(result); // ViewResult bekleniyor
    }



    [Test]
    public void SearchPage_WhenUserIsNotLoggedIn_ReturnsRedirectToLogin()
    {
        // Arrange: Session mock'lanıyor, UserId eksik
        var sessionMock = new Mock<ISession>();
        sessionMock.Setup(s => s.TryGetValue("UserId", out It.Ref<byte[]>.IsAny))
                   .Callback((string key, out byte[] value) =>
                   {
                       value = null; // Kullanıcı oturumda yok
                   })
                   .Returns(false);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        var controller = new HomeController(null) // Eğer DbContext gerekliyse ekleyin
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            }
        };

        // Act: SearchPage çağrılıyor
        var result = controller.SearchPage();

        // Assert: RedirectToActionResult kontrolü
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirectResult = result as RedirectToActionResult;
        Assert.AreEqual("Login", redirectResult.ActionName); // Login sayfasına yönlendirme
    }


   
       
    }



public static class SessionExtensions
{
    public static void SetInt32(this ISession session, string key, int value)
    {
        session.Set(key, BitConverter.GetBytes(value));
    }

    public static int? GetInt32(this ISession session, string key)
    {
        if (session.TryGetValue(key, out var value) && value != null && value.Length == 4)
        {
            return BitConverter.ToInt32(value, 0);
        }
        return null;
    }

    
}

public delegate void TryGetValueCallback(string key, out byte[] value);
