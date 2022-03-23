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
        private Repository<Adres> adresRepository;
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

            adresRepository = new Repository<Adres>(context);
        }

        [TestMethod]
        public void WhenCreatingRecordInDatabase_RecordIsAdded()
        {
            var adres = new Adres
            {
                Straat = "teststraat",
                Postcode = "2234GS",
                Woonplaats = "Tilburg"
            };

            var createdVrijwilliger = adresRepository.Create(adres);

            Assert.IsNotNull(createdVrijwilliger);
            Assert.IsTrue(createdVrijwilliger.Id > 0);
        }

        [TestMethod]
        public void WhenUpdatingRecordInDatabase_RecordIsUpdated()
        {
            var adres = new Adres
            {
                Straat = "teststraat",
                Postcode = "2234GS",
                Woonplaats = "Tilburg"
            };

            var createdVrijwilliger = adresRepository.Create(adres);
            Assert.IsNotNull(createdVrijwilliger);

            createdVrijwilliger.Straat = "Changed";
            adresRepository.Update(createdVrijwilliger);

            var updatedVrijwilliger = adresRepository.GetById(createdVrijwilliger.Id);

            Assert.IsTrue(updatedVrijwilliger.Straat == "Changed");
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void WhenUpdatingRecordInDatabaseThatDoesNotExits_ThrowsDbUpdateConcurrencyException()
        {
            var adres = new Adres
            {
                Id = -1,
                Straat = "teststraat",
                Postcode = "2234GS",
                Woonplaats = "Tilburg"
            };


            adresRepository.Update(adres);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenUpdatingRecordInDatabaseThatIsNull_ThrowsArgumentNullException()
        {
            adresRepository.Update(null);
        }

        [TestMethod]
        public void WhenDeletingRecordInDatabase_RecordIsDeleted()
        {
            var adres = new Adres
            {
                Straat = "teststraat",
                Postcode = "2234GS",
                Woonplaats = "Tilburg"
            };

            var createdVrijwilliger = adresRepository.Create(adres);
            Assert.IsNotNull(createdVrijwilliger);

            adresRepository.Delete(createdVrijwilliger);

            var updatedVrijwilliger = adresRepository.GetById(createdVrijwilliger.Id);

            Assert.IsNull(updatedVrijwilliger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenDeletingRecordInDatabaseThatIsNull_ThrowsArgumentNullException()
        {
            adresRepository.Delete(null);
        }

        [TestMethod]
        public void WhenGettingRecordById_RecordIsReturned()
        {
            var adres = new Adres
            {
                Straat = "teststraat",
                Postcode = "2234GS",
                Woonplaats = "Tilburg"
            };

            var createdVrijwilliger = adresRepository.Create(adres);
            Assert.IsNotNull(createdVrijwilliger);

            var retrievedVrijwilliger = adresRepository.GetById(createdVrijwilliger.Id);

            Assert.IsTrue(retrievedVrijwilliger.Id == createdVrijwilliger.Id);
        }

        [TestMethod]
        public void WhenGettingRecordByIdThatDoesNotExist_NullIsReturned()
        {
            var retrievedVrijwilliger = adresRepository.GetById(-1);

            Assert.IsNull(retrievedVrijwilliger);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var tempContext = new NuTwenteContext(contextOptions);
            var cleanupRepo =  new Repository<Adres>(tempContext);

            var toDelete = cleanupRepo
               .GetAll()
               .Where(x => x.Straat == "Test");

            toDelete
                .ToList()
                .ForEach(async x => cleanupRepo.Delete(x));


        }

    }
}