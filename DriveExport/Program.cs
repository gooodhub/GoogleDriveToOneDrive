using System;
using System.Configuration;

namespace DriveExport
{
    class Program
    {
        static void Main(string[] args)
        {
            GoogleDriveService.StartFileAndFolders();
            Console.WriteLine("L'import local des documents google a été réalisé, appuyez sur une touche pour continuer");
            Console.ReadLine();
            OneDriveService.UploadAllFilesAndFolders(@"" + ConfigurationManager.AppSettings["GoogleLocalFolderStore"] + ConfigurationManager.AppSettings["ownerToFilter"], "Ced");
            Console.WriteLine("La mise a disposition de vos documents vers onedrive est terminée");
            Console.ReadLine();
        }
    }
}
