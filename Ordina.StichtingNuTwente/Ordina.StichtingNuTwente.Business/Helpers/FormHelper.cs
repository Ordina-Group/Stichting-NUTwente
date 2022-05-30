using Newtonsoft.Json.Linq;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Helpers
{
    public static class FormHelper
    {

        public static string GetFilenameFromId(int id)
        {
            var fileName = "";
            switch (id)
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
                case 4:
                    fileName = "VrijwilligerAanmelding.json";
                    break;
            }
            return fileName;

        }

        public static Form GetFormFromFileId(int id)
        {
            var fileName = GetFilenameFromId(id);

            string jsonString = Encoding.UTF8.GetString(File.ReadAllBytes(fileName));
            return JObject.Parse(jsonString).ToObject<Form>();


        }
    }
}
