using FastExcel;
using OfficeOpenXml;
using Ordina.StichtingNuTwente.Business.Helpers;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System.Reflection;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class ReactionService : IReactionService
    {
        private readonly NuTwenteContext _context;

        public ReactionService(NuTwenteContext context)
        {
            _context = context;
        }

        public bool Save(AnswersViewModel viewModel, int? gastgezinId)
        {
            var dbmodel = ReactieMapping.FromWebToDatabaseModel(viewModel);
            var reactieRepository = new Repository<Reactie>(_context);
            dbmodel = reactieRepository.Create(dbmodel);
            UpdateDatabaseWithRelationalObjects(viewModel, dbmodel);
            if (dbmodel.Id > 0)
            {
                if (gastgezinId != null)
                {
                    var gastgezinRepository = new Repository<Gastgezin>(_context);
                    var gastgezin = gastgezinRepository.GetById(gastgezinId.Value);
                    if (gastgezin != null)
                    {
                        gastgezin.IntakeFormulier = dbmodel;
                        if (gastgezin.Status == GastgezinStatus.Aangemeld)
                        {
                            gastgezin.Status = GastgezinStatus.Bezocht;
                            var persoonRepository = new Repository<Persoon>(_context);
                            var persoon = persoonRepository.Get(x => x.Reactie != null && x.Reactie.Id == dbmodel.Id, "Reactie");
                            if (persoon != null)
                                gastgezin.Contact = persoon;
                        }
                        gastgezinRepository.Update(gastgezin);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(AnswersViewModel viewModel, int id)
        {
            var dbmodel = ReactieMapping.FromWebToDatabaseModel(viewModel);
            dbmodel.Id = id;
            var reactieRepository = new Repository<Reactie>(_context);
            var existingReaction = reactieRepository.GetById(id, "Antwoorden");
            foreach (var antwoord in dbmodel.Antwoorden)
            {
                var dbAntwoord = existingReaction.Antwoorden.SingleOrDefault(a => a.IdVanVraag == antwoord.IdVanVraag);
                if (dbAntwoord != null)
                {
                    dbAntwoord.Response = antwoord.Response;
                }
                else
                {
                    existingReaction.Antwoorden.Add(antwoord);
                }
            }

            reactieRepository.Update(existingReaction);
            UpdateDatabaseWithRelationalObjects(viewModel, existingReaction, id);
        }

        public void UpdateAll(int? id = 0)
        {
            var reactieRepository = new Repository<Reactie>(_context);
            var reacties = reactieRepository.GetAll("Antwoorden");
            if (id != 0 && id != null)
            {
                reacties = reacties.Where(r => r.FormulierId == id);
            }
            foreach (var reactie in reacties)
            {
                var viewModel = ReactieMapping.FromDatabaseToWebModel(reactie);
                try
                {
                    UpdateDatabaseWithRelationalObjects(viewModel, reactie, reactie.Id);
                }
                catch (Exception) { }
            }
        }

        public void UpdateDatabaseWithRelationalObjects(AnswersViewModel viewModel, Reactie reactie, int id = 0)
        {
            int formId = 0;
            int.TryParse(viewModel.Id, out formId);
            var form = FormHelper.GetFormFromFileId(formId);
            var PersoonRepo = new Repository<Persoon>(_context);
            var AdresRepo = new Repository<Adres>(_context);
            var UserRepo = new Repository<UserDetails>(_context);
            var gastgezinRepo = new Repository<Gastgezin>(_context);
            var plaatsingsInfoRepo = new Repository<PlaatsingsInfo>(_context);
            var dbPersoon = new Persoon();
            var dbAdres = new Adres();
            var dbUser = new UserDetails();
            var dbPlaatsingsInfo = new PlaatsingsInfo();
            if (id != 0)
            {
                dbPersoon = PersoonRepo.GetFirstOrDefault(p => p.Reactie != null && p.Reactie.Id == id, "Reactie");
                dbPersoon = dbPersoon ?? new Persoon();
                dbAdres = AdresRepo.GetFirstOrDefault(p => p.Reactie != null && p.Reactie.Id == id, "Reactie");
                dbAdres = dbAdres ?? new Adres();
                dbPlaatsingsInfo = plaatsingsInfoRepo.GetFirstOrDefault(p => p.Reactie != null && p.Reactie.Id == id, "Reactie") ?? new PlaatsingsInfo();
            }

            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "Adres")))
            {
                dbAdres = CreateDbObjectFromFormFilledWithAnswers<Adres>(form, viewModel, dbAdres);
                if (dbAdres != null)
                {
                    if (dbAdres.Id == 0)
                    {
                        dbAdres.Reactie = reactie;
                        AdresRepo.Create(dbAdres);
                    }
                    else
                    {
                        AdresRepo.Update(dbAdres);
                    }
                }
            }
            //attach user
            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "UserDetails")))
            {
                var questionId = form.Sections.FirstOrDefault(s => s.Questions.Any(q => q.Object == "UserDetails")).Questions.FirstOrDefault(q => q.Object == "UserDetails").Id;
                if (viewModel.answer.Any())
                {
                    var awnser = viewModel.answer.FirstOrDefault(a => a.Nummer.Trim() == questionId.ToString());
                    if (awnser != null)
                    {
                        var userNameAndEmail = awnser.Antwoord;
                        if (userNameAndEmail.Contains("(") && userNameAndEmail.Contains(")"))
                        {
                            var email = userNameAndEmail.Split("(")[1].Split(")")[0];
                            dbUser = UserRepo.GetFirstOrDefault(u => u.Email.Contains(email));
                            if (dbUser != null)
                            {
                                if (dbUser.Reacties != null)
                                {
                                    dbUser.Reacties.Add(reactie);
                                }
                                else
                                {
                                    dbUser.Reacties = new List<Reactie>() { reactie };
                                }
                                UserRepo.Update(dbUser);
                            }
                        }
                    }
                }
            }
            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "Persoon")))
            {
                dbPersoon = CreateDbObjectFromFormFilledWithAnswers<Persoon>(form, viewModel, dbPersoon);
                if (dbPersoon != null)
                {
                    if (dbPersoon.Id == 0)
                    {
                        if (dbAdres != null)
                        {
                            dbPersoon.Adres = dbAdres;
                        }
                        dbPersoon.Reactie = reactie;
                        PersoonRepo.Create(dbPersoon);
                    }
                    else
                    {
                        PersoonRepo.Update(dbPersoon);
                        if (form.Id == 2)
                        {
                            var gastgezin = gastgezinRepo.GetFirstOrDefault(g => g.IntakeFormulier != null && g.IntakeFormulier.Id == reactie.Id, "IntakeFormulier");
                            if (gastgezin != null)
                            {
                                gastgezin.Contact = dbPersoon;
                                gastgezinRepo.Update(gastgezin);
                            }
                        }
                    }
                }
            }
            //Formulier: Gastgezin aanmelden
            if (form.Id == 1 && id == 0)
            {
                var gastgezin = new Gastgezin
                {
                    AanmeldFormulier = reactie,
                    Contact = dbPersoon,
                    Status = (int)GastgezinStatus.Aangemeld,
                };

                gastgezinRepo.Create(gastgezin);
            }

            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "PlaatsingsInfo")))
            {
                dbPlaatsingsInfo = CreateDbObjectFromFormFilledWithAnswers<PlaatsingsInfo>(form, viewModel, dbPlaatsingsInfo);
                if (dbPlaatsingsInfo != null)
                {
                    if (dbPlaatsingsInfo.Id == 0)
                    {

                        var gastgezin = gastgezinRepo.GetFirstOrDefault(g => g.IntakeFormulier != null && g.IntakeFormulier.Id == reactie.Id, "IntakeFormulier");
                        if (gastgezin != null)
                        {
                            dbPlaatsingsInfo.Reactie = reactie;
                            dbPlaatsingsInfo = plaatsingsInfoRepo.Create(dbPlaatsingsInfo);
                            if (dbPlaatsingsInfo.Id != 0)
                            {
                                gastgezin.PlaatsingsInfo = dbPlaatsingsInfo;
                                gastgezinRepo.Update(gastgezin);
                            }
                        }
                    }
                    else
                    {
                        plaatsingsInfoRepo.Update(dbPlaatsingsInfo);
                        var gastgezin = gastgezinRepo.GetFirstOrDefault(g => g.IntakeFormulier != null && g.IntakeFormulier.Id == reactie.Id, "IntakeFormulier,PlaatsingsInfo");
                        if(gastgezin.PlaatsingsInfo == null)
                        {
                            gastgezin.PlaatsingsInfo = dbPlaatsingsInfo;
                            gastgezinRepo.Update(gastgezin);
                        }
                    }
                }
            }
        }

        private T CreateDbObjectFromFormFilledWithAnswers<T>(Form form, AnswersViewModel viewModel, T classObject)
        {
            var typeObject = typeof(T);
            foreach (var prop in typeObject.GetProperties())
            {
                foreach (var s in form.Sections)
                {
                    foreach (var q in s.Questions)
                    {
                        var answer = viewModel.answer.FirstOrDefault(a => a.Nummer.Trim() == q.Id.ToString());
                        if (answer != null && !string.IsNullOrEmpty(answer.Antwoord) && q.ParameterName == prop.Name && q.Object == typeObject.Name)
                        {
                            PropertyInfo propInfo = typeObject.GetProperty(prop.Name);
                            if (propInfo != null)
                            {
                                object value = null;
                                switch (prop.PropertyType.Name.ToLower())
                                {
                                    case "string":
                                        value = answer.Antwoord;
                                        break;

                                    case "boolean":
                                        value = Convert.ToBoolean(answer.Antwoord);
                                        break;

                                    case "datetime":
                                        value = Convert.ToDateTime(answer.Antwoord);
                                        break;

                                    case "int32":
                                        value = Convert.ToInt32(answer.Antwoord);
                                        break;

                                    case "decimal":
                                        value = Convert.ToDecimal(answer.Antwoord);
                                        break;

                                    default:
                                        value = answer.Antwoord;
                                        break;
                                }
                                propInfo.SetValue(classObject, value);
                            }
                        }
                    }
                }
            }
            return classObject;
        }

        public Form GetAnwersFromId(int Id)
        {
            var viewModel = new Form();
            var reactieRepository = new Repository<Reactie>(_context);
            var dbModel = reactieRepository.GetById(Id, "Antwoorden");
            if (dbModel != null)
            {
                viewModel = FormHelper.GetFormFromFileId(dbModel.FormulierId);
                foreach (var section in viewModel.Sections)
                {
                    foreach (var question in section.Questions)
                    {
                        var antwoord = dbModel.Antwoorden.FirstOrDefault(a => a.IdVanVraag == question.Id);
                        if (antwoord != null)
                        {
                            question.Answer = antwoord.Response;
                        }
                    }
                }
            }
            return viewModel;
        }

        public Reactie GetReactieFromId(int Id)
        {
            var reactieRepository = new Repository<Reactie>(_context);
            return reactieRepository.GetById(Id, "UserDetails");
        }

        public List<AnswerListModel> GetAllRespones(int? form = null)
        {
            List<AnswerListModel> viewModel = new List<AnswerListModel>();
            var reactieRepository = new Repository<Reactie>(_context);
            var persoonRepository = new Repository<Persoon>(_context);
            var reacties = reactieRepository.GetAll();
            var people = persoonRepository.GetAll("Reactie,Adres");
            if (form != null)
            {
                reacties = reacties.Where(f => f.FormulierId == form.Value);
            }
            var t = from reactie in reacties
                    join add in people
                    on reactie.Id equals add.Reactie?.Id
                    into PeopleReaction
                    from persoon in PeopleReaction.DefaultIfEmpty()
                    select new { reactie, persoon };

            viewModel = t.OrderByDescending(x => x.reactie.Id).ToList().ConvertAll(p =>
            {
                var awnser = ReactieMapping.FromDatabaseToWebListModel(p.reactie);
                awnser.Persoon = p.persoon;
                return awnser;
            });
            return viewModel;
        }

        public byte[] GenerateExportCSV(int? formId = null)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            byte[] retVal = null;
            List<AnswerListModel> viewModel = new List<AnswerListModel>();
            var reactieRepository = new Repository<Reactie>(_context);
            var dbItems = reactieRepository.GetAll("Antwoorden").ToList();
            if (formId != null)
            {
                dbItems = dbItems.Where(f => f.FormulierId == formId.Value).ToList();
                var form = FormHelper.GetFormFromFileId(formId.Value);
                var templateFile = new FileInfo("template.xlsx");
                var outputFile = new FileInfo("output.xlsx");

                //Create a worksheet with some rows
                var worksheet = new Worksheet();
                var rows = new List<Row>();

                var totalRows = dbItems.Count() + 1;
                var dbitemIndex = 0;
                for (int rowNumber = 1; rowNumber <= totalRows; rowNumber++)
                {
                    List<Cell> cells = new List<Cell>();
                    if (rowNumber == 1)
                    {
                        cells.Add(new Cell(1, "ReactieId"));
                        cells.Add(new Cell(2, "Datum ingevuld"));
                        var colum = 2;
                        var questions = new List<Question>();
                        foreach (var section in form.Sections)
                        {
                            foreach (var question in section.Questions)
                            {
                                questions.Add(question);
                            }
                        }
                        var questionsOrderedById = questions.OrderBy(q => q.Id).ToList();
                        for (int i = 1; i <= questionsOrderedById.MaxBy(q => q.Id).Id; i++)
                        {
                            var question = questionsOrderedById.FirstOrDefault(q => q.Id == i);
                            if (question != null)
                            {
                                colum++;
                                cells.Add(new Cell(colum, question.Text));
                            }
                            else
                            {
                                colum++;
                                cells.Add(new Cell(colum, ""));
                            }
                        }
                    }
                    else
                    {
                        cells.Add(new Cell(1, dbItems[dbitemIndex].Id));
                        cells.Add(new Cell(2, (dbItems[dbitemIndex].DatumIngevuld).ToString("dd/MM/yyyy HH:mm:ss")));
                        var colum = 2;
                        var answersOrderedById = dbItems[dbitemIndex].Antwoorden.OrderBy(a => a.IdVanVraag).ToList();
                        var maxQuestionId = 0;
                        if (answersOrderedById.Count > 0)
                        {
                            var maxAwnser = answersOrderedById.MaxBy(a => a.IdVanVraag);
                            if (maxAwnser != null)
                                maxQuestionId = maxAwnser.IdVanVraag;
                        }
                        for (int i = 1; i <= maxQuestionId; i++)
                        {
                            var awnser = answersOrderedById.FirstOrDefault(a => a.IdVanVraag == i);
                            if (awnser != null)
                            {
                                colum++;
                                cells.Add(new Cell(colum, awnser.Response));
                            }
                            else
                            {
                                colum++;
                                cells.Add(new Cell(colum, ""));
                            }
                        }
                        dbitemIndex++;
                    }
                    rows.Add(new Row(rowNumber, cells));
                }
                worksheet.Rows = rows;

                // Create an instance of FastExcel
                using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(templateFile, outputFile))
                {
                    // Write the data
                    fastExcel.Write(worksheet, "sheet1");
                }
                using (var filestream = outputFile.OpenRead())
                {
                    BinaryReader br = new BinaryReader(filestream);
                    long numBytes = new FileInfo(outputFile.Name).Length;
                    retVal = br.ReadBytes((int)numBytes);
                }
                outputFile.Delete();
            }

            return retVal;
        }

        public bool Delete(int reactionId)
        {
            var reactieRepository = new Repository<Reactie>(_context);
            var awnserRepository = new Repository<Antwoord>(_context);
            var personRepository = new Repository<Persoon>(_context);
            var adresRepository = new Repository<Adres>(_context);
            var existingReaction = reactieRepository.GetById(reactionId, "Antwoorden");

            foreach (var antwoord in existingReaction.Antwoorden)
            {
                awnserRepository.Delete(antwoord);
            }

            var person = personRepository.GetAll("Reactie").FirstOrDefault(r => r.Reactie != null && r.Reactie.Id == reactionId);
            if (person != null)
            {
                personRepository.Delete(person);
            }

            var adres = adresRepository.GetAll("Reactie").FirstOrDefault(a => a.Reactie != null && a.Reactie.Id == reactionId);
            if (adres != null)
                adresRepository.Delete(adres);

            reactieRepository.Delete(existingReaction);

            return reactieRepository.GetById(reactionId) == null;
        }
    }
}