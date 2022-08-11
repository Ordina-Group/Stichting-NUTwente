using Ordina.StichtingNuTwente.Business.Helpers;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Ordina.StichtingNuTwente.Models.Mappings;
using OfficeOpenXml;
using FastExcel;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IGastgezinService _gastgezinService;
        private readonly IUserService _userService;
        private readonly IReactionService _reactionService;

        private readonly IRepository<Gastgezin> GastgezinRepo;
        private readonly IRepository<Reactie> ReactionRepo;
        private readonly IRepository<Plaatsing> PlaatsingsRepo;
        private readonly IRepository<UserDetails> UserDetailRepo;
        private readonly IRepository<Persoon> PersoonRepo;
        private readonly IRepository<Adres> AdresRepo;
        private readonly IRepository<Antwoord> AntwoordRepo;
        private readonly IRepository<PlaatsingsInfo> PlaatsingsInfoRepo;

        public MaintenanceService(IGastgezinService gastgezinService, IUserService userService, IReactionService reactionService, IRepository<Gastgezin> gastgezinRepo, IRepository<Reactie> reactionRepo, IRepository<UserDetails> userDetailRepo, IRepository<Persoon> persoonRepo, IRepository<Adres> adresRepo, IRepository<Antwoord> antwoordRepo, IRepository<PlaatsingsInfo> plaatsingsInfoRepo, IRepository<Plaatsing> plaatsingsRepo)
        {
            _gastgezinService = gastgezinService;
            _userService = userService;
            _reactionService = reactionService;
            GastgezinRepo = gastgezinRepo;
            ReactionRepo = reactionRepo;
            UserDetailRepo = userDetailRepo;
            PersoonRepo = persoonRepo;
            AdresRepo = adresRepo;
            AntwoordRepo = antwoordRepo;
            PlaatsingsInfoRepo = plaatsingsInfoRepo;
            PlaatsingsRepo = plaatsingsRepo;
        }


        public List<MaintenanceMessage> UpdateStatus()
        {
            var messages = new List<MaintenanceMessage>();
            var gastgezinnen = GastgezinRepo.GetAll();
            foreach (var gastgezin in gastgezinnen)
            {
                var prevStatus = gastgezin.Status;
                if (gastgezin.Status == GastgezinStatus.NoodOpvang)
                {
                    gastgezin.NoodOpvang = true;
                }
                else if (gastgezin.Status == GastgezinStatus.OnHold)
                {
                    gastgezin.OnHold = true;
                }
                GastgezinRepo.Update(gastgezin);
                messages.Add(new MaintenanceMessage($"Status was {prevStatus} is now {gastgezin.Status}. OnHold is {gastgezin.OnHold}. Nood is {gastgezin.NoodOpvang}", MaintenanceMessageType.Success));
            }
            return messages;
        }

        public List<MaintenanceMessage> LinkIntakerToGastgezin()
        {
            var messages = new List<MaintenanceMessage>();
            var gastgezinnen = GastgezinRepo.GetAll("IntakeFormulier.UserDetails,Intaker");
            foreach (var gastgezin in gastgezinnen)
            {
                if (gastgezin.IntakeFormulier != null && gastgezin.IntakeFormulier.UserDetails != null && gastgezin.Intaker == null)
                {
                    var userDetails = gastgezin.IntakeFormulier.UserDetails;
                    gastgezin.Intaker = userDetails;
                    GastgezinRepo.Update(gastgezin);
                    messages.Add(new MaintenanceMessage($@"Gastgezin with id {gastgezin.Id} got linked with {userDetails.FirstName} {userDetails.LastName}", MaintenanceMessageType.Success));
                }
                else
                {
                    messages.Add(new MaintenanceMessage($@"Gastgezin with id {gastgezin.Id} did not change"));
                }
            }
            return messages;
        }

        public List<MaintenanceMessage> LoadDataFromExcel(Stream excelStream, int formId)
        {
            var messages = new List<MaintenanceMessage>();
            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            var rowNum = 0;
            Form questionForm = FormHelper.GetFormFromFileId(formId);
            var colomnIndexToQuestionID = new Dictionary<int, int>();
            foreach (var row in rows)
            {
                if (rowNum == 0)
                {
                    var index = 0;
                    var cells = row.Cells;
                    var done = false;

                    foreach (var cell in cells)
                    {
                        foreach (var s in questionForm.Sections)
                        {
                            done = false;
                            foreach (var q in s.Questions)
                            {
                                if (q.Text == cell.ToString())
                                {
                                    colomnIndexToQuestionID.Add(index, q.Id);
                                    done = true;
                                    break;
                                }
                            }
                            if (done) break;
                        }
                        index++;
                    }
                }
                else
                {
                    var reaction = new Reactie()
                    {
                        DatumIngevuld = DateTime.Now,
                        Antwoorden = new List<Antwoord>()
                    };
                    reaction.FormulierId = formId;

                    var cells = row.Cells;

                    var index = 0;
                    foreach (var cell in cells)
                    {
                        if (colomnIndexToQuestionID.ContainsKey(index))
                        {
                            var antwoord = new Antwoord();
                            var id = colomnIndexToQuestionID[index];
                            antwoord.IdVanVraag = id;
                            antwoord.Response = cell.ToString();
                            reaction.Antwoorden.Add(antwoord);
                        }
                        index++;
                    }
                    var webmodel = ReactieMapping.FromDatabaseToWebModel(reaction);
                    var created = _reactionService.Save(webmodel, null);
                    if (created)
                        messages.Add(new MaintenanceMessage($"Added reactie for row number {row.RowNumber}", MaintenanceMessageType.Success));
                    else
                        messages.Add(new MaintenanceMessage($"Was unable to create reactie for row number {row.RowNumber}", MaintenanceMessageType.Error));
                }
                rowNum++;
            }
            return messages;
        }

        public List<MaintenanceMessage> LoadPlaatsingDataFromExcel(Stream excelStream, ClaimsPrincipal User)
        {
            var messages = new List<MaintenanceMessage>();
            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            var rowNum = 0;
            var gastgezinnen = GastgezinRepo.GetAll("IntakeFormulier");
            foreach (var row in rows)
            {
                if (rowNum > 0)
                {
                    var index = 0;
                    var cells = row.Cells;
                    var done = false;
                    Gastgezin gastgezin = new Gastgezin();
                    int adults;
                    int children;
                    int unknown;
                    DateTime resDate = new DateTime();
                    DateTime plaatsingDate = new DateTime();

                    foreach (var cell in cells)
                    {
                        if (index == 0)
                        {
                            int number;
                            bool success = int.TryParse(cell.Value.ToString(), out number);
                            if (success)
                            {
                                gastgezin = gastgezinnen.Where(g => g.IntakeFormulier != null).FirstOrDefault(g => g.IntakeFormulier.Id == number);
                            }
                            else
                            {
                                messages.Add(new MaintenanceMessage($@"IntakeFormId for Gastgezin with IntakeFormId {cell.Value} could not be parsed", MaintenanceMessageType.Error));
                            }
                            if (gastgezin == null)
                            {
                                messages.Add(new MaintenanceMessage($@"Gastgezin with IntakeFormId {cell.Value} could not be found", MaintenanceMessageType.Error));
                            }
                        }
                        if (gastgezin != null)
                        {
                            if (index == 1)
                            {
                                var val = cell.Value.ToString();
                                if (val.Contains("on hold"))
                                {
                                    if (!gastgezin.OnHold)
                                    {
                                        gastgezin.OnHold = true;
                                        _gastgezinService.UpdateGastgezin(gastgezin, gastgezin.Id);
                                        messages.Add(new MaintenanceMessage($@"OnHold status added to Gastgezin with IntakeFormId {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Success));
                                    }
                                    else
                                    {
                                        messages.Add(new MaintenanceMessage($@"OnHold status already exists for Gastgezin with IntakeFormId {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Warning));
                                    }
                                }
                            }
                            if (index == 2)
                            {
                                var val = cell.Value.ToString();
                                if (val != ".")
                                {
                                    double d = double.Parse(val);
                                    resDate = DateTime.FromOADate(d);
                                }
                            }
                            if (index == 3)
                            {
                                var val = cell.Value.ToString();
                                if (val != ".")
                                {
                                    double d = double.Parse(val);
                                    plaatsingDate = DateTime.FromOADate(d);
                                }
                            }
                            if (index == 4)
                            {
                                var val = cell.Value.ToString();
                                if (val != ".")
                                {
                                    if (resDate != new DateTime())
                                    {
                                        if (val.Contains("v"))
                                        {
                                            var total = int.Parse(val[val.IndexOf("v") + 1].ToString());
                                            for (int i = 0; i < total; i++)
                                            {
                                                var plaatsing = new Plaatsing()
                                                {
                                                    DateTime = resDate,
                                                    AgeGroup = AgeGroup.Volwassene,
                                                    Amount = 1,
                                                    Age = -1,
                                                    Gender = Gender.Onbekend,
                                                    Active = true,
                                                    Gastgezin = gastgezin,
                                                    PlacementType = PlacementType.Reservering,
                                                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)

                                                };
                                                _gastgezinService.AddPlaatsing(plaatsing);
                                                messages.Add(new MaintenanceMessage($@"{plaatsing.PlacementType} for {plaatsing.Amount} {plaatsing.AgeGroup} on {plaatsing.DateTime} added to Gastgezin with IntakeFormId {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Success));
                                            }
                                        }
                                        if (val.Contains("k"))
                                        {
                                            var total = int.Parse(val[val.IndexOf("k") + 1].ToString());
                                            for (int i = 0; i < total; i++)
                                            {
                                                var plaatsing = new Plaatsing()
                                                {
                                                    DateTime = resDate,
                                                    AgeGroup = AgeGroup.Kind,
                                                    Amount = 1,
                                                    Age = -1,
                                                    Gender = Gender.Onbekend,
                                                    Active = true,
                                                    Gastgezin = gastgezin,
                                                    PlacementType = PlacementType.Reservering,
                                                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                                                };
                                                _gastgezinService.AddPlaatsing(plaatsing);
                                                messages.Add(new MaintenanceMessage($@"{plaatsing.PlacementType} for {plaatsing.Amount} {plaatsing.AgeGroup} on {plaatsing.DateTime} added to Gastgezin with IntakeFormId {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Success));
                                            }
                                        }
                                        if (!val.Contains("v") && !val.Contains("k"))
                                        {
                                            var total = int.Parse(val);
                                            for (int i = 0; i < total; i++)
                                            {
                                                var plaatsing = new Plaatsing()
                                                {
                                                    DateTime = resDate,
                                                    AgeGroup = AgeGroup.Onbekend,
                                                    Amount = 1,
                                                    Age = -1,
                                                    Gender = Gender.Onbekend,
                                                    Active = true,
                                                    Gastgezin = gastgezin,
                                                    PlacementType = PlacementType.Reservering,
                                                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                                                };
                                                _gastgezinService.AddPlaatsing(plaatsing);
                                                messages.Add(new MaintenanceMessage($@"{plaatsing.PlacementType} for {plaatsing.Amount} {plaatsing.AgeGroup} on {plaatsing.DateTime} added to Gastgezin with IntakeFormId {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Success));
                                            }
                                        }
                                    }
                                    if (plaatsingDate != new DateTime())
                                    {
                                        if (val.Contains("v"))
                                        {
                                            var total = int.Parse(val[val.IndexOf("v") + 1].ToString());
                                            for (int i = 0; i < total; i++)
                                            {
                                                var plaatsing = new Plaatsing()
                                                {
                                                    DateTime = plaatsingDate,
                                                    AgeGroup = AgeGroup.Volwassene,
                                                    Amount = 1,
                                                    Age = -1,
                                                    Gender = Gender.Onbekend,
                                                    Active = true,
                                                    Gastgezin = gastgezin,
                                                    PlacementType = PlacementType.Plaatsing,
                                                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                                                };
                                                _gastgezinService.AddPlaatsing(plaatsing);
                                                messages.Add(new MaintenanceMessage($@"{plaatsing.PlacementType} for {plaatsing.Amount} {plaatsing.AgeGroup} on {plaatsing.DateTime} added to Gastgezin with IntakeFormId {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Success));
                                            }
                                        }
                                        if (val.Contains("k"))
                                        {
                                            var total = int.Parse(val[val.IndexOf("k") + 1].ToString());
                                            for (int i = 0; i < total; i++)
                                            {
                                                var plaatsing = new Plaatsing()
                                                {
                                                    DateTime = plaatsingDate,
                                                    AgeGroup = AgeGroup.Kind,
                                                    Amount = 1,
                                                    Age = -1,
                                                    Gender = Gender.Onbekend,
                                                    Active = true,
                                                    Gastgezin = gastgezin,
                                                    PlacementType = PlacementType.Plaatsing,
                                                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                                                };
                                                _gastgezinService.AddPlaatsing(plaatsing);
                                                messages.Add(new MaintenanceMessage($@"{plaatsing.PlacementType} for {plaatsing.Amount} {plaatsing.AgeGroup} on {plaatsing.DateTime} added to Gastgezin with IntakeFormId {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Success));
                                            }
                                        }
                                        if (!val.Contains("v") && !val.Contains("k"))
                                        {
                                            var total = int.Parse(val);
                                            for (int i = 0; i < total; i++)
                                            {
                                                var plaatsing = new Plaatsing()
                                                {
                                                    DateTime = plaatsingDate,
                                                    AgeGroup = AgeGroup.Onbekend,
                                                    Amount = 1,
                                                    Age = -1,
                                                    Gender = Gender.Onbekend,
                                                    Active = true,
                                                    Gastgezin = gastgezin,
                                                    PlacementType = PlacementType.Plaatsing,
                                                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                                                };
                                                _gastgezinService.AddPlaatsing(plaatsing);
                                                messages.Add(new MaintenanceMessage($@"{plaatsing.PlacementType} for {plaatsing.Amount} {plaatsing.AgeGroup} on {plaatsing.DateTime} added to Gastgezin with IntakeFormId {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Success));
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        index++;
                    }
                }
                rowNum++;
            }
            return messages.ToList();
        }

        public List<MaintenanceMessage> UpdateDataFromExcel(Stream excelStream, int formId)
        {
            var messages = new List<MaintenanceMessage>();


            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            var rowNum = 0;
            Form questionForm = FormHelper.GetFormFromFileId(formId);
            var colomnIndexToQuestionID = new Dictionary<int, int>();
            foreach (var row in rows)
            {
                if (rowNum == 0)
                {
                    var index = 0;
                    var cells = row.Cells;
                    var done = false;

                    foreach (var cell in cells)
                    {
                        foreach (var s in questionForm.Sections)
                        {
                            done = false;
                            foreach (var q in s.Questions)
                            {
                                if (q.Text == cell.ToString())
                                {
                                    colomnIndexToQuestionID.Add(index, q.Id);
                                    done = true;
                                    break;
                                }
                            }
                            if (done) break;
                        }
                        index++;
                    }
                }
                else
                {
                    Reactie? reaction = null;


                    var cells = row.Cells;

                    var index = 0;
                    foreach (var cell in cells)
                    {
                        if (index == 0)
                        {
                            var id = int.Parse(cell.ToString());
                            reaction = ReactionRepo.GetById(id, "Antwoorden");
                            if (reaction == null)
                            {
                                messages.Add(new MaintenanceMessage($@"From with Id {id} was not found", MaintenanceMessageType.Warning));
                            }
                        }
                        else if (reaction != null && colomnIndexToQuestionID.ContainsKey(index))
                        {
                            var antwoord = new Antwoord();
                            var id = colomnIndexToQuestionID[index];
                            antwoord.IdVanVraag = id;
                            antwoord.Response = cell.ToString();
                            var a = reaction.Antwoorden.Where(a => a.IdVanVraag == id);
                            foreach (var ant in a)
                                reaction.Antwoorden.Remove(ant);
                            reaction.Antwoorden.Add(antwoord);
                        }
                        index++;
                    }
                    if (reaction != null)
                    {
                        ReactionRepo.Update(reaction);
                        messages.Add(new MaintenanceMessage($@"From with Id {reaction.Id} was updated", MaintenanceMessageType.Success));
                    }
                }
                rowNum++;
            }
            return messages.ToList();
        }


        public List<MaintenanceMessage> ImportGastgezinnen(Stream excelStream)
        {
            var messages = new List<MaintenanceMessage>();
            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                var row = rows[rowIndex];
                if (rowIndex == 0)
                {
                    continue;
                }

                int aanmeldId = 0;
                int? intakeId = null;
                string aanmeldIdText = "";
                string intakeIdText = "";

                try
                {
                    var cells = row.Cells.ToList();
                    aanmeldIdText = cells[0].ToString();
                    intakeIdText = cells[1].ToString();

                    if (string.IsNullOrWhiteSpace(aanmeldIdText))
                    {
                        continue;
                    }

                    aanmeldId = Convert.ToInt32(aanmeldIdText);

                    if (!string.IsNullOrWhiteSpace(intakeIdText))
                    {
                        try
                        {
                            intakeId = Convert.ToInt32(intakeIdText);
                        }
                        catch (FormatException)
                        {
                            //Ignore any format exception
                        }
                    }

                    var persoon = PersoonRepo.Get(e => e.Reactie.Id == aanmeldId, "Gastgezin");
                    if (persoon == null)
                    {
                        messages.Add(new MaintenanceMessage($@"Aanmeld Id: {aanmeldId} Intake Id: {intakeId} - Er is geen persoon met Reactie Id = Aanmeld Id", MaintenanceMessageType.Error));
                        continue;
                    }

                    if (persoon.Gastgezin != null)
                    {
                        //Already has a gastgezin coupled
                        messages.Add(new MaintenanceMessage($@"Aanmeld Id: {aanmeldId} Intake Id: {intakeId} - Gastgezin bestaat al."));
                        continue;
                    }

                    var gastgezin = new Gastgezin
                    {
                        Contact = persoon
                    };

                    if (intakeId.HasValue)
                    {
                        gastgezin.IntakeFormulier = ReactionRepo.GetById((intakeId.Value));

                        if (gastgezin.IntakeFormulier == null)
                        {
                            messages.Add(new MaintenanceMessage($@"Aanmeld Id: {aanmeldId} Intake Id: {intakeId} - Intake formulier was niet gevonden. Gastgezin niet toegevoed", MaintenanceMessageType.Error));
                            continue;
                        }
                    }

                    GastgezinRepo.Create(gastgezin);

                    persoon.Gastgezin = gastgezin;
                    PersoonRepo.Update(persoon);

                    messages.Add(new MaintenanceMessage($@"Aanmeld Id: {aanmeldId} Intake Id: {intakeId} - Gastgezin toegevoegd", MaintenanceMessageType.Success));
                }
                catch (FormatException ex)
                {
                    messages.Add(new MaintenanceMessage($@"Aanmeld Id: {aanmeldIdText} Intake Id: {intakeIdText} - {ex.Message}", MaintenanceMessageType.Error));
                }
                catch (Exception ex)
                {
                    messages.Add(new MaintenanceMessage($@"Aanmeld Id: {aanmeldId} Intake Id: {intakeId} - {ex.Message}", MaintenanceMessageType.Error));
                }
            }

            messages.Add(new MaintenanceMessage($@"Gastgezinnen total in database: {GastgezinRepo.GetAll().Count()}", MaintenanceMessageType.Error));

            return messages;
        }
        public List<MaintenanceMessage> UpdateAanmeldingFromIntakeId(Stream excelStream)
        {
            var messages = new List<MaintenanceMessage>();

            var gastgezinnen = GastgezinRepo.GetAll("AanmeldFormulier,Contact.Reactie");
            foreach (var gastgezin in gastgezinnen)
            {
                if (gastgezin.AanmeldFormulier == null)
                {
                    gastgezin.AanmeldFormulier = gastgezin.Contact.Reactie;
                }
            }

            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                var row = rows[rowIndex];
                if (rowIndex == 0)
                {
                    continue;
                }
                var cells = row.Cells.ToList();
                if (cells.Count > 1)
                {
                    string intakeIdText = cells[0].ToString();
                    string aanmeldIdText = cells[1].ToString();
                    int intakeId = 0;
                    int aanmeldId = 0;
                    try
                    {
                        intakeId = int.Parse(intakeIdText);
                        aanmeldId = int.Parse(aanmeldIdText);
                    }
                    catch (Exception)
                    {
                        messages.Add(new MaintenanceMessage($@"Could not parse {intakeIdText} or {aanmeldIdText}", MaintenanceMessageType.Error));
                    }
                    var gastgezin = GastgezinRepo.GetFirstOrDefault(x => x.IntakeFormulier != null && x.IntakeFormulier.Id == intakeId, "IntakeFormulier,AanmeldFormulier");
                    if (gastgezin == null)
                    {
                        messages.Add(new MaintenanceMessage($@"no gastgezin found with intake id {intakeId}", MaintenanceMessageType.Error));
                        continue;
                    }

                    var aanmeldFormulier = ReactionRepo.GetFirstOrDefault(x => x.Id == aanmeldId);
                    if (aanmeldFormulier == null)
                    {
                        messages.Add(new MaintenanceMessage($@"no form found with aanmeld id {aanmeldId}", MaintenanceMessageType.Error));
                        continue;
                    }

                    gastgezin.AanmeldFormulier = aanmeldFormulier;
                    GastgezinRepo.Update(gastgezin);
                    messages.Add(new MaintenanceMessage($@"Coupled {aanmeldId} to {intakeId}", MaintenanceMessageType.Success));
                }
                else
                {
                    messages.Add(new MaintenanceMessage($@"Not all cells defined", MaintenanceMessageType.Error));
                }

            }

            return messages;
        }

        public DatabaseIntegrityModel TestDatabaseIntegrity()
        {
            var result = new DatabaseIntegrityModel();

            result.Inconsistencies.Add(TestDoubleAanmeldIntakeInGastgezinnen());
            result.Inconsistencies.Add(TestMissingGastgezin());
            result.Inconsistencies.Add(TestMultiplePersoonGastgezin());
            result.Inconsistencies.Add(TestPersonHasNoGastgezin());
            result.Inconsistencies.Add(TestMissingAanmeldForIntakeFormulier());
            result.Inconsistencies.Add(TestMissingAanmeldForGastgezin());
            result.Inconsistencies.Add(TestCicleReference());
            result.Inconsistencies.Add(TestIntakeWithNoGastgezin());

            result.Statistics.Add(TestCountAllTables());
            result.Statistics.Add(TestHowManyAanmeldDontHaveIntake());

            return result;
        }


        private DatabaseIntegrityTest TestIntakeWithNoGastgezin()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Intake forms with no gastgezin",
                Description = "Checking all Intake Id for if they are in a Gastgezin"
            };

            var reacties = ReactionRepo.GetAll().Where(r => r.FormulierId == 2);
            var gastgezinnen = GastgezinRepo.GetAll("IntakeFormulier");

            foreach (var reactie in reacties)
            {
                var gastgezin = gastgezinnen.FirstOrDefault(g => g.IntakeFormulier != null && g.IntakeFormulier.Id == reactie.Id);
                if (gastgezin != null)
                    result.AddMessage($"{reactie.Id} has gastgezin", DatabaseIntegrityLevel.Success);
                else
                    result.AddMessage($"{reactie.Id} has no aanmelding", DatabaseIntegrityLevel.Error);
            }

            return result;
        }

        private DatabaseIntegrityTest TestMultiplePersoonGastgezin()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Duplicate Persoon (Contact) for Gastgezin",
                Description = "Checking all Gastgezin->Contact are unique"
            };

            var gastgezins = from c in GastgezinRepo.GetMany(e => e.Contact != null, "Contact")
                             group c by c.Contact into g
                             where g.Skip(1).Any()
                             from c in g
                             select c;

            if (!gastgezins.Any())
            {
                result.AddMessage("No duplicate Contact for Gastgezin", DatabaseIntegrityLevel.Success);
            }
            else
            {
                foreach (var gastgezin in gastgezins)
                {
                    result.AddMessage($@"Duplicate Persoon: GastgezinId {gastgezin.Id}, PersoonId {gastgezin.Contact.Id}", DatabaseIntegrityLevel.Error);
                }
            }

            return result;
        }

        private DatabaseIntegrityTest TestDoubleAanmeldIntakeInGastgezinnen()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Duplicate Aanmeld / Intake",
                Description = "Checking all Gastgezin->Persoon->Reacties duplicates (Aanmeld formulier) and all Gastgezinnen->IntakeFormulierId (Intake formulier)"
            };


            var gastgezinnen = GastgezinRepo.GetAll("AanmeldFormulier");

            var duplicateAanmelding = from c in gastgezinnen
                                      group c by c.AanmeldFormulier into g
                                      where g.Skip(1).Any()
                                      from c in g
                                      select c;

            if (!duplicateAanmelding.Any())
            {
                result.AddMessage("No duplicaties for AanmeldFormulier for Gastgezinnen", DatabaseIntegrityLevel.Success);
            }
            else
            {
                foreach (var gastgezin in duplicateAanmelding)
                {
                    result.AddMessage($@"Duplicate Aanmeld: GastgezinId {gastgezin.Id}, AanmeldFormulierId {gastgezin.AanmeldFormulier.Id}", DatabaseIntegrityLevel.Error);
                }
            }

            var duplicateIntake = from c in gastgezinnen
                                  where c.IntakeFormulier != null
                                  group c by c.IntakeFormulier into g
                                  where g.Skip(1).Any()
                                  from c in g
                                  select c;

            if (duplicateIntake.Count() == 0)
            {
                result.AddMessage("No duplicaties for IntakeFormulier for Gastgezinnen", DatabaseIntegrityLevel.Success);
            }
            else
            {
                foreach (var gastgezin in duplicateIntake)
                {
                    result.AddMessage($@"Duplicate Intake: GastgezinId {gastgezin.Id}, IntakeId {gastgezin.IntakeFormulier.Id}", DatabaseIntegrityLevel.Error);
                }
            }

            return result;
        }

        //private DatabaseIntegrityTest TestMissingGastgezin()
        //{
        //    var result = new DatabaseIntegrityTest
        //    {
        //        Title = "Missing Gastgezin for Aanmeld Formulier",
        //        Description = "Checking all Gastgezin records missing for Persoon->Reacties where Reactie is AanmeldFormulier"
        //    };

        //    var gastgezinnen = GastgezinRepo.GetAll("Contact,AanmeldFormulier");
        //    var personen = PersoonRepo.GetMany(e => e.Reactie != null && e.Reactie.FormulierId == 1, "Reactie");

        //    int totalProblems = 0;
        //    foreach (var persoon in personen)
        //    {
        //        if (gastgezinnen.Any(e => e.Contact.Id == persoon.Id)) continue;

        //        totalProblems++;
        //        result.AddMessage($@"Gastgezin missing for: PersoonId {persoon.Id} ReactieId {persoon.Reactie.Id}", DatabaseIntegrityLevel.Error);
        //    }

        //    if (totalProblems == 0)
        //    {
        //        result.AddMessage($@"No Gastgezin found without a Persoon", DatabaseIntegrityLevel.Success);
        //    }

        //    return result;
        //}

        private DatabaseIntegrityTest TestMissingGastgezin()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Missing Gastgezin for Aanmeld Formulier",
                Description = "Checking id of all reactions of type 'AanmeldFormulier' for a matching id in Gastgezin.AanmeldFormulier"
            };

            var gastgezinnen = GastgezinRepo.GetAll("AanmeldFormulier");
            var aanmeldFormulieren = ReactionRepo.GetAll().Where(r => r.FormulierId == 1);

            int totalProblems = 0;
            foreach (var form in aanmeldFormulieren)
            {
                if (gastgezinnen.Any(e => e.AanmeldFormulier.Id == form.Id)) continue;

                totalProblems++;
                result.AddMessage($@"Gastgezin missing for: AanmeldformulierId {form.Id}", DatabaseIntegrityLevel.Error);
            }

            if (totalProblems == 0)
            {
                result.AddMessage($@"No Gastgezin found without a Persoon", DatabaseIntegrityLevel.Success);
            }

            return result;
        }

        private DatabaseIntegrityTest TestMissingAanmeldForIntakeFormulier()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Missing Aanmeld Formulier for Intake Formulier",
                Description = "Check if an Intake Formulier (Reactie) is not associated with an Aanmeld Formulier (Reactie)"
            };

            result.AddMessage("Not implemented yet");

            return result;
        }

        private DatabaseIntegrityTest TestMissingAanmeldForGastgezin()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Missing Aanmeld Formulier in Gastgezin Record",
                Description = "Check if an Gastgezin->AanmeldFormulier is not null"
            };

            var gastgezinnen = GastgezinRepo.GetMany(e => e.AanmeldFormulier == null, "AanmeldFormulier");

            foreach (var gastgezin in gastgezinnen)
            {
                result.AddMessage($@"Gastgezin missing Aanmeld: GastgezinId {gastgezin.Id}", DatabaseIntegrityLevel.Error);
            }

            if (!gastgezinnen.Any())
            {
                result.AddMessage("All Gastgezinnen have an AanmeldFormulierId", DatabaseIntegrityLevel.Success);
            }

            return result;
        }

        private DatabaseIntegrityTest TestCicleReference()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Check Gastgezin and Persoon Circle reference",
                Description = "Check if Gastgezin with Persoon do not have correct foreign keys to each other. They should both have the same keys towards each other."
            };

            var gastgezinnen = GastgezinRepo.GetAll();
            var personen = PersoonRepo.GetAll("Reactie,Gastgezin");

            var errorCount = 0;
            foreach (var gastgezin in gastgezinnen)
            {
                if (gastgezin.Contact == null)
                {
                    continue;
                }

                var correctPersoonId = gastgezin.Contact.Id;
                foreach (var persoon in personen.Where(e => e.Gastgezin != null && e.Gastgezin.Id == gastgezin.Id))
                {
                    if (persoon.Id != correctPersoonId)
                    {
                        result.AddMessage($@"Persoon has incorrect Gastgezin reference: PersoonId {persoon.Id} Incorrect GastgezinId {persoon.Gastgezin.Id}. Gastgezin is associated with PersoonId {correctPersoonId}", DatabaseIntegrityLevel.Error);
                        errorCount++;
                    }
                }
            }

            if (errorCount == 0)
            {
                result.AddMessage($@"No problems found", DatabaseIntegrityLevel.Success);
            }

            return result;
        }

        private DatabaseIntegrityTest TestPersonHasNoGastgezin()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Check Person has no gastgezin when connected to Aanmeld formulier",
                Description = "Check if Person->Gastgezin is null and with Person->Reactie->FormulierId is 1 (1 = Aanmeld formulier)."
            };

            var personen = PersoonRepo.GetAll("Reactie");

            var errorCount = 0;
            foreach (var persoon in personen.Where(e => e.Reactie != null && e.Reactie.Id == 1 && e.Gastgezin == null))
            {
                result.AddMessage($@"Persoon has no Gastgezin reference: PersoonId {persoon.Id}", DatabaseIntegrityLevel.Error);
                errorCount++;
            }

            if (errorCount == 0)
            {
                result.AddMessage($@"No problems found", DatabaseIntegrityLevel.Success);
            }

            return result;
        }

        private DatabaseIntegrityTest TestCountAllTables()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Count Table Records",
                Description = "Count all records for all tables in the database"
            };

            result.AddMessage($@"Table Adres: {AdresRepo.Count()} records");
            result.AddMessage($@"Table Antwoord: {AntwoordRepo.Count()} records");
            result.AddMessage($@"Table Gastgezin: {GastgezinRepo.Count()} records");
            result.AddMessage($@"Table Persoon: {PersoonRepo.Count()} records");
            result.AddMessage($@"Table Plaatsing: {PlaatsingsRepo.Count()} records");
            result.AddMessage($@"Table PlaatsingsInfo: {PlaatsingsInfoRepo.Count()} records");
            result.AddMessage($@"Table Reacties: {ReactionRepo.Count()} records");
            result.AddMessage($@"Table UsersDetails: {UserDetailRepo.Count()} records");

            return result;
        }

        private DatabaseIntegrityTest TestHowManyAanmeldDontHaveIntake()
        {
            var result = new DatabaseIntegrityTest
            {
                Title = "Number of Aanmeld which do not have an Intake",
                Description = "Check how many Aanmeld Formulieren do not have an Intake Formulier associated with them"
            };

            result.AddMessage("Not implemented yet");

            return result;
        }

        public List<MaintenanceMessage> LoadCapacityFromExcel(Stream excelStream)
        {
            var messages = new List<MaintenanceMessage>();
            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            var rowNum = 0;
            var gastgezinnen = _gastgezinService.GetAllGastgezinnen();
            foreach (var row in rows)
            {
                if (rowNum > 2)
                {
                    var index = 0;
                    var cells = row.Cells;
                    var done = false;
                    var gastgezin = new Gastgezin();
                    var maxChildren = 0;
                    var maxAdults = 0;

                    foreach (var cell in cells)
                    {
                        if (index == 0)
                        {
                            var intakeId = int.Parse(cell.ToString());
                            gastgezin = gastgezinnen.FirstOrDefault(g => g.IntakeFormulier != null && g.IntakeFormulier.Id == intakeId);
                            if (gastgezin == null)
                            {
                                messages.Add(new MaintenanceMessage($@"Gastgezin with intakeform {cell} not found", MaintenanceMessageType.Error));
                            }
                        }
                        if (gastgezin != null)
                        {
                            if (index == 1)
                            {
                                gastgezin.MaxYoungerThanThree = int.Parse(cell.ToString());
                            }
                            if (index == 2)
                            {
                                gastgezin.MaxOlderThanTwo = int.Parse(cell.ToString());
                            }
                        }
                        index++;
                    }
                    if (gastgezin != null)
                    {
                        _gastgezinService.UpdateGastgezin(gastgezin, gastgezin.Id);
                        messages.Add(new MaintenanceMessage($@"Max capacity updated to {gastgezin.MaxOlderThanTwo} adults and {gastgezin.MaxYoungerThanThree} children for Gastgezin with intakeform {gastgezin.IntakeFormulier.Id}", MaintenanceMessageType.Success));
                    }
                }
                rowNum++;
            }
            return messages;
        }

        public List<MaintenanceMessage> DuplicateComments()
        {
            var messages = new List<MaintenanceMessage>();
            var gastgezinnen = GastgezinRepo.GetAll().Where(g => g.VrijwilligerOpmerkingen != null && g.VrijwilligerOpmerkingen != "");
            foreach(var g in gastgezinnen)
            {
                if (g.CoordinatorOpmerkingen != g.VrijwilligerOpmerkingen)
                {
                    g.CoordinatorOpmerkingen = g.VrijwilligerOpmerkingen;
                    GastgezinRepo.Update(g);
                    messages.Add(new MaintenanceMessage($"CoordinatorOpmerking of gastgezin {g.Id} set to: '{g.CoordinatorOpmerkingen}'.", MaintenanceMessageType.Success));
                }
                else
                {
                    messages.Add(new MaintenanceMessage($"CoordinatorOpmerking of gastgezin {g.Id} already set to: '{g.CoordinatorOpmerkingen}'.", MaintenanceMessageType.Info));
                }
            }
            return messages;
        }
    }
}