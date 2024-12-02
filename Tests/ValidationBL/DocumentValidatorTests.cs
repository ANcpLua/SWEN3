using FluentValidation;
using FluentValidation.TestHelper;
using PaperlessService.Entities;
using PaperlessService.Validation;

namespace Tests.ValidationBL;

[TestFixture]
public class DocumentValidatorTests
{
    private IValidator<BlDocument> _validator;

    [OneTimeSetUp]
    public void Initialize()
    {
        _validator = new BlValidation();
    }

    #region Name Validation Tests

    [TestCase("", "'Name' must not be empty.")]
    [TestCase("   ", "'Name' must not be empty.")]
    public async Task Validate_NameValidation_DetectsInvalidInput(string invalidName, string expectedError)
    {
        // Arrange
        var document = new BlDocument
        {
            Id = 1,
            Name = invalidName,
            FilePath = "/valid/path",
            DateUploaded = DateTime.UtcNow
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(document);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Name)
                        .WithErrorMessage(expectedError);
    }

    [Test]
    public async Task Validate_NameValidation_AcceptsValidInput()
    {
        // Arrange
        var document = new BlDocument
        {
            Id = 1,
            Name = "Valid Document Name",
            FilePath = "/valid/path",
            DateUploaded = DateTime.UtcNow
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(document);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region FilePath Validation Tests

    [TestCase("", "'File Path' must not be empty.")]
    [TestCase("   ", "'File Path' must not be empty.")]
    public async Task Validate_FilePathValidation_DetectsInvalidInput(string invalidPath, string expectedError)
    {
        // Arrange
        var document = new BlDocument
        {
            Id = 1,
            Name = "Valid Document Name",
            FilePath = invalidPath,
            DateUploaded = DateTime.UtcNow
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(document);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.FilePath)
                        .WithErrorMessage(expectedError);
    }

    [Test]
    public async Task Validate_FilePathValidation_AcceptsValidInput()
    {
        // Arrange
        var document = new BlDocument
        {
            Id = 1,
            Name = "Valid Document Name",
            FilePath = "/valid/file/path.pdf",
            DateUploaded = DateTime.UtcNow
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(document);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.FilePath);
    }

    #endregion

    #region DateUploaded Validation Tests

    [Test]
    public async Task Validate_DateUploadedValidation_DetectsFutureDate()
    {
        // Arrange
        var document = new BlDocument
        {
            Id = 1,
            Name = "Valid Document Name",
            FilePath = "/valid/path",
            DateUploaded = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(document);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.DateUploaded)
                        .WithErrorMessage("Upload date cannot be in the future");
    }

    [Test]
    public async Task Validate_DateUploadedValidation_AcceptsPresentDate()
    {
        // Arrange
        var document = new BlDocument
        {
            Id = 1,
            Name = "Valid Document Name",
            FilePath = "/valid/path",
            DateUploaded = DateTime.UtcNow
        };

        // Act
        var validationResult = await _validator.TestValidateAsync(document);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.DateUploaded);
    }

    #endregion
}
