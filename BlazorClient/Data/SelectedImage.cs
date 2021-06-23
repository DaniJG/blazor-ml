using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorClient.Data
{
    public class SelectedImage
    {
        private IBrowserFile _file;
        private Stream _fileReadStream;
        public string Base64Image { get; private set; }
        public ImageClassificationResult ClassificationResult { get; set; }
        public string Name => _file.Name;
        public double UploadedPercentage => 100.0 * _fileReadStream.Position / _file.Size;

        public SelectedImage(IBrowserFile file)
        {
            _file = file;
        }

        public async Task<MemoryStream> Upload(Action OnDataRead)
        {
            if (_fileReadStream is not null) throw new InvalidOperationException("Already uploaded");

            _fileReadStream = _file.OpenReadStream(10 * 1024 * 1024 /* 10MB */);
            var memoryStream = new MemoryStream();

            // Download File contents into a memory stream, so we can later hand it over to the ML.NET service
            // Note instead of using "_fileReadStream.CopyToAsync(memoryStream)", we have the loop below so we can call the callback and update the progress bar
            var buffer = new byte[80 * 1024];
            while ((await _fileReadStream.ReadAsync(buffer)) != 0)
            {
                await memoryStream.WriteAsync(buffer);
                OnDataRead();
            }

            // Get a base64 so we can render an image preview
            Base64Image = Convert.ToBase64String(memoryStream.ToArray());

            return memoryStream;
        }
    }
}