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

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly NuTwenteContext _context;
        private readonly IFormBusiness _formBusiness;
        private readonly IGastgezinService _gastgezinService;
        private readonly IUserService _userService;
        public MaintenanceService(NuTwenteContext context, IFormBusiness formBusiness, IGastgezinService gastgezinService, IUserService userService)
        {
            _context = context;
            _formBusiness = formBusiness;
            _gastgezinService = gastgezinService;
            _userService = userService;
        }


        public List<MaintenanceMessage> LinkBegeleiderToGastgezin()
        {
            var messages = new List<MaintenanceMessage>();
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            var gastgezinnen = gastgezinRepository.GetAll("IntakeFormulier.UserDetails,Begeleider");
            foreach (var gastgezin in gastgezinnen)
            {
                if (gastgezin.IntakeFormulier != null && gastgezin.IntakeFormulier.UserDetails != null && gastgezin.Begeleider == null)
                {
                    var userDetails = gastgezin.IntakeFormulier.UserDetails;
                    gastgezin.Begeleider = userDetails;
                    gastgezinRepository.Update(gastgezin);
                    messages.Add(new MaintenanceMessage($@"Gastgezin with id {gastgezin.Id} got linked with {userDetails.FirstName} {userDetails.LastName}", MaintenanceMessageType.Success));
                }
                else
                {
                    messages.Add(new MaintenanceMessage($@"Gastgezin with id {gastgezin.Id} did not change"));
                }
            }
            return messages;
        }

        public void LoadDataFromExcel(Stream excelStream, int formId)
        {
            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            var rowNum = 0;
            var reactieRepository = new Repository<Reactie>(_context);
            string file = FormHelper.GetFilenameFromId(formId);
            Form questionForm = _formBusiness.createFormFromJson(1, file);
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

                    var created = reactieRepository.Create(reaction);
                }
                rowNum++;
            }
        }

        public void LoadPlaatsingDataFromExcel(Stream excelStream, ClaimsPrincipal User)
        {
            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            var rowNum = 0;
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            var gastgezinnen = gastgezinRepository.GetAll("IntakeFormulier");
            foreach (var row in rows)
            {
                if (rowNum > 1)
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
                            gastgezin = gastgezinnen.FirstOrDefault(g => g.IntakeFormulier.Id == (int)cell.Value);
                        }
                        if (index == 1)
                        {
                            var val = cell.Value.ToString();
                            if (val.Contains("on hold"))
                            {
                                gastgezin.Status = GastgezinStatus.OnHold;
                                _gastgezinService.UpdateGastgezin(gastgezin,gastgezin.Id);
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
                                if (val.Contains("v"))
                                {
                                    var plaatsing = new Plaatsing()
                                    {
                                        DateTime = DateTime.Now,
                                        AgeGroup = AgeGroup.Volwassene,
                                        Amount = val[val.IndexOf("v") + 1],
                                        Gastgezin = gastgezin,
                                        PlacementType = PlacementType.Plaatsing,
                                        Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                                    };
                                    _gastgezinService.AddPlaatsing(plaatsing);
                                }
                                if (val.Contains("k"))
                                {
                                    var plaatsing = new Plaatsing()
                                    {
                                        DateTime = DateTime.Now,
                                        AgeGroup = AgeGroup.Kind,
                                        Amount = val[val.IndexOf("k") + 1],
                                        Gastgezin = gastgezin,
                                        PlacementType = PlacementType.Plaatsing,
                                        Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                                    };
                                    _gastgezinService.AddPlaatsing(plaatsing);
                                }
                                if (!val.Contains("v") && !val.Contains("k"))
                                {
                                    var plaatsing = new Plaatsing()
                                    {
                                        DateTime = DateTime.Now,
                                        AgeGroup = AgeGroup.Onbekend,
                                        Amount = int.Parse(val),
                                        Gastgezin = gastgezin,
                                        PlacementType = PlacementType.Plaatsing,
                                        Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                                    };
                                    _gastgezinService.AddPlaatsing(plaatsing);
                                }
                            }
                        }
                        index++;
                    };
                }
                rowNum++;
            }
        }

        public List<MaintenanceMessage> UpdateDataFromExcel(Stream excelStream, int formId)
        {
            var messages = new List<MaintenanceMessage>();


            using FastExcel.FastExcel fastExcel = new(excelStream);
            var worksheet = fastExcel.Worksheets[0];
            worksheet.Read();
            var rows = worksheet.Rows.ToArray();
            var rowNum = 0;
            var reactieRepository = new Repository<Reactie>(_context);
            string file = FormHelper.GetFilenameFromId(formId);
            Form questionForm = _formBusiness.createFormFromJson(formId, file);
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
                            reaction = reactieRepository.GetById(id, "Antwoorden");
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
                        reactieRepository.Update(reaction);
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

            var reactieRepository = new Repository<Reactie>(_context);
            var gastgezinRespority = new Repository<Gastgezin>(_context);
            var persoonRepository = new Repository<Persoon>(_context);

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

                    var persoon = persoonRepository.Get(e => e.Reactie.Id == aanmeldId, "Gastgezin");
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
                        Contact = persoon,
                        Status = GastgezinStatus.Bezocht
                    };

                    if (intakeId.HasValue)
                    {
                        gastgezin.IntakeFormulier = reactieRepository.GetById((intakeId.Value));

                        if (gastgezin.IntakeFormulier == null)
                        {
                            messages.Add(new MaintenanceMessage($@"Aanmeld Id: {aanmeldId} Intake Id: {intakeId} - Intake formulier was niet gevonden. Gastgezin niet toegevoed", MaintenanceMessageType.Error));
                            continue;
                        }
                    }

                    gastgezinRespority.Create(gastgezin);

                    persoon.Gastgezin = gastgezin;
                    persoonRepository.Update(persoon);

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

            messages.Add(new MaintenanceMessage($@"Gastgezinnen total in database: {gastgezinRespority.GetAll().Count()}", MaintenanceMessageType.Error));

            return messages;
        }
    }

    public class MaintenanceMessage
    {
        public MaintenanceMessage(string message, MaintenanceMessageType messageType = MaintenanceMessageType.Info)
        {
            Message = message;
            MessageType = messageType;
        }

        public string Message { get; set; }

        public MaintenanceMessageType MessageType { get; set; }
    }

    public enum MaintenanceMessageType
    {
        Info,
        Warning,
        Error,
        Success
    }
}

