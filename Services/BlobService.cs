using Azure.Storage.Blobs;

namespace EventEase.Services
{
    public class BlobService
    {
        private readonly IConfiguration _configuration;

        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string connectionString =
                _configuration["AzureBlobStorage"];

            string containerName = "venue-images";

            BlobContainerClient container =
                new BlobContainerClient(
                    connectionString,
                    containerName);

            await container.CreateIfNotExistsAsync();

            BlobClient blob =
                container.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, true);
            }

            return blob.Uri.ToString();
        }
    }
}