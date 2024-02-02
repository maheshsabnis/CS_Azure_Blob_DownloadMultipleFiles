using Azure.Storage.Blobs;

namespace CS_Azure_Blob_DownloadMultipleFiles
{
    public class BLOBDownloader
    {
        private readonly BlobServiceClient _blobClient;

        public BLOBDownloader(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task DownloadMultipleFilesAsync(string contName)
        {
            // Get the Path of the Project Folder 
            string? startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent?.Parent?.FullName;
            // Create a BlobClient Object based on ContainerName
            var blobConClient = _blobClient.GetBlobContainerClient(contName);
            // Call the listing operation and return pages
            var blobItemPages = blobConClient.GetBlobsAsync()
                    .AsPages();
            // SemaphoreSlim with 8 THreads
            var semaphoreSlim = new SemaphoreSlim(8);
            var tasksBlobDownload = new List<Task>();
            // Process each BLOB
            await foreach (var blobItemPage in blobItemPages)
            {
                foreach (var blob in blobItemPage.Values)
                {
                    // blocks the current thread until it can enter the System.Threading.SemaphoreSlim.
                    await semaphoreSlim.WaitAsync();

                    tasksBlobDownload.Add(Task.Run(async () =>
                    {
                        try
                        {
                            // Get the BLOB Name
                            var client = blobConClient.GetBlobClient(blob.Name);
                            // Create a BLOB in the DownloadedFiles folder
                            using var file = File.Create($"{Path.Combine(startupPath, "DownloadedFiles")}\\{blob.Name}");
                            await client.DownloadToAsync(file);
                        }
                        finally
                        {
                            semaphoreSlim.Release();
                        }
                    }));
                }

                await Task.WhenAll(tasksBlobDownload);
                tasksBlobDownload.Clear();
            }
        }
    }
}
