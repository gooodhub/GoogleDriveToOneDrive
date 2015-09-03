using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Http;
using Google.Apis.Services;
using File = Google.Apis.Drive.v2.Data.File;

namespace DriveExport
{

    public class Dossier
    {
        public string Id { get; set; }
        public string Nom { get; set; }
    }

    public static class GoogleDriveService
    {
        static int nbFiles = 0;
        static int nbCopiedFiles = 0;
        public static List<Dossier> DossiersTemp;

        public static void StartFileAndFolders()
        {
            DriveService service = Instanciate(ConfigurationManager.AppSettings["ownerToFilter"]);

            List<File> docs = RetrieveAllFilesAndFolders(service, ConfigurationManager.AppSettings["folderParentToRetrieve"], ConfigurationManager.AppSettings["ownerToFilter"]);
            Console.WriteLine(docs);
            Console.Read();
        }

        public static DriveService Instanciate(String ownerToFilter)
        {
            IConfigurableHttpClientInitializer cred = Authenticate(ownerToFilter);

            DriveService service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred,
                ApplicationName = ConfigurationManager.AppSettings["applicationName"],
            });

            return service;
        }

        public static bool AllFilesArePresent()
        {
            IConfigurableHttpClientInitializer cred = Authenticate(ConfigurationManager.AppSettings["ownerToFilter"]);

            DriveService service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred,
                ApplicationName = "GoogleDriveToOneDrive",
            });

            List<File> result = new List<File>();
            FilesResource.ListRequest request = service.Files.List();
            request.MaxResults = 1000;
            request.Q = ("'pl.sgard@clt-services.com' IN owners");
            int i = 0;
            do
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        FileList files = request.Execute();
                        String ownerToFilter = "pl.sgard@clt-services.com";

                        result.AddRange(files.Items);
                        request.PageToken = files.NextPageToken;
                        i++;
                        foreach (File file in files.Items)
                        {
                            if (file.MimeType != "application/vnd.google-apps.folder")
                            {
                                if (file.MimeType != "application/vnd.google-apps.form")
                                {
                                    DirectoryInfo root = new DirectoryInfo(@"F:\GoogleDrive\" + ownerToFilter);

                                    FileInfo[] listfiles = root.GetFiles(file.Title + ".*");
                                    if (listfiles.Length == 0)
                                    {
                                        Console.WriteLine(file.Title);
                                        return false;
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken) || i < 11);
            return true;
        }

        private static IConfigurableHttpClientInitializer Authenticate(String ownerToFilter)
        {
            X509Certificate2 cert = new X509Certificate2(ConfigurationManager.AppSettings["accountServiceP12"], "notasecret", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            ServiceAccountCredential cred = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(ConfigurationManager.AppSettings["accountServiceName"])
                {
                    User = ownerToFilter,
                    Scopes = new[] { DriveService.Scope.Drive },
                }.FromCertificate(cert));
            return cred;
        }

        public static List<File> GetChildrenFiles(DriveService service, string idParent)
        {
            List<File> result = new List<File>();
            FilesResource.ListRequest request = service.Files.List();
            request.MaxResults = 1000;
            request.Q = ("'" + idParent + "'" + " IN parents");
            int i = 0;
            do
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        FileList files = request.Execute();
                        result.AddRange(files.Items);
                        request.PageToken = files.NextPageToken;
                        i++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
            return result;
        }

        public static List<File> RetrieveContentAndChildrens(DriveService service, File fichier, String owner)
        {
            if (fichier.MimeType == "application/vnd.google-apps.folder")
            {
                var folderPath = @"" + ConfigurationManager.AppSettings["GoogleLocalFolderStore"] + owner + "\\" + fichier.Title + "\\";
                Directory.CreateDirectory(folderPath);

                List<File> childrens = GetChildrenFiles(service, fichier.Id);
                if (childrens != null && childrens.Any())
                {
                    foreach (var chil in childrens)
                    {
                        RetrieveContentAndChildrens(service, chil, owner + "\\" + fichier.Title);
                    }
                }
                return childrens;
            }
            var fileToDownload = DownloadFile(fichier, service, owner);
            return null;
        }

        public static List<File> RetrieveAllFilesAndFolders(DriveService service, String folderIdToRetrieve, String ownerToFilter)
        {
            var files = GetChildrenFiles(service, folderIdToRetrieve);

            foreach (File file in files)
            {
                if (file.Parents.Any())
                {
                    RetrieveContentAndChildrens(service, file, ownerToFilter);
                }
            }
            return files;
        }

        public static async Task<Stream> DownloadFile(File file, DriveService service, string owner = "tous")
        {
            try
            {
                string url = "";
                string fileExtension = "";

                if (!String.IsNullOrEmpty(file.DownloadUrl))
                {
                    url = file.DownloadUrl;
                    fileExtension = file.FileExtension;
                }
                else
                {
                    if (file.MimeType == "application/vnd.google-apps.folder")
                    {
                        return null;
                    }

                    if (file.MimeType == "application/vnd.google-apps.document")
                    {
                        url = file.ExportLinks.SingleOrDefault(f => f.Value.Contains("exportFormat=docx")).Value;
                        fileExtension = "docx";
                    }
                    else if (file.MimeType == "application/vnd.google-apps.presentation")
                    {
                        url = file.ExportLinks.SingleOrDefault(f => f.Value.Contains("exportFormat=pptx")).Value;
                        fileExtension = "pptx";
                    }
                    else if (file.MimeType == "application/vnd.google-apps.form")
                    {
                        url = file.EmbedLink;
                    }
                    else
                        if (file.MimeType == "application/vnd.google-apps.spreadsheet")
                        {
                            if (file.ExportLinks.Any(f => f.Value.Contains("exportFormat=xlsx")))
                            {
                                url = file.ExportLinks.SingleOrDefault(f => f.Value.Contains("exportFormat=xlsx")).Value;
                                fileExtension = "xlsx";
                            }
                        }
                        else if (file.MimeType == "application/vnd.google-apps.audio")
                        {
                        }
                        else if (file.MimeType == "application/vnd.google-apps.drawing")
                        {
                            url = file.ExportLinks.SingleOrDefault(f => f.Value.Contains("exportFormat=png")).Value;
                            fileExtension = "png";
                        }
                        else if (file.MimeType == "application/vnd.google-apps.sites")
                        {
                        }
                        else if (file.MimeType == "application/vnd.google-apps.video")
                        {
                        }
                }

                if (!String.IsNullOrEmpty(url))
                {
                    nbFiles++;
                    HttpResponseMessage response = await service.HttpClient.GetAsync(new Uri(url));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = await response.Content.ReadAsStreamAsync();
                        var fileTitle = file.Title.Replace('\\', '-');
                        fileTitle = fileTitle.Replace('/', '-');
                        fileTitle = fileTitle.Replace('#', ' ');
                        var filePath = @"" + ConfigurationManager.AppSettings["GoogleLocalFolderStore"] + owner + "\\" + fileTitle + "." + fileExtension;

                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            stream.CopyTo(fileStream);
                            nbCopiedFiles++;
                            Console.WriteLine("Fichiers copiés " + nbCopiedFiles + " " + fileTitle);
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return null;
            }
            return null;
        }
    }
}
