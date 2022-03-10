using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Linq;

namespace Ordina.StichtingNuTwente.Test
{
    [TestClass]
    public class TestDatabaseImplementation
    {
        private Repository<Vrijwilliger> vrijwilligerRepository;
        private NuTwenteContext context;
        private static DbContextOptions<NuTwenteContext> contextOptions;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            contextOptions = new DbContextOptionsBuilder<NuTwenteContext>()
                .UseSqlServer(@"Server=tcp:nutwente.database.windows.net,1433;Initial Catalog=nutwente-dev;Persist Security Info=False;User ID=nutwente-admin;Password=NWswl$9S2vfl*nS7x!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                .Options;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            context = new NuTwenteContext(contextOptions);

            vrijwilligerRepository = new Repository<Vrijwilliger>(context);
        }

        [TestMethod]
        public void WhenCreatingRecordInDatabase_RecordIsAdded()
        {
            var vrijwilliger = new Vrijwilliger
            {
                FirstName = "Dummy",
                LastName = "Test"
            };

            var createdVrijwilliger = vrijwilligerRepository.Create(vrijwilliger);

            Assert.IsNotNull(createdVrijwilliger);
            Assert.IsTrue(createdVrijwilliger.Id > 0);
        }

        [TestMethod]
        public void WhenUpdatingRecordInDatabase_RecordIsUpdated()
        {
            var vrijwilliger = new Vrijwilliger
            {
                FirstName = "DummyUpdate",
                LastName = "Test"
            };

            var createdVrijwilliger = vrijwilligerRepository.Create(vrijwilliger);
            Assert.IsNotNull(createdVrijwilliger);

            createdVrijwilliger.FirstName = "Changed";
            vrijwilligerRepository.Update(createdVrijwilliger);

            var updatedVrijwilliger = vrijwilligerRepository.GetById(createdVrijwilliger.Id);

            Assert.IsTrue(updatedVrijwilliger.FirstName == "Changed");
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void WhenUpdatingRecordInDatabaseThatDoesNotExits_ThrowsDbUpdateConcurrencyException()
        {
            var vrijwilliger = new Vrijwilliger
            {
                Id = -1,
                FirstName = "DummyUpdate",
                LastName = "Test"
            };

            vrijwilligerRepository.Update(vrijwilliger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenUpdatingRecordInDatabaseThatIsNull_ThrowsArgumentNullException()
        {
            vrijwilligerRepository.Update(null);
        }

        [TestMethod]
        public void WhenDeletingRecordInDatabase_RecordIsDeleted()
        {
            var vrijwilliger = new Vrijwilliger
            {
                FirstName = "DummyUpdate",
                LastName = "Test"
            };

            var createdVrijwilliger = vrijwilligerRepository.Create(vrijwilliger);
            Assert.IsNotNull(createdVrijwilliger);

            vrijwilligerRepository.Delete(createdVrijwilliger);

            var updatedVrijwilliger = vrijwilligerRepository.GetById(createdVrijwilliger.Id);

            Assert.IsNull(updatedVrijwilliger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenDeletingRecordInDatabaseThatIsNull_ThrowsArgumentNullException()
        {
            vrijwilligerRepository.Delete(null);
        }

        [TestMethod]
        public void WhenGettingRecordById_RecordIsReturned()
        {
            var vrijwilliger = new Vrijwilliger
            {
                FirstName = "DummyGet",
                LastName = "Test"
            };

            var createdVrijwilliger = vrijwilligerRepository.Create(vrijwilliger);
            Assert.IsNotNull(createdVrijwilliger);

            var retrievedVrijwilliger = vrijwilligerRepository.GetById(createdVrijwilliger.Id);

            Assert.IsTrue(retrievedVrijwilliger.Id == createdVrijwilliger.Id);
        }

        [TestMethod]
        public void WhenGettingRecordByIdThatDoesNotExist_NullIsReturned()
        {
            var retrievedVrijwilliger = vrijwilligerRepository.GetById(-1);

            Assert.IsNull(retrievedVrijwilliger);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var tempContext = new NuTwenteContext(contextOptions);
            var cleanupRepo =  new Repository<Vrijwilliger>(tempContext);

            var toDelete = cleanupRepo
               .GetAll()
               .Where(x => x.LastName == "Test");

            toDelete
                .ToList()
                .ForEach(async x => cleanupRepo.Delete(x));


        }

    }
}