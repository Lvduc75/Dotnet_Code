using ConsoleUtils.ImageOptimizer;

namespace ConsoleUtils
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Specify the folder path where the images are located
            string folderPath = @"D:\TaiLieu\Net\Sample\After_Picture"; // Change this to your image folder path

            // Create an instance of ImageProcessor
            var imageProcessor = new ImageProcessor();

            // Call the method to optimize images in the specified folder
            imageProcessor.OptimizeImagesInFolder(folderPath, maxWidth: 1000, quality: 75L);

            Console.WriteLine("Image optimization complete.");
        }
    }
}
