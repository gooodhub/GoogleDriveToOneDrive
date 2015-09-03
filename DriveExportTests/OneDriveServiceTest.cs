using System;
using DriveExport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriveExportTests
{
    [TestClass]
    [Ignore]
    public class OneDriveServiceTest
    {
        [TestMethod]
        public void CanUploadFileToSharePoint()
        {
            OneDriveService.SaveFileToSharePoint(@"C:\myfile.txt", "PL");
        }

        [TestMethod]
        public void UploadAllFilesFromFolder()
        {
            OneDriveService.UploadAllFilesFromFolder(@"F:\GoogleDrive\c.burceaux@clt-services.com", "PL2");           
        }

        [TestMethod]
        public void UploadAllFilesAndFoldersFromFolder()
        {
            OneDriveService.UploadAllFilesAndFolders(@"F:\GoogleDrive\c.burceaux@clt-services.com", "Ced");
        }
    }
}
