using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CloudSyncManager.Services
{
	public class AzureCloudStorageSyncService
	{
		private readonly string _storageConnectionString = "UseDevelopmentStorage=true";
		private readonly string _localStoragePath = @"C:\Users\Goran Vukojičić\Desktop\cloudSyncManager-TASK\LOCAL_STORAGE";
		private readonly string _videoBlobContainerName = "video-container";
		private readonly string _audioBlobContainerName = "audio-container";
		private readonly string _pdfBlobContainerName = "pdf-container";
		
		private List<string> _pdfSupportedExtensions = new List<string> { ".pdf" };
		private List<string> _audioSupportedExtensions = new List<string> { ".mp3" };
		private List<string> _videoSupportedExtensions = new List<string> { ".mp4", ".avi" };


		private Dictionary<string, BlobContainerClient> _extensionBlobContainerDictionary = new Dictionary<string, BlobContainerClient>();

		private BlobStorageService _blobStorageService;

		private BlobContainerClient _videoBlobContainerClient;
		private BlobContainerClient _audioBlobContainerClient;
		private BlobContainerClient _pdfBlobContainerClient;

		public AzureCloudStorageSyncService()
		{
			SetupBlobStorageContainerClients();
			SetupExtensionToBlobContainerMatching();
		}

		public async Task<List<BlobItem>> ListFiles()
		{
			var containerClients = new List<BlobContainerClient> { _videoBlobContainerClient, _audioBlobContainerClient,_pdfBlobContainerClient };
			var blobs = new List<BlobItem>();

			foreach (var containerClient in containerClients) 
			{
				var containerBlobs = await GetBlobsFromContainer(containerClient);
				blobs.AddRange(containerBlobs);
			}
			
			return blobs;
		}

		public async Task<List<BlobItem>> ListAudioFiles()
		{
			return await GetBlobsFromContainer(_audioBlobContainerClient);
		}

		public async Task<List<BlobItem>> ListVideoFiles()
		{
			return await GetBlobsFromContainer(_videoBlobContainerClient);
		}

		public async Task<List<BlobItem>> ListPdfFiles()
		{
			return await GetBlobsFromContainer(_pdfBlobContainerClient);
		}

		public async Task<List<BlobItem>> GetBlobsFromContainer(BlobContainerClient containerClient) 
		{
			var blobs = new List<BlobItem>();
			var resultSegment = containerClient.GetBlobsAsync().AsPages(default, 5);
			await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
			{
				foreach (BlobItem blobItem in blobPage.Values)
				{
					blobs.Add(blobItem);
				}
			}

			return blobs;
		}

		public async Task SyncFileToBlobStorage(string localFilePath)
		{
			var fileExtension = Path.GetExtension(localFilePath);
			
			var blobContainerClient = _extensionBlobContainerDictionary[fileExtension];

			await UploadFileToBlobStorage(blobContainerClient, localFilePath);
		}

		
		public async Task DownloadFileFromBlobStorage(string fileName)
		{
			var fileExtension = Path.GetExtension(fileName);

			var blobContainerClient = _extensionBlobContainerDictionary[fileExtension];

			await DownloadFileFromBlobStorage(blobContainerClient, fileName);
		}

		private async Task UploadFileToBlobStorage(BlobContainerClient containerClient, string fileName)
		{
			var localFilePath = Path.Combine(_localStoragePath, fileName);

			var fileExists = File.Exists(localFilePath);

			if (!fileExists)
			{
				Console.WriteLine($"File {localFilePath} does not exist !!!");
				return;
			}
			BlobClient blobClient = containerClient.GetBlobClient(fileName);

			await blobClient.UploadAsync(localFilePath, true);
		}

		
		private async Task DownloadFileFromBlobStorage(BlobContainerClient containerClient, string fileName)
		{
			BlobClient blobClient = containerClient.GetBlobClient(fileName);

			if (!blobClient.Exists())
			{
				Console.WriteLine($"File {fileName} does not exist on cloud!!!");
				return;
			}

			var localFilePath = Path.Combine(_localStoragePath, fileName);
			FileStream fileStream = File.OpenWrite(localFilePath);
			await blobClient.DownloadToAsync(fileStream);
			fileStream.Close();
		}


		private void SetupExtensionToBlobContainerMatching()
		{
			_extensionBlobContainerDictionary = new Dictionary<string, BlobContainerClient>();
			_videoSupportedExtensions.ForEach(ext => { _extensionBlobContainerDictionary.Add(ext, _videoBlobContainerClient); });
			_audioSupportedExtensions.ForEach(ext => { _extensionBlobContainerDictionary.Add(ext, _audioBlobContainerClient); });
			_pdfSupportedExtensions.ForEach(ext => { _extensionBlobContainerDictionary.Add(ext, _pdfBlobContainerClient); });
		}

		private void SetupBlobStorageContainerClients()
		{
			_blobStorageService = new BlobStorageService(_storageConnectionString);

			_videoBlobContainerClient = _blobStorageService.GetOrCreateBlobContainerClient(_videoBlobContainerName);
			_audioBlobContainerClient = _blobStorageService.GetOrCreateBlobContainerClient(_audioBlobContainerName);
			_pdfBlobContainerClient = _blobStorageService.GetOrCreateBlobContainerClient(_pdfBlobContainerName);
		}
	}
}