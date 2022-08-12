using Azure.Storage.Blobs;

namespace CloudSyncManager.Services
{
	public class BlobStorageService
	{
		protected string _storageConnectionString;
		protected readonly BlobServiceClient _serviceClient;

		public BlobStorageService(string storageConnectionString)
		{
			_storageConnectionString = storageConnectionString ?? "UseDevelopmentStorage=true";
			_serviceClient = new BlobServiceClient(_storageConnectionString);
		}

		public BlobContainerClient GetOrCreateBlobContainerClient(string containerName)
		{
			var containerClient = _serviceClient.GetBlobContainerClient(containerName);
			containerClient.CreateIfNotExists();
			return containerClient;
		}
	}
}