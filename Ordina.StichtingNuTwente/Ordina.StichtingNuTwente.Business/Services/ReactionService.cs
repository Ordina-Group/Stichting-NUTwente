using FastExcel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using Ordina.StichtingNuTwente.Business.Helpers;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Entities;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System.Reflection;
using System.Text;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class ReactionService : IReactionService
    {
        private readonly NuTwenteContext _context;
        public ReactionService(NuTwenteContext context)
        {
            _context = context;
        }

        public bool Save(AnswersViewModel viewModel)
        {
            var dbmodel = ReactieMapping.FromWebToDatabaseModel(viewModel);
            var reactieRepository = new Repository<Reactie>(_context);
            dbmodel = reactieRepository.Create(dbmodel);
            UpdateDatabaseWithRelationalObjects(viewModel, dbmodel);
            if (dbmodel.Id > 0)
            {
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

        public void UpdateDatabaseWithRelationalObjects(AnswersViewModel viewModel, Reactie reactie, int id = 0)
        {
            int formId = 0;
            int.TryParse(viewModel.Id, out formId);
            var form = FormHelper.GetFormFromFileId(formId);
            var PersoonRepo = new Repository<Persoon>(_context);
            var AdresRepo = new Repository<Adres>(_context);
            var dbPersoon = new Persoon();
            var dbAdres = new Adres();
            if (id!= 0)
            {
                dbPersoon = PersoonRepo.GetFirstOrDefault(p => p.Reactie != null && p.Reactie.Id == id, "Reactie") ;
                dbPersoon = dbPersoon?? new Persoon();
                dbAdres = AdresRepo.GetFirstOrDefault(p => p.Reactie != null && p.Reactie.Id == id, "Reactie");
                dbAdres = dbAdres ?? new Adres();
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
            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "Persoon")))
            {
                dbPersoon = CreateDbObjectFromFormFilledWithAnswers<Persoon>(form, viewModel, dbPersoon);
                if (dbPersoon != null)
                {
                    if (dbPersoon.Id == 0)
                    {
                        if(dbAdres !=null)
                        {
                            dbPersoon.Adres = dbAdres;
                        }    
                        dbPersoon.Reactie = reactie;
                        PersoonRepo.Create(dbPersoon);
                    }
                    else
                    {
                        PersoonRepo.Update(dbPersoon);
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
                        if (answer != null && !string.IsNullOrEmpty(answer.Antwoord) && q.ParameterName == prop.Name)
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

        public List<AnswerListModel> GetAllRespones(int? form = null)
        {
            List<AnswerListModel> viewModel = new List<AnswerListModel>();
            var reactieRepository = new Repository<Reactie>(_context);
            var dbItems = reactieRepository.GetAll();
            if (form != null)
            {
                dbItems = dbItems.Where(f => f.FormulierId == form.Value);
            }
            viewModel = dbItems.ToList().ConvertAll(r => ReactieMapping.FromDatabaseToWebListModel(r));
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
                        var QuestionsOrderedByID = questions.OrderBy(q => q.Id);
                        foreach(var question in QuestionsOrderedByID)
                        {
                            colum++;
                            cells.Add(new Cell(colum, question.Text));
                        }

                    }
                    else
                    {
                        cells.Add(new Cell(1, dbItems[dbitemIndex].Id));
                        cells.Add(new Cell(2, (dbItems[dbitemIndex].DatumIngevuld).ToString("dd/MM/yyyy HH:mm:ss")));
                        var colum = 2;

                        foreach (var antwoord in dbItems[dbitemIndex].Antwoorden)
                        {
                            colum++;
                            cells.Add(new Cell(colum, antwoord.Response));
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
            var existingReaction = reactieRepository.GetById(reactionId, "Antwoorden");

            foreach(var antwoord in existingReaction.Antwoorden)
            {
                awnserRepository.Delete(antwoord);
            }
            reactieRepository.Delete(existingReaction);

            return reactieRepository.GetById(reactionId) == null;
        }
    }
}
