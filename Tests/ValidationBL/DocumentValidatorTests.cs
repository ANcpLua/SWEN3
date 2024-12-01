using FluentValidation;
using FluentValidation.TestHelper;
using PaperlessService.Entities;
using PaperlessService.Validation;
using Tests.TestHelper;

namespace Tests.ValidationBL;

[TestFixture]
public class DocumentValidatorTests
{
    private IValidator<BlDocument> _validator;

    [OneTimeSetUp]
    public void Initialize()
    {
        _validator = new DocumentValidator();
    }

    #region Name Validation Tests

    [TestCase("", "'Name' darf nicht leer sein.")]
    [TestCase("   ", "'Name' darf nicht leer sein.")]
    public async Task Validate_NameValidation_DetectsInvalidInput(string invalidName, string expectedError)
    {
        // Arrange
        var document = TestFramework.TestDataFactory.Documents.CreateBlDocument(
            TestFramework.TestDataFactory.Documents.CreateDocument(1));
        document.Name = invalidName;

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
        var document = TestFramework.TestDataFactory.Documents.CreateBlDocument(
            TestFramework.TestDataFactory.Documents.CreateDocument(
                id: 1,
                name: "Valid Document Name"
            ));

        // Act
        var validationResult = await _validator.TestValidateAsync(document);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region FilePath Validation Tests

    [TestCase("", "'File Path' darf nicht leer sein.")]
    [TestCase("  ", "'File Path' darf nicht leer sein.")]
    public async Task Validate_FilePathValidation_DetectsInvalidInput(string invalidPath, string expectedError)
    {
        // Arrange
        var document = TestFramework.TestDataFactory.Documents.CreateBlDocument(
            TestFramework.TestDataFactory.Documents.CreateDocument(1));
        document.FilePath = invalidPath;

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
        var document = TestFramework.TestDataFactory.Documents.CreateBlDocument(
            TestFramework.TestDataFactory.Documents.CreateDocument(
                id: 1,
                filePath: "/valid/file/path.pdf"
            ));

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
        var document = TestFramework.TestDataFactory.Documents.CreateBlDocument(
            TestFramework.TestDataFactory.Documents.CreateDocument(
                id: 1,
                dateUploaded: DateTime.UtcNow.AddDays(1)
            ));

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
        var document = TestFramework.TestDataFactory.Documents.CreateBlDocument(
            TestFramework.TestDataFactory.Documents.CreateDocument(
                id: 1,
                dateUploaded: DateTime.UtcNow
            ));

        // Act
        var validationResult = await _validator.TestValidateAsync(document);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.DateUploaded);
    }

    #endregion
}