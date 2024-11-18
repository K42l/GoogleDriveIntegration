using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using Google.Apis.Download;
using Google.Drive.Integration.Business;
using Google.Drive.Integration.Connection;
using Google.Drive.Integration.Interface;

namespace Google.Drive.Integration
{
    public class Integration : IIntegration
    {
        private DriveService service { get; set; }
        public Integration(string applicationName, string credentialFilePath, string? emailServiceAccount = null)
        {
            service = ConnectionServiceAccount.CreateConnection(applicationName, credentialFilePath, emailServiceAccount);
        }

        public async Task<GoogleFile> CreateFolderAsync(string driveId, string folderName, string? parentId = null)
        {
            try
            {
                if (!String.IsNullOrEmpty(folderName))
                {
                    GoogleFile driveFolder = new()
                    {
                        DriveId = driveId,
                        Name = folderName,
                        MimeType = "application/vnd.google-apps.folder"
                    };

                    if (!string.IsNullOrEmpty(parentId))
                    {
                        driveFolder.Parents = new string[] { parentId };
                    }

                    var command = service.Files.Create(driveFolder);
                    command.SupportsAllDrives = true;

                    var file = await command.ExecuteAsync();
                    return file;
                }
                else
                {
                    throw new Exception("Param folderName is null or empty");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GoogleFile> UploadFileAsync(string driveId, Stream file, string fileName, string? parentId = null, string? fileDescription = null)
        {
            try
            {
                string fileMime = GoogleDriveBusiness.GetMimeType(fileName);
                GoogleFile driveFile = new()
                {
                    DriveId = driveId,
                    Name = fileName,
                    Description = fileDescription,
                    MimeType = fileMime

                };

                if (!String.IsNullOrEmpty(parentId))
                {
                    driveFile.Parents = new string[] { parentId };
                } 

                var request = service.Files.Create(driveFile, file, fileMime);
                request.Fields = "id";
                request.SupportsAllDrives = true;

                var response = await request.UploadAsync();
                if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                    throw response.Exception;

                return request.ResponseBody;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<GoogleFile>> ListFilesAsync(string driveId, string[]? containsName = null, string? name = null,string? parentId = null)
        {
            try
            {
                var list = service.Files.List();
                var requestType = "file";
                var fileList = GoogleDriveBusiness.SetQuerysRequest(list, requestType, driveId, containsName, name, parentId);

                var result = new List<GoogleFile>();
                string pageToken = null;
                do
                {
                    fileList.PageToken = pageToken;
                    var filesResult = await fileList.ExecuteAsync();
                    var files = filesResult.Files;
                    pageToken = filesResult.NextPageToken;
                    result.AddRange(files);
                }
                while (pageToken != null);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<GoogleFile>> ListFoldersAsync(string driveId, string[]? containsName = null, string? name = null,string? parentId = null)
        {
            try
            {
                var list = service.Files.List();
                var requestType = "folder";
                var folderList = GoogleDriveBusiness.SetQuerysRequest(list, requestType, driveId, containsName, name, parentId);

                var result = new List<GoogleFile>();
                string? pageToken = null;
                do
                {
                    folderList.PageToken = pageToken;
                    var foldersResult = await folderList.ExecuteAsync();
                    var folders = foldersResult.Files;
                    pageToken = foldersResult.NextPageToken;
                    result.AddRange(folders);
                }
                while (pageToken != null);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TeamDriveList> GetAllTeamDrivesAsync()
        {
            try
            {
                var request = service.Teamdrives.List();
                request.PageSize = 100;
                return await request.ExecuteAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> DownloadFileAsync(string fileId)
        {
            try { 
                var request = service.Files.Get(fileId);
                request.SupportsAllDrives = true;

                using (MemoryStream stream = new MemoryStream())
                {
                    // Add a handler which will be notified on progress changes.
                    // It will notify if download fail.
                    request.MediaDownloader.ProgressChanged +=
                        progress =>
                        {
                            if (progress.Status == DownloadStatus.Failed)
                            {
                                throw new Exception("Download failed.");
                            }
                        };
                    await request.DownloadAsync(stream);

                    var fileBytes = stream.ToArray();
                    return fileBytes;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GoogleFile> GetFileInfoAsync(string fileId)
        {
            try
            {
                var request = service.Files.Get(fileId);
                request.SupportsAllDrives = true;

                return await request.ExecuteAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
