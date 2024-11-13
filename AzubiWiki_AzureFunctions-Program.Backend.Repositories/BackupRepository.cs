using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
namespace AzubiWiki_AzureFunctions_Program.Backend.Repositories
{
    public class BackupRepository : IBackupService
    {
        private readonly IFileHandler _fileHandler;
        private readonly ILogger _logger;
        public BackupRepository(IFileHandler fileHandler, ILogger logger)
        {
            _fileHandler = fileHandler;
            _logger = logger;
        }

        string blobCarContainerName = Environment.GetEnvironmentVariable("BlobCarContainerName");
        string blobGarageContainerName = Environment.GetEnvironmentVariable("BlobGarageContainerName");
        string connectionString = Environment.GetEnvironmentVariable("ConnectionString");


        string CarFilePath = Environment.GetEnvironmentVariable("CarFile");
        string GarageFilePath = Environment.GetEnvironmentVariable("GarageFile");

        public async Task BackupStorage()
        {
            BlobServiceClient blobClientService = new BlobServiceClient(connectionString);


            BlobContainerClient carContainerClient = blobClientService.GetBlobContainerClient(blobCarContainerName);
            await carContainerClient.CreateIfNotExistsAsync();
            string jsonCarContent = await _fileHandler.ReadAllTextAsync(CarFilePath);

            if (!String.IsNullOrWhiteSpace(jsonCarContent))
            {
                BlobClient blobClient = carContainerClient.GetBlobClient($"{DateTime.UtcNow:yyyyMMddHHmmss} car-backup");

                using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonCarContent)))
                {
                    await blobClient.UploadAsync(stream);

                }
            }
            else
            {
                _logger.LogInformation("[CAUTION: Car Database Backup Was NOT Made!]");
            }


            BlobContainerClient garageContainerClient = blobClientService.GetBlobContainerClient(blobGarageContainerName);
            await garageContainerClient.CreateIfNotExistsAsync();
            string jsonGarageContent = await _fileHandler.ReadAllTextAsync(GarageFilePath);

            if (!String.IsNullOrWhiteSpace(jsonGarageContent))
            {
                BlobClient blobClient = garageContainerClient.GetBlobClient($"{DateTime.UtcNow:yyyyMMddHHmmss} garage-backup");

                using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonGarageContent)))
                {
                    await blobClient.UploadAsync(stream);
                }

            }
            else
            {
                _logger.LogInformation("[CAUTION: Garage Database Backup Was NOT Saved!]");
            }
        }

        public async Task RestoreFromBackup()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);


            BlobContainerClient carContainerClient = blobServiceClient.GetBlobContainerClient(blobCarContainerName);

            List<BlobItem> carBackups = new List<BlobItem>();
            await foreach (BlobItem backup in carContainerClient.GetBlobsAsync())
            {
                carBackups.Add(backup);
            }

            if (carBackups != null && carBackups.Count > 0)
            {
                BlobItem carBackup = carBackups.OrderByDescending(x => x.Name).FirstOrDefault();

                BlobClient blobClient = carContainerClient.GetBlobClient(carBackup.Name);

                if (await blobClient.ExistsAsync())
                {
                    BlobDownloadInfo contents = await blobClient.DownloadAsync();

                    string jsonContent;
                    using (StreamReader reader = new StreamReader(contents.Content))
                    {
                        jsonContent = await reader.ReadToEndAsync();
                    }

                    if (!String.IsNullOrWhiteSpace(jsonContent))
                    {
                        await _fileHandler.WriteAllTextAsync(CarFilePath, jsonContent);
                    }
                    else
                    {
                        throw new BackupIsEmptyException(carBackup.Name);
                    }
                }
                else
                {
                    throw new NoBackupsFoundException("Car");
                }
            }
            else
            {
                _logger.LogInformation("[CAUTION: Code Skipped The Process Of Retrieving The Backup Of The Car Json Database Because It Was The Blob ]");
            }




            BlobContainerClient garageContainerClient = blobServiceClient.GetBlobContainerClient(blobGarageContainerName);

            List<BlobItem> garageBackups = new List<BlobItem>();
            await foreach (BlobItem backup in garageContainerClient.GetBlobsAsync())
            {
                garageBackups.Add(backup);
            }

            if (garageBackups != null && garageBackups.Count > 0)
            {
                BlobItem garageBackup = garageBackups.OrderByDescending(g => g.Name).FirstOrDefault();

                BlobClient blobClient = garageContainerClient.GetBlobClient(garageBackup.Name);

                if (await blobClient.ExistsAsync())
                {
                    BlobDownloadInfo contents = await blobClient.DownloadAsync();

                    string jsonContent;
                    using (StreamReader reader = new StreamReader(contents.Content))
                    {
                        jsonContent = await reader.ReadToEndAsync();
                    }

                    if (!String.IsNullOrWhiteSpace(jsonContent))
                    {
                        await _fileHandler.WriteAllTextAsync(GarageFilePath, jsonContent);
                    }
                    else
                    {
                        throw new BackupIsEmptyException(garageBackup.Name);
                    }
                }
                else
                {
                    throw new NoBackupsFoundException("Garage");
                }
            }
            else
            {
                _logger.LogInformation("[CAUTION: Code Skipped The Process Of Retrieving The Backup Of The Car Json Database Because It Was The Blob Was Not Found]");
            }

        }

        public Task DeleteBackup()
        {
            return null;
        }
    }
}
