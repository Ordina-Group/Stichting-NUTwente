using Newtonsoft.Json.Linq;
using Ordina.StichtingNuTwente.Entities;

namespace Ordina.StichtingNuTwente.Business
{
    public class FormBusiness : IFormBusiness
    {
        public Form createFormFromJson(int formId)
        {
            string fileName = "forms.json";
            string jsonString = File.ReadAllText(fileName);
            var root = JObject.Parse(jsonString);
            Form form = root[formId-1].ToObject<Form>();
            return form;
        }
    }
}