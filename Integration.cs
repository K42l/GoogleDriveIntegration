using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Download;
using Google.Drive.Integration.Business;
using Google.Drive.Integration.Connection;
using Google.Drive.Integration.Interface;

namespace Google.Drive.Integration
{
    public class Integration : IIntegration
    {
        private DriveService service { get; set; }
        private GoogleDriveBusiness googleDriveBusiness = new GoogleDriveBusiness();
        public Integration(string applicationName, string credentialFilePath, string? emailServiceAccount = null)
        {
            ConnectionServiceAccount connectionServiceAccount= new ConnectionServiceAccount();
            service = connectionServiceAccount.CreateConnection(applicationName, credentialFilePath, emailServiceAccount);
        }

        public async Task<Google.Apis.Drive.v3.Data.File> CreateFolderAsync(string driveId, string folderName, string? parentId = null)
        {
            try
            {
                if (!String.IsNullOrEmpty(folderName))
                {
                    var driveFolder = new Google.Apis.Drive.v3.Data.File()
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
                    throw new Exception("folderName nulo ou vazio");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Google.Apis.Drive.v3.Data.File> UploadFileAsync(string driveId, Stream file, string fileName, string? parentId = null, string? fileDescription = null)
        {
            try
            {
                string fileMime = googleDriveBusiness.GetMimeType(fileName);
                var driveFile = new Google.Apis.Drive.v3.Data.File()
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

        public async Task<List<Google.Apis.Drive.v3.Data.File>> GetFilesAsync(string driveId, string[]? containsName = null, string? name = null,string? parentId = null)
        {
            try
            {
                var list = service.Files.List();
                var requestType = "file";
                var fileList = googleDriveBusiness.SetQuerysRequest(list, requestType, driveId, containsName, name, parentId);

                var result = new List<Google.Apis.Drive.v3.Data.File>();
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

        public async Task<List<Google.Apis.Drive.v3.Data.File>> GetFoldersAsync(string driveId, string[]? containsName = null, string? name = null,string? parentId = null)
        {
            try
            {
                var list = service.Files.List();
                var requestType = "folder";
                var folderList = googleDriveBusiness.SetQuerysRequest(list, requestType, driveId, containsName, name, parentId);

                var result = new List<Google.Apis.Drive.v3.Data.File>();
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
                var response = await request.ExecuteAsync();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> DriveDownloadFileAsync(string fileId)
        {
            try { 
                var request = service.Files.Get(fileId);
                request.SupportsAllDrives = true;
                //string base64String;

                using (MemoryStream stream = new MemoryStream())
                {
                    // Add a handler which will be notified on progress changes.
                    // It will notify on each chunk download and when the
                    // download is completed or failed.
                    request.MediaDownloader.ProgressChanged +=
                        progress =>
                        {
                            if (progress.Status == DownloadStatus.Failed)
                            {
                                throw new Exception("Falha no download");
                            }
                        };
                    await request.DownloadAsync(stream);

                    var fileBytes = stream.ToArray();
                    //base64String = Convert.ToBase64String(fileBytes, 0, fileBytes.Length);
                    return fileBytes;
                }
                //return base64String;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
