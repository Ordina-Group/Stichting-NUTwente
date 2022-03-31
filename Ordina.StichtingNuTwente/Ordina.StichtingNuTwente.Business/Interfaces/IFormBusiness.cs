using Ordina.StichtingNuTwente.Models.ViewModels;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IFormBusiness
    {
        public Form createFormFromJson(int formId, string fileName);
    }
}