using Newtonsoft.Json.Linq;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Entities;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
