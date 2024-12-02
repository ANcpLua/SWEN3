using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaperlessREST.Controllers;
using PaperlessService.InterfacesBL;
using Microsoft.Extensions.Logging;
using Contract.DTOModels;
using Microsoft.AspNetCore.Http;

namespace Tests.Controller
{
    [TestFixture]
    public class UploadControllerTests
    {
        private Mock<IUploadService> _mockUploadService;
        private Mock<IValidator<DocumentUploadDto>> _mockUploadValidator;
        private Mock<ILogger<UploadController>> _mockLogger;
        private UploadController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUploadService = new Mock<IUploadService>();
            _mockUploadValidator = new Mock<IValidator<DocumentUploadDto>>();
            _mockLogger = new Mock<ILogger<UploadController>>();
            _controller = new UploadController(_mockUploadService.Object, _mockUploadValidator.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Upload_WhenValidationFails_ReturnsBadRequest()
        {
            // Arrange
            var uploadDto = new DocumentUploadDto();
            var validationFailures = new List<FluentValidation.Results.ValidationFailure>
            {
                new("Title", "Title is required"),
                new("File", "File is required")
            };

            _mockUploadValidator
                .Setup(v => v.ValidateAsync(uploadDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));

            // Act
            var result = await _controller.Upload(uploadDto, CancellationToken.None);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo(validationFailures));
        }

        [Test]
        public async Task Upload_WhenFileIsEmpty_ReturnsBadRequest()
        {
            // Arrange
            var uploadDto = new DocumentUploadDto
            {
                Title = "Test Document",
                File = new FormFile(Stream.Null, 0, 0, "File", "emptyfile.pdf")
            };

            _mockUploadValidator
                .Setup(v => v.ValidateAsync(uploadDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _controller.Upload(uploadDto, CancellationToken.None);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("File is required."));
        }

        [Test]
        public async Task Upload_WhenServiceFails_ReturnsInternalServerError()
        {
            // Arrange
            var uploadDto = new DocumentUploadDto
            {
                Title = "Valid Title",
                File = new FormFile(Stream.Null, 0, 1024, "File", "testfile.pdf")
            };

            _mockUploadValidator
                .Setup(v => v.ValidateAsync(uploadDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
    
            _mockUploadService
                .Setup(s => s.Upload(uploadDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Upload(uploadDto, CancellationToken.None);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult?.Value, Is.EqualTo("An error occurred while uploading the document"));
        }

        [Test]
        public async Task Upload_WhenValid_ReturnsCreatedResult()
        {
            // Arrange
            var document = new DocumentDto
            {
                Id = 1,
                Name = "Test Document",
                FilePath = "/path/to/file.pdf",
                DateUploaded = DateTime.UtcNow
            };

            var uploadDto = new DocumentUploadDto
            {
                Title = "Test Document",
                File = new FormFile(Stream.Null, 0, 1024, "File", "testfile.pdf")
            };

            _mockUploadValidator
                .Setup(v => v.ValidateAsync(uploadDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mockUploadService
                .Setup(s => s.Upload(uploadDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(document);

            // Act
            var result = await _controller.Upload(uploadDto, CancellationToken.None);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.Multiple(() =>
            {
                Assert.That(createdResult?.ActionName, Is.EqualTo(nameof(GetDocumentController.GetById)));
                Assert.That(createdResult?.RouteValues?["id"], Is.EqualTo(document.Id));
                Assert.That(createdResult?.Value, Is.EqualTo(document));
            });
        }

        [Test]
        public async Task Upload_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var uploadDto = new DocumentUploadDto();
            _mockUploadValidator
                .Setup(v => v.ValidateAsync(uploadDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockUploadService
                .Setup(s => s.Upload(uploadDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Error uploading document"));

            // Act
            var result = await _controller.Upload(uploadDto, CancellationToken.None);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult?.Value, Is.EqualTo("An error occurred while uploading the document"));
        }
    }
}
