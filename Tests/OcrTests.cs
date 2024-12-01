using ImageMagick;
using Moq;
using Nest;
using OcrWorker.Tesseract;

[TestFixture]
public class OcrTests
{
    private Mock<IImageProcessor> _mockImageProcessor;
    private Mock<IOcrEngine> _mockEngine;
    private Mock<IPage> _mockPage;
    private Ocr _sut;

    [SetUp]
    public void Setup()
    {
        _mockImageProcessor = new Mock<IImageProcessor>();
        _mockEngine = new Mock<IOcrEngine>();
        _mockPage = new Mock<IPage>();
        
        var options = new OcrOptions
        {
            TessDataPath = "test/path",
            Language = "eng"
        };

        _sut = new Ocr(
            options,
            _mockImageProcessor.Object,
            (path, lang, mode) => _mockEngine.Object
        );
    }

    [Test]
    public void OcrImage_WithValidImage_ReturnsExpectedText()
    {
        // Arrange
        const string expectedText = "Sample OCR Text";
        var imageStream = new MemoryStream();
        var processedBytes = new byte[] { 1, 2, 3 };

        _mockImageProcessor
            .Setup(x => x.ProcessImageToBytes(It.IsAny<IMagickImage>()))
            .Returns(processedBytes);

        _mockPage
            .Setup(x => x.GetText())
            .Returns(expectedText);

        _mockEngine
            .Setup(x => x.Process(It.IsAny<IPix>()))
            .Returns(_mockPage.Object);

        // Act
        var result = _sut.OcrImage(imageStream);

        // Assert
        Assert.That(result, Is.EqualTo(expectedText));
        _mockImageProcessor.Verify(x => x.ProcessImageToBytes(It.IsAny<IMagickImage>()), Times.Once);
        _mockEngine.Verify(x => x.Process(It.IsAny<IPix>()), Times.Once);
        _mockPage.Verify(x => x.GetText(), Times.Once);
    }

    [Test]
    public void OcrImage_WhenImageProcessingFails_ThrowsOcrException()
    {
        // Arrange
        var imageStream = new MemoryStream();
        _mockImageProcessor
            .Setup(x => x.ProcessImageToBytes(It.IsAny<IMagickImage>()))
            .Throws<InvalidOperationException>();

        // Act & Assert
        Assert.Throws<OcrException>(() => _sut.OcrImage(imageStream));
    }
}

public interface IImageProcessor
{
    byte[] ProcessImageToBytes(IMagickImage image);
}
{
}