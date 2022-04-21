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
    }
}

