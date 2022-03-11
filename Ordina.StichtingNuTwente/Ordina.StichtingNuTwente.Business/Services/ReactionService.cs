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
            reactieRepository.Create(dbmodel);
            return true;
        }
    }
}
