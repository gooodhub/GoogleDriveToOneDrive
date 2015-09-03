using System;
using DriveExport;
using Google.Apis.Drive.v2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriveExportTests
{
    [TestClass]
    [Ignore]
    public class GoogleDriveServiceTest
    {
        private DriveService _service;
        private String _ownerToFilter;

        [TestInitialize]
        public void Init()
        {
            _ownerToFilter = "c.burceaux@clt-services.com";
            _service = GoogleDriveService.Instanciate(_ownerToFilter);
        }

        [TestMethod]
        public void RetrieveAllGoogleFilesFromUser()
        {
            GoogleDriveService.RetrieveAllFilesAndFolders(_service, "0B281NSEIlLQCfm1yLTlSQktNY1ZDZE1hUTVzc05JWC1RZnlCSU1kWC1aRmNnT1gtWUJVNWM", _ownerToFilter);
        }

        [TestMethod]
        public void TestStartAll()
        {
            GoogleDriveService.StartFileAndFolders();
        }
    }
}
