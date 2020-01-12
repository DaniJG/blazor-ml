using System;
using System.IO;
using System.Threading.Tasks;
using BlazorInputFile;

namespace BlazorClient.Data
{
    public class SelectedImage
    {
        private IFileListEntry _file;
        public string Base64Image { get; private set; }
        public ImageClassificationResult ClassificationResult { get; set; }
        public string Name => _file.Name;
        public double UploadedPercentage => 100.0 * _file.Data.Position / _file.Size;

        public SelectedImage(IFileListEntry file)
        {
            _file = file;
        }

        public async Task<MemoryStream> Upload(Action OnDataRead)
        {
            if (_file.Data.Position > 0) throw new InvalidOperationException("Already uploaded");

            EventHandler eventHandler = (sender, eventArgs) => OnDataRead();
            _file.OnDataRead += eventHandler;

            // Download File contents into a memory stream, so we can later hand it over to the ML.NET service
            var fileStream = new MemoryStream();
            await _file.Data.CopyToAsync(fileStream);

            // Get a base64 so we can render an image preview
            Base64Image = Convert.ToBase64String(fileStream.ToArray());

            _file.OnDataRead -= eventHandler;
            return fileStream;
        }
    }
}