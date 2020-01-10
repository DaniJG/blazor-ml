using System;
using System.IO;
using System.Threading.Tasks;
using BlazorInputFile;

namespace BlazorClient.Data
{
    public class SelectedImage
    {
        public IFileListEntry File { get; set; }
        public string Base64Image { get; set; }
        public ImageClassificationResult ClassificationResult { get; set; }
        public double UploadedPercentage => 100.0 * File.Data.Position / File.Size;

        public async Task<MemoryStream> Upload(Action OnDataRead)
        {
            if (File.Data.Position > 0) throw new InvalidOperationException("Already uploaded");

            EventHandler eventHandler = (sender, eventArgs) => OnDataRead();
            File.OnDataRead += eventHandler;

            // Download File contents into a memory stream, so we can later hand it over to the ML.NET service
            var fileStream = new MemoryStream();
            await File.Data.CopyToAsync(fileStream);

            // Get a base64 so we can render an image preview
            Base64Image = Convert.ToBase64String(fileStream.ToArray());

            File.OnDataRead -= eventHandler;
            return fileStream;
        }
    }
}