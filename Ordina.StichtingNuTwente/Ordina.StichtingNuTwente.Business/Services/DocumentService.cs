using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;

using Newtonsoft.Json;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System.Text;
using OfficeOpenXml;
using Ordina.StichtingNuTwente.Business.Helpers;
using FastExcel;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class DocumentService : IDocumentService
    {
        public NuTwenteContext _context { get; }
        HttpClient client = new HttpClient();
        public DocumentService(NuTwenteContext context)
        {
            _context = context;
        }


        public byte[] GenerateGastgezinnenPerGemeente()
        {
            var gastgezinRepo = new Repository<Gastgezin>(_context);

            var allGastgezinnen = gastgezinRepo.GetAll(IGastgezinService.IncludeProperties).Where(g => !g.Deleted).ToList();
            var gastgezinnenMetVluchtelingen = allGastgezinnen.Where(g => g.Plaatsingen != null && g.Plaatsingen.Where(p => p.Active && (p.PlacementType == PlacementType.Plaatsing || p.PlacementType == PlacementType.GeplaatsteReservering)).Count() > 0);
            var gastgezinnenPerGemeente = new Dictionary<string, List<Gastgezin>>();
            foreach (var gastgezin in gastgezinnenMetVluchtelingen)
            {
                var gemeente = "";
                if (gastgezin.Contact?.Adres?.Postcode != null)
                {
                    var postcode = gastgezin.Contact.Adres.Postcode;
                    gemeente = GemeenteFromPostcode(postcode);
                    if (gemeente == "")
                    {
                        gemeente = postcode;
                    }
                }
                else
                {
                    gemeente = "Geen postcode beschikbaar";
                }
                List<Gastgezin> gastgezinnen = new List<Gastgezin>();
                if (gastgezinnenPerGemeente.TryGetValue(gemeente, out gastgezinnen))
                {
                    gastgezinnen.Add(gastgezin);
                    gastgezinnenPerGemeente[gemeente] = gastgezinnen;
                }
                else
                {
                    gastgezinnenPerGemeente[gemeente] = new List<Gastgezin>() { gastgezin };
                }
            }
            var sorted = gastgezinnenPerGemeente.OrderBy(x => x.Key).ToList();
            var text = $"Aantal plaatsingen gastgezinnen NuTwente d.d. {DateTime.Now.ToShortDateString()} \n";
            foreach(var gastgezinGemeentePair in sorted)
            {
                var gezinnen = "";
                var sortedGezinnen = gastgezinGemeentePair.Value.OrderBy(x=> x.IntakeFormulier?.Id);
                var totalVluchtelingen = 0;
                foreach (var gastgezin in sortedGezinnen)
                {
                    var vluchtelingen = gastgezin.Plaatsingen.Count( p=> p.Active && (p.PlacementType == PlacementType.Plaatsing || p.PlacementType == PlacementType.GeplaatsteReservering));
                    gezinnen += $"* {gastgezin.IntakeFormulier?.Id} - {gastgezin.Contact.Naam} - {gastgezin.Contact.Adres.Straat} {gastgezin.Contact.Adres.Woonplaats} - {vluchtelingen} p \n";
                    totalVluchtelingen += vluchtelingen;
                }
                text += $"{gastgezinGemeentePair.Key}: {totalVluchtelingen} \n";
                text += gezinnen;
            }
            return Encoding.ASCII.GetBytes(text);
        }

        public string GemeenteFromPostcode(string postCode)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://geodata.nationaalgeoregister.nl/locatieserver/v3/free?q=postcode:" + postCode);
                var gemeente = JsonConvert.DeserializeObject<LocatieApiResult>(json);
                var response = gemeente.Response;
                if (response != null && response.Docs.Count > 0)
                {
                    return response.Docs[0].Gemeentenaam;
                }
            }

            return "";
        }



        public byte[] GenerateDataDumpToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var outputFile = new FileInfo("output.xlsx");
            var template = new FileInfo("DataDumpTemplate.xlsx");
            byte[] returnValue = new byte[0];
            var gastgezinnen = new Repository<Gastgezin>(_context).GetAll("AanmeldFormulier,IntakeFormulier,Buddy,Begeleider,Plaatsingen,PlaatsingsInfo,Comments").ToList();
            var reactieRepository = new Repository<Reactie>(_context);
            var aanmeldFormulieren = reactieRepository.GetAll("Antwoorden,Comments").Where(r => r.FormulierId == 1).ToList();
            var intakeFormulieren = reactieRepository.GetAll("Antwoorden,Comments").Where(r => r.FormulierId == 2).ToList();
            var plaatsingen = new Repository<Plaatsing>(_context).GetAll("Vrijwilliger,Gastgezin").ToList();
            var userDetails = new Repository<UserDetails>(_context).GetAll("Reacties").ToList();
            var contactLogs = new Repository<ContactLog>(_context).GetAll("Contacter").ToList();
            var comments = new Repository<Comment>(_context).GetAll("Commenter").ToList();
            using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(template, outputFile))
            {
                AddExcelTab("Gastgezinnen", EntityToRowHelper.GastgezinToDataRow(gastgezinnen), fastExcel);
                AddExcelTab("AanmeldFormulier", EntityToRowHelper.ReactiesToDataRows(aanmeldFormulieren, 1), fastExcel);
                AddExcelTab("IntakeFormulier", EntityToRowHelper.ReactiesToDataRows(intakeFormulieren, 2), fastExcel);
                AddExcelTab("Plaatsingen", EntityToRowHelper.PlaatsingenToDataRows(plaatsingen), fastExcel);
                AddExcelTab("Vrijwilligers", EntityToRowHelper.VrijwilligersToDataRows(userDetails), fastExcel);
                AddExcelTab("ContactLogs", EntityToRowHelper.ContactLogsToDataRows(contactLogs), fastExcel);
                AddExcelTab("Comments", EntityToRowHelper.CommentsToDataRows(comments), fastExcel);
            }

            using (var filestream = outputFile.OpenRead())
            {
                BinaryReader br = new BinaryReader(filestream);
                long numBytes = new FileInfo(outputFile.Name).Length;
                returnValue = br.ReadBytes((int)numBytes);
            }
            outputFile.Delete();

            return returnValue;
        }

        private void AddExcelTab(string tabName, ICollection<Row> data, FastExcel.FastExcel excel)
        {
            var worksheet = new Worksheet();
            worksheet.Rows = data;
            excel.Write(worksheet, tabName);
        }
    }
}
