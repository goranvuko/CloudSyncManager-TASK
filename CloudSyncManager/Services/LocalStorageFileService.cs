namespace CloudSyncManager.Services
{
	public class LocalStorageFileService
	{
		private readonly string _localStoragePath = @"C:\Users\Goran Vukojičić\Desktop\cloudSyncManager-TASK\LOCAL_STORAGE";
		private List<string> _pdfSupportedExtensions = new List<string> { ".pdf" };
		private List<string> _audioSupportedExtensions = new List<string> { ".mp3" };
		private List<string> _videoSupportedExtensions = new List<string> { ".mp4", ".avi" };

		public LocalStorageFileService()
		{
		}

		public FileInfo[] GetAllLocalFiles()
		{
			DirectoryInfo directory = new DirectoryInfo(_localStoragePath);
			FileInfo[] files = directory.GetFiles();
			return files;
		}

		public FileInfo[] GetLocalAudioFiles()
		{
			DirectoryInfo directory = new DirectoryInfo(_localStoragePath);
			FileInfo[] files = directory.GetFiles();
			return files.Where(x => _audioSupportedExtensions.Contains(Path.GetExtension(x.Name))).ToList().ToArray();
		}

		public FileInfo[] GetLocalVideoFiles()
		{
			DirectoryInfo directory = new DirectoryInfo(_localStoragePath);
			FileInfo[] files = directory.GetFiles();
			return files.Where(x => _videoSupportedExtensions.Contains(Path.GetExtension(x.Name))).ToList().ToArray();
		}

		public FileInfo[] GetLocalPdfFiles()
		{
			DirectoryInfo directory = new DirectoryInfo(_localStoragePath);
			FileInfo[] files = directory.GetFiles();
			return files.Where(x => _pdfSupportedExtensions.Contains(Path.GetExtension(x.Name))).ToList().ToArray();
		}

	}
}