// See https://aka.ms/new-console-template for more information
using Azure.Storage.Blobs;
using CS_Azure_Blob_DownloadMultipleFiles;

Console.WriteLine("Download Multiple Blobs");

var connectionString = "CONNSTR";
var blobClient = new BlobServiceClient(connectionString);

var download = new BLOBDownloader(blobClient);

await download.DownloadMultipleFilesAsync("images");

Console.WriteLine("Completed");
Console.ReadLine(); 
