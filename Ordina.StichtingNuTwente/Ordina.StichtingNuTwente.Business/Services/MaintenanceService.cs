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
using System.Diagnostics;
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
                if (rowNum > 2)
                {
                    var index = 0;
                    var cells = row.Cells;
                    var done = false;
                    Gastgezin gastgezin = new Gastgezin();
                    int adults;
                    int children;
                    int unknown;
                    foreach (var cell in cells)
                    {
                        if (index == 0)
                        {
                            gastgezin = gastgezinnen.FirstOrDefault(g => g.IntakeFormulier.Id == (int)cell.Value);
                        }
                        if (index == 8)
                        {
                            var val = cell.Value.ToString();
                            if (val != null && val != "")
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

        public void UpdateDataFromExcel(Stream excelStream, int formId)
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
                    Reactie? reaction = null;


                    var cells = row.Cells;

                    var index = 0;
                    foreach (var cell in cells)
                    {
                        if (index == 0)
                        {
                            var id = int.Parse(cell.ToString());
                            reaction = reactieRepository.GetById(id, "Antwoorden");
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
                    }
                }
                rowNum++;
            }
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
                        catch(FormatException)
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

