using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThyroCareX.Service.Abstarct
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Stream fileStream, string originalFileName);
        void DeleteImage(string path);
    }
}
