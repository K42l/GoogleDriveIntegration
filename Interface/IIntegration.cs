using Google.Apis.Drive.v3.Data;

namespace Google.Drive.Integration.Interface
{ 
    public interface IIntegration
    {
        /// <summary>
        /// Create a folder on the given Drive Id <br/>
        /// If the 'parentId' is null, the folder will be created on the drive's root
        /// </summary>
        /// <param name="driveId">Drive Id where the folder will be created</param>
        /// <param name="folderName">Folder's name</param>
        /// <param name="parentId">Parent Id</param>
        /// <returns>Details of the created folder</returns>
        Task<Google.Apis.Drive.v3.Data.File> CreateFolderAsync(string driveId, string folderName, string? parentId = null);

        /// <summary>
        /// Upload the file Stream file<br/>
        /// If the 'parentId' is null, o file will be created on the drive's root
        /// </summary>
        /// <param name="driveId">Drive ID where the file will be sent.</param>
        /// <param name="file">File's Stream</param>
        /// <param name="fileName">File's Name</param>
        /// <param name="parentId">Folder's Id where the file will be sent to</param>
        /// <param name="fileDescription">File description</param>
        /// <returns>
        /// The details of the file that was sent
        /// </returns>
        Task<Google.Apis.Drive.v3.Data.File> UploadFileAsync(string driveId, Stream file, string fileName, string? parentId = null, string? fileDescription = null);

        /// <summary>
        /// Search files on the given Drive<br/>
        /// </summary>
        /// <param name="driveId">Drive's Id where the files will be searched</param>
        /// <param name="name">Files Name</param>
        /// <param name="containsName">Files that contains the string on their names</param>
        /// <param name="parentId">Folder ID where the files will be searched. If is null, the files will be searched on the entire drive.</param>
        /// <returns>
        /// If 'containsName' and 'name' are null, will return all the files<br/>
        /// If 'parentId' is null, will return all the files that match the others params.<br/>
        /// </returns>
        Task<List<Google.Apis.Drive.v3.Data.File>> GetFilesAsync(string driveId, string[]? containsName = null, string? name = null, string? parentId = null);

        /// <summary>
        /// Search folders on the drive<br/>
        /// </summary>
        /// <param name="driveId">Drive's Id where the folders will be searched</param>
        /// <param name="name">Folder's name</param>
        /// <param name="containsName">Folders that contain the name</param>
        /// <param name="parentId">Parent Id where the folders should be searched. If is null, will search the entire drive</param>
        /// <returns>
        /// If 'containsName' and 'name' are null, will return all the files<br/>
        /// Se a 'parentId' não for informado, trará todas as pastas do drive.<br/>
        /// If 'parentId' is null, will return all folders on the drive that match the others params
        /// </returns>
        Task<List<Google.Apis.Drive.v3.Data.File>> GetFoldersAsync(string driveId, string[]? containsName = null, string? name = null, string? parentId = null);

        /// <summary>
        /// Search all drives that are been shared with the service account being used by the application
        /// </summary>
        /// <returns>
        /// All the drives details
        /// </returns>
        Task<TeamDriveList> GetAllTeamDrivesAsync();

        /// <summary>
        /// Download a file from the drive
        /// </summary>
        /// <param name="fileId">Files's ID to be downloaded</param>
        /// <returns>
        /// The file in a byte array
        /// </returns>
        Task<byte[]> DriveDownloadFileAsync(string fileId);
    }
}
