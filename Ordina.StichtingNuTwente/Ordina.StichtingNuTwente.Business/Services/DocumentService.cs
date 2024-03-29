﻿using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System.Text;
using OfficeOpenXml;
using Ordina.StichtingNuTwente.Business.Helpers;
using FastExcel;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using Ordina.StichtingNuTwente.Extensions;

namespace Ordina.StichtingNuTwente.Business.Services
{
    /// <summary>
    /// Verwerkt Documenten en Excel files
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private readonly IRepository<Gastgezin> GastgezinRepo;
        private readonly IRepository<Reactie> ReationRepo;
        private readonly IRepository<Plaatsing> PlaatsingsRepo;
        private readonly IRepository<UserDetails> UserDetailRepo;
        private readonly IRepository<ContactLog> ContactLogRepo;
        private readonly IRepository<Comment> CommentRepo;
        public DocumentService(IRepository<Gastgezin> gastgezinRepo, IRepository<Reactie> reationRepo, IRepository<Plaatsing> plaatsingsRepo, IRepository<UserDetails> userDetailRepo, IRepository<ContactLog> contactLogRepo, IRepository<Comment> commentRepo)
        {
            GastgezinRepo = gastgezinRepo;
            ReationRepo = reationRepo;
            PlaatsingsRepo = plaatsingsRepo;
            UserDetailRepo = userDetailRepo;
            ContactLogRepo = contactLogRepo;
            CommentRepo = commentRepo;
        }


        public byte[] GenerateGastgezinnenPerGemeente()
        {
            var allGastgezinnen = GastgezinRepo.GetAll(IGastgezinService.IncludeProperties).Where(g => !g.Deleted && g.IntakeFormulier != null).ToList();
            var gastgezinnenMetVluchtelingen = allGastgezinnen.Where(g => g.Plaatsingen != null && g.Plaatsingen.Where(p => p.Active && (p.PlacementType == PlacementType.Plaatsing || p.PlacementType == PlacementType.GeplaatsteReservering)).Count() > 0);
            var gastgezinnenPerGemeente = new Dictionary<string, List<Gastgezin>>();
            foreach (var gastgezin in gastgezinnenMetVluchtelingen)
            {
                var gemeente = "";
                if (gastgezin.Contact?.Adres?.Postcode != null)
                {
                    var postcode = gastgezin.Contact.Adres.Postcode;
                    gemeente = GemeenteFromPostcodeAsync(postcode).Result;
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
            var text = $"Plaatsingen bij gastgezinnen NuTwente per gemeente d.d. {DateTime.Now.ToShortDateString()}\n\n";
            var globalTotalVluchtelingen = gastgezinnenMetVluchtelingen.SelectMany(g => g.Plaatsingen.Where(p => p.Active && (p.PlacementType == PlacementType.Plaatsing || p.PlacementType == PlacementType.GeplaatsteReservering))).Count();
            var totalGastgezinnen = allGastgezinnen.Count();
            text += $"Totaal: {globalTotalVluchtelingen} vluchtelingen in {gastgezinnenMetVluchtelingen.Count()} gastgezinnen. {totalGastgezinnen} gastgezinnen zijn verbonden aan NuTwente\n\n\n";

            foreach (var gastgezinGemeentePair in sorted)
            {
                var totalVluchtelingen = 0;
                foreach (var gastgezin in gastgezinGemeentePair.Value)
                {
                    var vluchtelingen = gastgezin.Plaatsingen.Count(p => p.Active && (p.PlacementType == PlacementType.Plaatsing || p.PlacementType == PlacementType.GeplaatsteReservering));
                    totalVluchtelingen += vluchtelingen;
                }
                text += $"{gastgezinGemeentePair.Key}: {totalVluchtelingen}\n";
            }

            text += $"\n--------------------------------------------------------------------------------\n";
            var vertrokkenVluchtelingen = PlaatsingsRepo.GetAll().Where(p => p.PlacementType == PlacementType.VerwijderdePlaatsing && p.DepartureDestination != null && p.DepartureDestination != DepartureDestination.Correctie);
            text += $"\nTotaal: {vertrokkenVluchtelingen.Count()} vertrokken vluchtelingen.\n\n";
            
            text += $"Per bestemming:\n\n";
            var destinationGroups = vertrokkenVluchtelingen.GroupBy(p => p.DepartureDestination).ToList();
            foreach (var group in destinationGroups)
            {
                var destinationGroupName = group.Key.GetAttribute<DisplayAttribute>().Name;
                var destinationGroupCount = group.Count();
                if(destinationGroupCount > 0)
                    text += $"{destinationGroupName}: {destinationGroupCount}\n";
            }

            text += $"\n--------------------------------------------------------------------------------\n";
            text += $"\nGegevens ter verificatie:\n\n";
            foreach (var gastgezinGemeentePair in sorted)
            {
                var gezinnen = "";
                var sortedGezinnen = gastgezinGemeentePair.Value.OrderBy(x=> x.IntakeFormulier?.Id);
                var totalVluchtelingen = 0;
                foreach (var gastgezin in sortedGezinnen)
                {
                    var vluchtelingen = gastgezin.Plaatsingen.Count( p=> p.Active && (p.PlacementType == PlacementType.Plaatsing || p.PlacementType == PlacementType.GeplaatsteReservering));
                    gezinnen += $"* {gastgezin.IntakeFormulier?.Id} - {gastgezin.Contact.Naam} - {gastgezin.Contact.Adres.Straat} {gastgezin.Contact.Adres.Woonplaats} - {vluchtelingen} p\n";
                    totalVluchtelingen += vluchtelingen;
                }
                text += $"{gastgezinGemeentePair.Key}: {totalVluchtelingen}\n";
                text += $"{gezinnen}\n";
            }
            return Encoding.UTF8.GetBytes(text);
        }

        public async Task<string> GemeenteFromPostcodeAsync(string postCode)
        {
            using HttpClient client = new() { };
            {
                var postCodeFixed = postCode.Trim().Replace(" ", "");
                var gemeente = await client.GetFromJsonAsync<LocatieApiResult>("https://geodata.nationaalgeoregister.nl/locatieserver/v3/free?q=postcode:" + postCodeFixed);
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
            var gastgezinnen = GastgezinRepo.GetAll("AanmeldFormulier,IntakeFormulier,Buddy,Intaker,Plaatsingen,PlaatsingsInfo,Comments").ToList();
            var aanmeldFormulieren = ReationRepo.GetAll("Antwoorden,Comments").Where(r => r.FormulierId == 1).ToList();
            var intakeFormulieren = ReationRepo.GetAll("Antwoorden,Comments").Where(r => r.FormulierId == 2).ToList();
            var plaatsingen = PlaatsingsRepo.GetAll("Vrijwilliger,Gastgezin").ToList();
            var userDetails = UserDetailRepo.GetAll("Reacties").ToList();
            var contactLogs = ContactLogRepo.GetAll("Contacter").ToList();
            var comments = CommentRepo.GetAll("Commenter").ToList();
            using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(template, outputFile))
            {
                AddExcelTab("Gastgezinnen", EntityToRowHelper.GastgezinToDataRows(gastgezinnen), fastExcel);
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
