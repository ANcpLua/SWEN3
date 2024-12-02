using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaperlessREST.Controllers;
using PaperlessService.InterfacesBL;

namespace Tests.Controller;

[TestFixture]
public class DeleteDocumentControllerTests
{
    private Mock<IDeleteDocumentService> _mockService;
    private DeleteDocumentController _controller;
    private ILogger<DeleteDocumentController> _logger;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<DeleteDocumentController>>().Object;
        _mockService = new Mock<IDeleteDocumentService>();
        _controller = new DeleteDocumentController(_mockService.Object, _logger);
    }

    [Test]
    public async Task Delete_WhenSuccessful_ReturnsNoContent()
    {
        var documentId = 1;

        var result = await _controller.Delete(documentId, CancellationToken.None);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockService.Verify(s => s.DeleteAsync(documentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}