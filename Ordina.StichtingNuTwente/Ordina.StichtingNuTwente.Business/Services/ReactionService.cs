using FastExcel;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Entities;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
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
            if (dbmodel.Id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Form GetAnwersFromId(int Id)
        {
            var viewModel = new Form();
            var reactieRepository = new Repository<Reactie>(_context);
            var dbModel =reactieRepository.GetById(Id, "Antwoorden");
            if(dbModel !=null)
            {
                var fileName = "";
                switch(dbModel.FormulierId)
                {
                    case 1:
                        fileName = "GastgezinAanmelding.json";
                        break;
                    case 2:
                        fileName = "GastgezinIntake.json";
                        break;
                    case 3:
                        fileName = "VluchtelingIntake.json";
                        break;
                }
                string jsonString = Encoding.UTF8.GetString(File.ReadAllBytes(fileName));
                viewModel = JObject.Parse(jsonString).ToObject<Form>();
                foreach(var section in viewModel.Sections)
                {
                    foreach(var question in section.Questions)
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

        public List<AnswerListModel> GetAllRespones(int? form= null)
        {
            List<AnswerListModel> viewModel= new List<AnswerListModel>();
            var reactieRepository = new Repository<Reactie>(_context);
            var dbItems = reactieRepository.GetAll(); 
            if (form !=null)
            {
                dbItems = dbItems.Where(f=> f.FormulierId== form.Value);
            }
            viewModel = dbItems.ToList().ConvertAll(r => ReactieMapping.FromDatabaseToWebListModel(r));
            return viewModel;
        }


        public byte[] GenerateExportCSV(int? formId =null)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            byte[] retVal = null;
            List<AnswerListModel> viewModel = new List<AnswerListModel>();
            var reactieRepository = new Repository<Reactie>(_context);
            var dbItems = reactieRepository.GetAll("Antwoorden").ToList();
            if (formId != null)
            {
                dbItems = dbItems.Where(f => f.FormulierId == formId.Value).ToList();
                var fileName = "";
                switch (formId.Value)
                {
                    case 1:
                        fileName = "GastgezinAanmelding.json";
                        break;
                    case 2:
                        fileName = "GastgezinIntake.json";
                        break;
                    case 3:
                        fileName = "VluchtelingIntake.json";
                        break;
                }
                string jsonString = Encoding.UTF8.GetString(File.ReadAllBytes(fileName));
                var form = JObject.Parse(jsonString).ToObject<Form>();
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
                        foreach (var section in form.Sections)
                        {
                            foreach (var question in section.Questions)
                            {
                                colum++;
                                cells.Add(new Cell(colum, question.Text));

                            }
                        }
                    }
                    else
                    {
                        cells.Add(new Cell(1, dbItems[dbitemIndex].Id));
                        cells.Add(new Cell(2, dbItems[dbitemIndex].DatumIngevuld));
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

    }
}
