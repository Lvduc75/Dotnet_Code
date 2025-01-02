using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ConsoleUtils.ImageOptimizer
{
    public class ImageOptimizerWithSixLabors
    {
        public static void OptimizeImage(string inputPath, string outputPath, int maxWidth, int maxHeight, int quality)
        {
            // Load the image
            using (Image image = Image.Load(inputPath))
            {
                // 1. Resize image to fit within specified dimensions while maintaining aspect ratio
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxWidth, maxHeight)
                }));

                // 2. Remove metadata to save space
                image.Metadata.ExifProfile = null;

                // 3. Save the image in a modern format (e.g., WebP or JPEG)
                // You can choose between WebP or JPEG based on your needs
                var extension = Path.GetExtension(outputPath)?.ToLower();
                if (extension == ".webp")
                {
                    image.Save(outputPath, new WebpEncoder { Quality = quality });
                }
                else if (extension == ".jpg" || extension == ".jpeg")
                {
                    image.Save(outputPath, new JpegEncoder { Quality = quality });
                }
                else
                {
                    throw new NotSupportedException("Unsupported output format. Use .webp or .jpeg");
                }
            }
        }
    }
}
