using System.Text;
using ImageMagick;
using Tesseract;

namespace OcrWorker.Tesseract;

public class Ocr : IOcrClient
{
    private readonly string? _language;
    private readonly string? _tessDataPath;

    public Ocr(OcrOptions options)
    {
        _tessDataPath = options.TessDataPath;
        _language = options.Language;
        MagickNET.Initialize();
    }

    public string OcrPdf(Stream inputStream)
    {
        var stringBuilder = new StringBuilder();
        try
        {
            using var magickImage = new MagickImage(inputStream);
            ProcessImage(magickImage, stringBuilder);
        }
        catch (MagickMissingDelegateErrorException)
        {
            try
            {
                var settings = new MagickReadSettings
                {
                    Density = new Density(300)
                };

                using var images = new MagickImageCollection();
                inputStream.Position = 0;
                images.Read(inputStream, settings);
                Console.WriteLine($"Processing {images.Count} pages...");

                foreach (var image in images) ProcessImage(image, stringBuilder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file: {ex.Message}");
                throw;
            }
        }

        return stringBuilder.ToString().Trim();
    }

    public string OcrImage(Stream imageStream)
    {
        var stringBuilder = new StringBuilder();
        try
        {
            using var magickImage = new MagickImage(imageStream);
            ProcessImage(magickImage, stringBuilder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing image file: {ex.Message}");
            throw;
        }

        return stringBuilder.ToString().Trim();
    }

    private void ProcessImage(IMagickImage image, StringBuilder stringBuilder)
    {
        try
        {
            image.Format = MagickFormat.Png;
            image.ColorSpace = ColorSpace.RGB;
            image.Alpha(AlphaOption.Remove);
            image.Enhance();
            image.Sharpen(0, 1.0);

            var imageData = image.ToByteArray();

            using var tesseract = new TesseractEngine(_tessDataPath, _language, EngineMode.Default);
            using var pix = Pix.LoadFromMemory(imageData);
            if (pix == null)
            {
                Console.WriteLine("Failed to load image into Pix");
                return;
            }

            using var page = tesseract.Process(pix);
            var text = page.GetText();
            stringBuilder.AppendLine(text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing image: {ex.Message}");
        }
    }
}