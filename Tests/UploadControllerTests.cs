using Moq;
using PaperlessREST.Controllers;
using PaperlessService.InterfacesBL;
using Contract.DTOModels;
using Microsoft.AspNetCore.Mvc;
using PaperlessService.Entities;
using Microsoft.AspNetCore.Http;

namespace Tests;

[TestFixture]
public class UploadControllerTests
{
    private Mock<IUploadService> _mockUploadService;
    private UploadController _controller;

    [SetUp]
    public void Setup()
    {
        _mockUploadService = new Mock<IUploadService>();
        _controller = new UploadController(_mockUploadService.Object);
    }

    [Test]
    public async Task Upload_ValidDocument_ReturnsCreatedResponse()
    {
        // Arrange
        var testFile = CreateTestFormFile("test.pdf", "application/pdf");
        var uploadDto = new DocumentUploadDto
        {
            Title = "Test Document",
            File = testFile
        };

        var expectedDocument = new BlDocument
        {
            Id = 1,
            Name = "Test Document",
            FilePath = "test-guid.pdf",
            DateUploaded = DateTime.UtcNow
        };

        _mockUploadService
            .Setup(x => x.Upload(It.IsAny<DocumentUploadDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDocument);

        // Act
        var result = await _controller.Upload(uploadDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        
        Assert.Multiple(() =>
        {
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult?.ActionName, Is.EqualTo(nameof(UploadController.Upload)));
            Assert.That(createdResult.RouteValues["id"], Is.EqualTo(expectedDocument.Id));
            Assert.That(createdResult.Value, Is.EqualTo(expectedDocument));
        });

        _mockUploadService.Verify(
            x => x.Upload(It.IsAny<DocumentUploadDto>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Test]
    public async Task Upload_WhenServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var testFile = CreateTestFormFile("test.pdf", "application/pdf");
        var uploadDto = new DocumentUploadDto
        {
            Title = "Test Document",
            File = testFile
        };

        _mockUploadService
            .Setup(x => x.Upload(It.IsAny<DocumentUploadDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Upload failed"));

        // Act
        var result = await _controller.Upload(uploadDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result.Result as BadRequestObjectResult;
        
        Assert.Multiple(() =>
        {
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Upload failed"));
        });

        _mockUploadService.Verify(
            x => x.Upload(It.IsAny<DocumentUploadDto>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    private static IFormFile CreateTestFormFile(string fileName, string contentType)
    {
        var content = new byte[] { 0x12, 0x34, 0x56, 0x78 };
        var stream = new MemoryStream(content);
        
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.Setup(f => f.FileName).Returns(fileName);
        mockFormFile.Setup(f => f.ContentType).Returns(contentType);
        mockFormFile.Setup(f => f.Length).Returns(stream.Length);
        mockFormFile.Setup(f => f.OpenReadStream()).Returns(stream);
        
        return mockFormFile.Object;
    }
}