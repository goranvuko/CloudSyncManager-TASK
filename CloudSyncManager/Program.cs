using CloudSyncManager.Helpers;
using CloudSyncManager.Services;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Cloud Sync Manager");

ListAllCommands();

var localFileService = new LocalStorageFileService();
var syncService = new AzureCloudStorageSyncService();

while (true)
{
	Console.WriteLine($"Please Enter Command...");
	var command = Console.ReadLine();

	if (command == Commands.SEE_COMMANDS)
	{
		ListAllCommands();
		continue;
	}

	if (command == Commands.LIST_LOCAL)
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		ListLocalFiles(localFileService);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	if (command == Commands.LIST_LOCAL_AUDIO)
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		ListLocalAudioFiles(localFileService);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	if (command == Commands.LIST_LOCAL_VIDEO)
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		ListLocalVideoFiles(localFileService);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	if (command == Commands.LIST_LOCAL_PDF)
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		ListLocalPdfFiles(localFileService);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	if (command == Commands.LIST_CLOUD)
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		await ListCloudFiles(syncService);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	if (command == Commands.LIST_CLOUD_AUDIO)
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		await ListCloudAudioFiles(syncService);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	if (command == Commands.LIST_CLOUD_VIDEO)
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		await ListCloudVideoFiles(syncService);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	if (command == Commands.LIST_CLOUD_PDF)
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		await ListCloudPdfFiles(syncService);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	if (command.Contains(Commands.UPLOAD))
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		await SyncToCloud(syncService, command);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	// NEW NEW NEW
	if (command.Contains(Commands.DOWNLOAD))
	{
		Console.WriteLine($"----------------------------------------------------------------------------------");
		await Download(syncService, command);
		Console.WriteLine($"----------------------------------------------------------------------------------");
		continue;
	}

	Console.WriteLine($"Command not recognized...");
}

static void ListLocalFiles(LocalStorageFileService localFileService)
{
	Console.WriteLine("Command Recognized : Listing all local files...");
	var localFiles = localFileService.GetAllLocalFiles();
	localFiles.ToList().ForEach(file => Console.WriteLine(file.Name));
	if (!localFiles.Any()) Console.WriteLine("Found no files.");
}

static void ListLocalAudioFiles(LocalStorageFileService localFileService)
{
	Console.WriteLine("Command Recognized : Listing local audio files...");
	var localFiles = localFileService.GetLocalAudioFiles();
	localFiles.ToList().ForEach(file => Console.WriteLine(file.Name));
	if (!localFiles.Any()) Console.WriteLine("Found no files.");
}

static void ListLocalVideoFiles(LocalStorageFileService localFileService)
{
	Console.WriteLine("Command Recognized : Listing local video files...");
	var localFiles = localFileService.GetLocalVideoFiles();
	localFiles.ToList().ForEach(file => Console.WriteLine(file.Name));
	if (!localFiles.Any()) Console.WriteLine("Found no files.");
}

static void ListLocalPdfFiles(LocalStorageFileService localFileService)
{
	Console.WriteLine("Command Recognized : Listing local pdf files...");
	var localFiles = localFileService.GetLocalPdfFiles();
	localFiles.ToList().ForEach(file => Console.WriteLine(file.Name));
	if (!localFiles.Any()) Console.WriteLine("Found no files.");
}

static async Task SyncToCloud(AzureCloudStorageSyncService syncService, string? command)
{
	Console.WriteLine("Command Recognized : Sync file to cloud");

	var fileName = command.Split(" ").Last();
	Console.WriteLine($"Syncing file: {fileName} to cloud");
	await syncService.SyncFileToBlobStorage(fileName);
	Console.WriteLine($"Syncing file: {fileName} to cloud finished.");
}

static async Task ListCloudFiles(AzureCloudStorageSyncService syncService)
{
	Console.WriteLine("Command Recognized : List cloud files");
	var cloudFiles = await syncService.ListFiles();
	if (!cloudFiles.Any()) 
	{
		Console.WriteLine("Found no files");
		return;
	}

	Console.WriteLine("Cloud files : ");
	foreach (var cloudFile in cloudFiles)
	{
		Console.WriteLine(cloudFile.Name);
	}
}

static async Task ListCloudAudioFiles(AzureCloudStorageSyncService syncService)
{
	Console.WriteLine("Command Recognized : List cloud audio files");

	var cloudFiles = await syncService.ListAudioFiles();
	if (!cloudFiles.Any())
	{
		Console.WriteLine("Found no files");
		return;
	}

	Console.WriteLine("Cloud audio files : ");
	foreach (var cloudFile in cloudFiles)
	{
		Console.WriteLine(cloudFile.Name);
	}
}

static async Task ListCloudVideoFiles(AzureCloudStorageSyncService syncService)
{
	Console.WriteLine("Command Recognized : List cloud video files");
;
	var cloudFiles = await syncService.ListVideoFiles();
	if (!cloudFiles.Any())
	{
		Console.WriteLine("Found no files");
		return;
	}

	Console.WriteLine("Cloud video files : ");
	foreach (var cloudFile in cloudFiles)
	{
		Console.WriteLine(cloudFile.Name);
	}
}

static async Task ListCloudPdfFiles(AzureCloudStorageSyncService syncService)
{
	Console.WriteLine("Command Recognized : List cloud pdf files");
	var cloudFiles = await syncService.ListPdfFiles();
	if (!cloudFiles.Any())
	{
		Console.WriteLine("Found no files");
		return;
	}

	Console.WriteLine("Cloud pdf files : ");
	foreach (var cloudFile in cloudFiles)
	{
		Console.WriteLine(cloudFile.Name);
	}
}

static void ListAllCommands()
{
	Console.WriteLine("----------------------------------------------------------------------------------");
	Console.WriteLine("Commands: ");
	Console.WriteLine($"{Commands.LIST_LOCAL}                       -> lists all local files ");
	Console.WriteLine($"{Commands.LIST_CLOUD}                       -> lists all cloud files ");
	Console.WriteLine($"{Commands.LIST_LOCAL_AUDIO}                 -> lists all local audio files ");
	Console.WriteLine($"{Commands.LIST_LOCAL_VIDEO}                 -> lists all local video files ");
	Console.WriteLine($"{Commands.LIST_LOCAL_PDF}                   -> lists all local pdf files ");
	Console.WriteLine($"{Commands.LIST_CLOUD_AUDIO}                 -> lists all cloud audio files ");
	Console.WriteLine($"{Commands.LIST_CLOUD_VIDEO}                 -> lists all cloud video files ");
	Console.WriteLine($"{Commands.LIST_CLOUD_PDF}                   -> lists all cloud pdf files ");
	Console.WriteLine($"{Commands.UPLOAD} [fileName]		-> upload file to cloud");
	Console.WriteLine($"{Commands.DOWNLOAD} [fileName]              -> download file from cloud");
	Console.WriteLine($"----------------------------------------------------------------------------------");
}

static async Task Download(AzureCloudStorageSyncService syncService, string? command)
{
	Console.WriteLine("Command Recognized : Download file");

	var fileName = command.Split(" ").Last();
	Console.WriteLine($"Downloading file: {fileName} from cloud");
	await syncService.DownloadFileFromBlobStorage(fileName);
	Console.WriteLine($"Downloading file: {fileName} from cloud finished.");
}

