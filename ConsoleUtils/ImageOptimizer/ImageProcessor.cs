using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Encoder = System.Drawing.Imaging.Encoder;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ConsoleUtils.ImageOptimizer
{
    public class ImageProcessor
    {
        public void OptimizeImagesInFolder(string folderPath, int maxWidth = 1000, long quality = 75L)
        {
            var imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

            var outputFolder = Path.Combine(folderPath, "Optimized");
            Directory.CreateDirectory(outputFolder);

            // Use Task.WhenAll to manage parallel processing more effectively
            var tasks = imageFiles.Select(imageFile => Task.Run(() =>
            {
                try
                {
                    OptimizeImage(imageFile, outputFolder, maxWidth, quality);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error optimizing {imageFile}: {ex.Message}");
                }
            })).ToArray();

            Task.WhenAll(tasks).Wait();
        }

        private void OptimizeImage(string inputPath, string outputFolder, int maxWidth, long quality)
        {
            using (var image = Image.FromFile(inputPath))
            {
                int newWidth = image.Width > maxWidth ? maxWidth : image.Width;
                int newHeight = (int)((double)newWidth / image.Width * image.Height);

                using (var resizedImage = new Bitmap(image, new Size(newWidth, newHeight)))
                {
                    string outputPath = Path.Combine(outputFolder, Path.GetFileName(inputPath));
                    outputPath = GetUniqueFilePath(outputPath); // Avoid file overwriting

                    var codec = GetEncoderInfo(image.RawFormat.Equals(ImageFormat.Png) ? "image/png" : "image/jpeg");

                    if (codec != null)
                    {
                        var encoderParameters = new EncoderParameters(1)
                        {
                            Param = { [0] = new EncoderParameter(Encoder.Quality, quality) }
                        };
                        resizedImage.Save(outputPath, codec, encoderParameters);
                    }
                    else
                    {
                        resizedImage.Save(outputPath); // Fallback to default saving
                    }

                    Console.WriteLine($"Optimized: {outputPath}");
                }
            }
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.MimeType == mimeType);
        }

        // Ensures a unique file name to avoid overwriting
        private string GetUniqueFilePath(string filePath)
        {
            int count = 1;
            string outputPath = filePath;

            while (File.Exists(outputPath))
            {
                var directory = Path.GetDirectoryName(filePath);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                var extension = Path.GetExtension(filePath);
                outputPath = Path.Combine(directory, $"{fileNameWithoutExtension}_{count++}{extension}");
            }

            return outputPath;
        }
    }
}
