using AutoMapper;
using Contract.DTOModels;
using Mapper;
using Microsoft.AspNetCore.Http;
using Moq;
using PaperlessService.Entities;
using PostgreSQL.Entities;
using Tests.TestHelper;

namespace Tests.Mapper;

[TestFixture]
public class MappingTests
{
    private IMapper _mapper;
    private MapperConfiguration _configuration;

    [OneTimeSetUp]
    public void Initialize()
    {
        _configuration = new MapperConfiguration(cfg => cfg.AddProfile<Mapping>());
        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void Configuration_IsValid()
    {
        // Assert
        _configuration.AssertConfigurationIsValid();
    }

    [Test]
    public void Map_DocumentToBlDocument_MapsCorrectly()
    {
        // Arrange
        var source = TestFramework.TestDataFactory.Documents.CreateDocument(
            id: 1,
            name: "Test Document",
            filePath: "/test/path.pdf",
            dateUploaded: new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc)
        );

        // Act
        var result = _mapper.Map<BlDocument>(source);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(source.Id));
            Assert.That(result.Name, Is.EqualTo(source.Name));
            Assert.That(result.FilePath, Is.EqualTo(source.FilePath));
            Assert.That(result.DateUploaded, Is.EqualTo(source.DateUploaded));
            Assert.That(result.File, Is.Null); // File should be ignored in mapping
        });
    }

    [Test]
    public void Map_BlDocumentToDocument_WithZeroId_DoesNotMapId()
    {
        // Arrange
        var source = new BlDocument
        {
            Id = 0, // Default value
            Name = "Test Document",
            FilePath = "/test/path.pdf",
            DateUploaded = DateTime.UtcNow
        };
        
        // Act
        var result = _mapper.Map<Document>(source);

        // Assert
        Assert.That(result.Id, Is.EqualTo(0));
    }

    [Test]
    public void Map_DocumentUploadDtoToBlDocument_MapsCorrectly()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var source = new DocumentUploadDto
        {
            Title = "Test Upload",
            File = mockFile.Object
        };

        // Act
        var result = _mapper.Map<BlDocument>(source);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(source.Title));
            Assert.That(result.File, Is.EqualTo(source.File));
            Assert.That(result.Id, Is.EqualTo(0)); // Should be default
            Assert.That(result.FilePath, Is.Null); // Should be ignored
            Assert.That(result.DateUploaded.Date, Is.EqualTo(DateTime.UtcNow.Date));
        });
    }

    [Test]
    public void Map_BlDocumentToDocumentDto_MapsCorrectly()
    {
        // Arrange
        var source = new BlDocument
        {
            Id = 1,
            Name = "Test Document",
            FilePath = "/test/path.pdf",
            DateUploaded = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            File = Mock.Of<IFormFile>()
        };

        // Act
        var result = _mapper.Map<DocumentDto>(source);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(source.Name));
            // Verify other DTO properties are mapped correctly
        });
    }

    [Test]
    public void Map_BlDocumentToDocument_WithNonZeroId_MapsId()
    {
        // Arrange
        var source = new BlDocument
        {
            Id = 42,
            Name = "Test Document",
            FilePath = "/test/path.pdf",
            DateUploaded = DateTime.UtcNow
        };

        // Act
        var result = _mapper.Map<Document>(source);

        // Assert
        Assert.That(result.Id, Is.EqualTo(source.Id));
    }
}