using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LibraryProject.Controllers;
using LibraryProject.Models;
using Microsoft.EntityFrameworkCore;
using LibraryProject;

[TestFixture]
public class ReservationControllerTest
{
    private Mock<LibraryDbContext> _mockContext;
    private ReservationController _controller;

    [SetUp]
    public void SetUp()
    {
        // Mock DbContext setup
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _mockContext = new Mock<LibraryDbContext>(options);

        // Mock _controller
        _controller = new ReservationController(_mockContext.Object);
    }

    [Test]
    public async Task Approve_ReservationStatusIsNotPending_ReturnsFailure()
    {
        // Arrange: "Onaylanmamış" olmayan bir rezervasyon oluşturuluyor
        var reservationId = 1;

        var reservation = new Reservation
        {
            ReservationId = reservationId,
            ReservationStatus = "Onaylandı" // Durumu "Onaylandı"
        };

        // Context'e kaydediyoruz
        _mockContext.Object.Reservations.Add(reservation);
        await _mockContext.Object.SaveChangesAsync();

        // Act: Approve metodunu çağırıyoruz
        var result = await _controller.Approve(reservationId);

        // Assert: JsonResult döndüğünü kontrol ediyoruz
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);

        // Success sonucunun false olduğunu kontrol ediyoruz
        dynamic jsonResponse = jsonResult.Value;
        Assert.AreEqual(false, jsonResponse.success);
    }
    [TearDown]
    public void TearDown()
    {
       
        _controller.Dispose();
    }
}
