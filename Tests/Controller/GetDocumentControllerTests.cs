using Contract.DTOModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaperlessREST.Controllers;
using PaperlessService.InterfacesBL;
using PostgreSQL.Entities;

namespace Tests.Controller;

[TestFixture]
public class GetDocumentControllerTests
{
    private Mock<IGetDocumentService> _mockService;
    private GetDocumentController _controller;
    private Mock<ILogger<GetDocumentController>> _mockLogger;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<GetDocumentController>>();
        _mockService = new Mock<IGetDocumentService>();
        _controller = new GetDocumentController(_mockService.Object, _mockLogger.Object);
    }

    #region GetContent
    
            
    
    
    [Test]
    public async Task GetById_WhenDocumentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var documentId = 1;
        _mockService
            .Setup(s => s.GetByIdAsync(documentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.GetById(documentId, CancellationToken.None);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult?.Value, Is.EqualTo("Document not found."));
    }
   
    [Test]
    public async Task GetMetadata_WhenDocumentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var documentId = 1;
        _mockService
            .Setup(s => s.GetMetadataAsync(documentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.GetMetadata(documentId, CancellationToken.None);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult?.Value, Is.EqualTo("Document not found."));
    }

    [Test]
    public async Task GetContent_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var documentId = 1;
        _mockService
            .Setup(s => s.GetFileAsync(documentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetContent(documentId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult?.Value, Is.EqualTo("An error occurred while retrieving the file"));
    }

    [Test]
    public async Task GetById_WhenDocumentExists_ReturnsDocument()
    {
        // Arrange
        var documentId = 1;
        var document = new DocumentDto
        {
            Id = documentId,
            Name = "Document Name",
            FilePath = "/path/to/file",
            DateUploaded = DateTime.UtcNow
        };

        _mockService
            .Setup(s => s.GetByIdAsync(documentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        // Act
        var result = await _controller.GetById(documentId, CancellationToken.None);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(document));
    }

    [Test]
    public async Task GetMetadata_WhenDocumentExists_ReturnsDocumentMetadata()
    {
        // Arrange
        var documentId = 1;
        var document = new DocumentDto
        {
            Id = documentId,
            Name = "Test Document",
            FilePath = "/path/to/doc",
            DateUploaded = DateTime.UtcNow
        };

        _mockService
            .Setup(s => s.GetMetadataAsync(documentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        // Act
        var result = await _controller.GetMetadata(documentId, CancellationToken.None);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(document));
    }

    [Test]
    public async Task GetContent_WhenFileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var documentId = 1;
        _mockService
            .Setup(s => s.GetFileAsync(documentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string.Empty, string.Empty));

        // Act
        var result = await _controller.GetContent(documentId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult?.Value, Is.EqualTo("Content file not found for this document."));
    }

    [Test]
    public async Task GetContent_WhenFileExists_ReturnsPhysicalFile()
    {
        // Arrange
        var documentId = 1;
        var filePath = "/path/to/file.pdf";
        var contentType = "application/pdf";

        _mockService
            .Setup(s => s.GetFileAsync(documentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((filePath, contentType));

        // Act
        var result = await _controller.GetContent(documentId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<PhysicalFileResult>());
        var physicalFileResult = result as PhysicalFileResult;
        Assert.Multiple(() =>
        {
            Assert.That(physicalFileResult!.FileName, Is.EqualTo(filePath));
            Assert.That(physicalFileResult.ContentType, Is.EqualTo(contentType));
        });
    }

    [Test]
    public async Task GetAll_ReturnsAllDocuments()
    {
        var documents = new List<DocumentDto>
        {
            new DocumentDto
            {
                Id = 1,
                Name = "Document 1",
                FilePath = "/path/to/doc1",
                DateUploaded = DateTime.UtcNow.AddDays(-1)
            },
            new DocumentDto
            {
                Id = 2,
                Name = "Document 2",
                FilePath = "/path/to/doc2",
                DateUploaded = DateTime.UtcNow
            }
        };

        _mockService
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(documents);

        var result = await _controller.GetAll(CancellationToken.None); 

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnedDtos = okResult?.Value as IEnumerable<DocumentDto>;

        Assert.Multiple(() =>
        {
            var documentDtos = returnedDtos!.ToList();
            Assert.That(documentDtos, Is.Not.Null);
            Assert.That(documentDtos?.Count(), Is.EqualTo(documents.Count));
            Assert.That(documentDtos, Is.EquivalentTo(documents));
        });
    }

    [Test]
    public async Task GetAll_WhenNoDocumentsExist_ReturnsEmptyList()
    {
        _mockService
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DocumentDto>());

        var result = await _controller.GetAll(CancellationToken.None);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnedDtos = okResult?.Value as IEnumerable<DocumentDto>;

        Assert.Multiple(() =>
        {
            var documentDtos = returnedDtos!.ToList();
            Assert.That(documentDtos, Is.Not.Null);
            Assert.That(documentDtos?.Any(), Is.False);
        });
    }
    
    #endregion
}
