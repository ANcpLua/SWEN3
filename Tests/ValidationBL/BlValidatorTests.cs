using FluentValidation;
using PaperlessService.Entities;
using PaperlessService.Validation;

namespace Tests.ValidationBL;

[TestFixture]
public class BlValidatorTests
{
    private IValidator<BlDocument> _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new BlValidation();
    }

    [Test]
    public void Validate_ValidDocument_Passes()
    {
        // Arrange
        var document = new BlDocument
        {
            Name = "test.pdf",
            FilePath = "uploads/test.pdf",
            DateUploaded = DateTime.UtcNow.AddMinutes(-1)
        };

        // Act
        var result = _validator.Validate(document);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_InvalidDocument_Fails()
    {
        // Arrange
        var document = new BlDocument
        {
            Name = "",
            FilePath = "",
            DateUploaded = DateTime.UtcNow.AddDays(1) // Future date
        };

        // Act
        var result = _validator.Validate(document);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.With.Property("PropertyName").EqualTo("Name"));
            Assert.That(result.Errors, Has.Some.With.Property("PropertyName").EqualTo("FilePath"));
            Assert.That(result.Errors, Has.Some.With.Property("PropertyName").EqualTo("DateUploaded"));
        });
    }
}
