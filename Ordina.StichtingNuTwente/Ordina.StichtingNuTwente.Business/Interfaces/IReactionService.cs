using Ordina.StichtingNuTwente.Models.ViewModels;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IReactionService
    {
        public bool Save(AnswersViewModel viewModel);
        public bool Delete(int reactionId);
        public void Update(AnswersViewModel viewModel, int id);

        public Form GetAnwersFromId(int Id);

        public List<AnswerListModel> GetAllRespones(int? form = null);

        public byte[] GenerateExportCSV(int? formId = null);
    }
}