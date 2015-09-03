using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.SharePoint.Client;
using File = Microsoft.SharePoint.Client.File;

namespace DriveExport
{
    public static class OneDriveService
    {
        public static void UploadAllFilesFromFolder(string folder, string destinationFolder)
        {
            string[] filePaths = Directory.GetFiles(folder);
            foreach (var file in filePaths)
            {
                SaveFileToSharePoint(file, destinationFolder);
            }
        }

        public static void UploadAllFilesAndFolders(string folder, string destinationFolder)
        {
            var filesToCrowl = DirSearch(folder);
            foreach (var file in filesToCrowl)
            {
                string[] path = file.Split('\\');
                var folderPath = path.Take(path.Length - 1);

                if (path.Length == 4)
                {
                    SaveFileToSharePoint(file, destinationFolder);
                }
                else if (path.Length == 5)
                {
                    SaveFileToSharePoint(file, destinationFolder + "\\" + path[3]);
                }
                else if (path.Length > 4)
                {
                    string destinationFolderLocal = "";
                    for (int i = 4; i < path.Length; i++)
                    {
                        if (destinationFolderLocal == "")
                            destinationFolderLocal = destinationFolder;
                        destinationFolderLocal = destinationFolderLocal + "\\" + path[i - 1];
                    }
                    SaveFileToSharePoint(file, destinationFolderLocal);
                }
            }
        }

        private static List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt);
            }

            return files;
        }

        public static void SaveFileToSharePoint(string fileName, string destinationFolder)
        {
            try
            {
                using (var context = new ClientContext(ConfigurationManager.AppSettings["onedrive:urlSite"]))
                {
                    var passWord = new SecureString();
                    foreach (var c in ConfigurationManager.AppSettings["onedrive:password"]) passWord.AppendChar(c);
                    context.Credentials = new SharePointOnlineCredentials(ConfigurationManager.AppSettings["onedrive:login"], passWord);
                    var web = context.Web;

                    byte[] fileData = System.IO.File.ReadAllBytes(fileName);
                    using (Stream stream = new MemoryStream(fileData))
                    {
                        var newFile = new FileCreationInformation { ContentStream = stream, Url = Path.GetFileName(fileName) };
                        List docs = web.Lists.GetByTitle("Documents");

                        context.Load(docs.RootFolder.Folders);
                        context.ExecuteQuery();

                        List<Folder> foldersInLibrary = docs.RootFolder.Folders.ToList();

                        var destinationFolders = destinationFolder.Split('\\');
                        Folder folderToWork = null;
                        int i = 0;
                        foreach (var destFolder in destinationFolders)
                        {
                            if (i == 0)
                            {
                                if (foldersInLibrary.Count(s => s.Name == destFolder) == 0)
                                {
                                    docs.RootFolder.Folders.Add(destFolder);
                                    context.ExecuteQuery();
                                }

                                context.Load(docs.RootFolder.Folders.GetByUrl(destFolder).Files);
                                context.ExecuteQuery();
                                folderToWork = docs.RootFolder.Folders.GetByUrl(destFolder);
                            }

                            if (i >= 1)
                            {
                                if (folderToWork != null)
                                {
                                    context.Load(folderToWork);
                                    context.ExecuteQuery();

                                    context.Load(folderToWork.Folders);
                                    context.ExecuteQuery();

                                    List<Folder> foldersExists = folderToWork.Folders.ToList();

                                    if (foldersExists.Count(s => s.Name == destFolder) == 0)
                                    {
                                        folderToWork.Folders.Add(destFolder);
                                        context.ExecuteQuery();
                                    }

                                    folderToWork = folderToWork.Folders.GetByUrl(destFolder);
                                    context.Load(folderToWork.Files);
                                    context.ExecuteQuery();
                                }
                            }
                            i++;
                        }

                        if (folderToWork != null)
                        {
                            List<File> filesInLibrary = folderToWork.Files.ToList();
                            var fileNameSimple = Path.GetFileName(fileName);

                            var isPresent = filesInLibrary.Any(s => s.Name == fileNameSimple);
                            if (isPresent)
                            {
                                return;
                            }
                            folderToWork.Files.Add(newFile);
                            context.ExecuteQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(fileName);
                Console.WriteLine(e);
            }

        }
    }
}
