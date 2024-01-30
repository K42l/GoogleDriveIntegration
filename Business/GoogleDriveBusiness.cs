using Google.Apis.Drive.v3;

namespace Google.Drive.Integration.Business
{
    public class GoogleDriveBusiness
    {

        /// <summary>
        /// Method to get the Mime Type through the file extension.<br/>
        /// Work only on windows.
        /// </summary>
        /// <param name="fileName">Full filename to be verifiyed*</param>
        /// <returns>String with the Mime Type</returns>
        public string GetMimeType(string fileName)
        {
            try
            {
                string mimeType = "application/unknown";
                string ext = System.IO.Path.GetExtension(fileName).ToLower();
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
                return mimeType;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method to creat a query string.<br/>
        /// The "service" created by the connection, must be sent on the parameter "listRequest"<br />
        /// Default request type is "file". If the requisition is to a folder, send requestType as "folder"
        /// </summary>
        /// <param name="listRequest">Created Connection service*</param>
        /// <param name="requestType">Request type - folder/file*</param>
        /// <param name="driveId">Drive's ID*</param>
        /// <param name="containsName">Array with the names that the file/folder can contain.</param>
        /// <param name="name">File/Folder exact name</param>
        /// <param name="parentId">File/Folder parent id</param>
        /// <returns>The FilesResource.ListRequest with the query.</returns>
        public FilesResource.ListRequest SetQuerysRequest(FilesResource.ListRequest listRequest, 
                                                          string requestType, 
                                                          string driveId, 
                                                          string[]? containsName = null, 
                                                          string? name = null, 
                                                          string? parentId = null)
        {
            listRequest.Q = $"mimeType!='application/vnd.google-apps.folder'";
            if(requestType == "folder")
                listRequest.Q = $"mimeType='application/vnd.google-apps.folder'";
            if (!String.IsNullOrEmpty(parentId))
                listRequest.Q += $" and '{parentId}' in parents";
            if (!String.IsNullOrEmpty(name))
                listRequest.Q += $" and name='{name}'";
            if (containsName != null )
                foreach (var item in containsName)
                    listRequest.Q += $" and fullText contains '{item}'";
            listRequest.Q += " and trashed=false";
            listRequest.Fields = "nextPageToken, files(id, name, size, mimeType, parents)";
            listRequest.IncludeItemsFromAllDrives = true;
            listRequest.Corpora = "drive";
            listRequest.SupportsAllDrives = true;
            listRequest.DriveId = driveId;

            return listRequest;
        }
    }
}
