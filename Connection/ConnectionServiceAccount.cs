using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Security.Cryptography.X509Certificates;

namespace Google.Drive.Integration.Connection
{
    public class ConnectionServiceAccount
    {
        /// <summary>
        /// Creates the connection that will be used during the application
        /// </summary>
        /// <param name="applicationName">String with the Project Name created on the Cloud*</param>
        /// <param name="credentialFilePath">String with the path to the credential file of the service account*</param>
        /// <param name="emailServiceAccount">String with the service account's email. Required only when using .p12 file</param>
        /// <exception cref="Exception"></exception>
        public DriveService CreateConnection(string applicationName, string credentialFilePath, string? emailServiceAccount = null)
        {
            DriveService service = new DriveService();
            try
            {
                if (!File.Exists(credentialFilePath))
                    throw new Exception("File not found at the path: " + credentialFilePath);

                if (Path.GetExtension(credentialFilePath).ToLower() == ".json")
                {
                    GoogleCredential credential;

                    using (var stream = new FileStream(credentialFilePath, FileMode.Open, FileAccess.Read))
                    {
                        credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.Drive);
                    }

                    service = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = applicationName,
                    });
                }
                else if (Path.GetExtension(credentialFilePath).ToLower() == ".p12")
                {
                    if (string.IsNullOrEmpty(emailServiceAccount))
                        throw new Exception("emailServiceAccount is required to .p12 file.");

                    var certificate = new X509Certificate2(credentialFilePath, "notasecret", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                    var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(emailServiceAccount).FromCertificate(certificate));

                    // Create the  Drive service.
                    service = new DriveService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = applicationName
                    });
                }
                else
                {
                    throw new Exception("File type must be .json or .p12");
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error creating the DriveService. Exception Message: " + e.Message);
            }
            return service;
        }
    }
}
