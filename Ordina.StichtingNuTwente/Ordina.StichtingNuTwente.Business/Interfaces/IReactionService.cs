using Ordina.StichtingNuTwente.Entities;
using Ordina.StichtingNuTwente.Models.ViewModels;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IReactionService
    {
        public bool Save(AnswersViewModel viewModel);
    }
}