using Microsoft.AspNetCore.Mvc;
using Moq;
using PaperlessREST.Controllers;
using PaperlessService.InterfacesBL;

namespace Tests;

[TestFixture]
public class DeleteDocumentControllerTests
{
    private Mock<IDeleteDocumentService> _mockService;
    private DeleteDocumentController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IDeleteDocumentService>();
        _controller = new DeleteDocumentController(_mockService.Object);
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