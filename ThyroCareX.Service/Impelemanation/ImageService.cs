using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Service.Abstarct;
using static System.Net.Mime.MediaTypeNames;
using SkiaSharp;


namespace ThyroCareX.Service.Impelemanation
{
    public class ImageService:IImageService
    {
        private readonly string _rootPath;

        public ImageService(string rootPath)
        {
            _rootPath = Path.Combine(rootPath, "uploads", "doctors");

            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string originalFileName)
        {
            if (fileStream == null || fileStream.Length == 0)
                return null;

            // توليد اسم جديد
            var fileName = $"{Guid.NewGuid():N}.webp";
            var filePath = Path.Combine(_rootPath, fileName);

            // استخدم SkiaSharp لتحويل الصورة مباشرة إلى WebP
            using var skBitmap = SKBitmap.Decode(fileStream);
            if (skBitmap == null)
                return null;

            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Webp, 80); // 80% جودة عالية مع ضغط ممتاز

            // كتابة مباشرة للملف باستخدام FileStream
            using var output = File.OpenWrite(filePath);
             data.SaveTo(output);

            // إعادة المسار النسبي
            return Path.Combine("uploads", "doctors", fileName).Replace("\\", "/");
        }
        public void DeleteImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            var fullPath = Path.Combine(_rootPath, Path.GetFileName(path));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

    }
}
