using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
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

        public AnswersViewModel GetAnwersFromId(int Id)
        {
            var viewModel = new AnswersViewModel();
            var reactieRepository = new Repository<Reactie>(_context);
            var dbModel =reactieRepository.GetById(Id);
            if(dbModel !=null)
            {
                viewModel = ReactieMapping.FromDatabaseToWebModel(dbModel);
            }
            return viewModel;
        }
    }
}
