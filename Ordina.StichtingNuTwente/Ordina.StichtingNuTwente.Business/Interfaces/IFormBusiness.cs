using Ordina.StichtingNuTwente.Entities;
namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IFormBusiness
    {
        public Form createFormFromJson(int formId, string fileName);
    }
}