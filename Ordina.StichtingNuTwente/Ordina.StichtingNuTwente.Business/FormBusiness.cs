using Newtonsoft.Json.Linq;
using Ordina.StichtingNuTwente.Entities;

namespace Ordina.StichtingNuTwente.Business
{
    public class FormBusiness : IFormBusiness
    {
        public Form createFormFromJson(int formId, string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            Form form = JObject.Parse(jsonString).ToObject<Form>();
            return form;
        }


    }
}