using Newtonsoft.Json.Linq;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Entities;
using System.Text;

namespace Ordina.StichtingNuTwente.Business
{
    public class FormBusiness : IFormBusiness
    {
        public Form createFormFromJson(int formId, string fileName)
        {
            string jsonString = Encoding.UTF8.GetString(File.ReadAllBytes(fileName));
            Form form = JObject.Parse(jsonString).ToObject<Form>();
            return form;
        }


    }
}