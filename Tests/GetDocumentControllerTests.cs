using Contract.DTOModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaperlessREST.Controllers;
using PaperlessService.InterfacesBL;
using PostgreSQL.Entities;
using Tests.TestHelper;

namespace Tests;

[TestFixture]
public class GetDocumentControllerTests
{
    private Mock<IGetDocumentService> _mockService;
    private GetDocumentController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IGetDocumentService>();
        _controller = new GetDocumentController(_mockService.Object);
    }

    [Test]
    public async Task GetById_WhenDocumentExists_ReturnsOkResult()
    {
        var documentId = 1;
        var document = TestFramework.TestDataFactory.Documents.CreateDocument(documentId);
        var blDocument = TestFramework.TestDataFactory.Documents.CreateBlDocument(document);

        _mockService
            .Setup(s => s.GetByIdAsync(documentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(blDocument);

        var result = await _controller.GetById(documentId);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnedDto = okResult?.Value as DocumentDto;

        Assert.Multiple(() =>
        {
            Assert.That(returnedDto, Is.Not.Null);
            Assert.That(returnedDto?.Id, Is.EqualTo(documentId));
            Assert.That(returnedDto?.Name, Is.EqualTo(document.Name));
            Assert.That(returnedDto?.FilePath, Is.EqualTo(document.FilePath));
            Assert.That(returnedDto?.DateUploaded, Is.EqualTo(document.DateUploaded));
        });
    }
    
    [Test]
    public async Task GetAll_ReturnsAllDocuments()
    {
        var documents = new List<Document>
        {
            TestFramework.TestDataFactory.Documents.CreateDocument(1),
            TestFramework.TestDataFactory.Documents.CreateDocument(2)
        };

        var blDocuments = documents
            .Select(d => TestFramework.TestDataFactory.Documents.CreateBlDocument(d))
            .ToList();

        _mockService
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(blDocuments);

        var result = await _controller.GetAll();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnedDtos = okResult?.Value as IEnumerable<DocumentDto>;

        Assert.Multiple(() =>
        {
            Assert.That(returnedDtos, Is.Not.Null);
            Assert.That(returnedDtos?.Count(), Is.EqualTo(documents.Count));
        });
    }
}