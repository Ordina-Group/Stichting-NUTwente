using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Mappings
{
    public static class ReactieMapping
    {
        public static Reactie FromWebToDatabaseModel(AnswersViewModel viewModel, Reactie dbModel = null)
        {
            if(dbModel== null)
            {
                dbModel = new Reactie();
            }
            int id = 0;
            int.TryParse(viewModel.Id, out id);
             
            dbModel.FormulierId = id;
            dbModel.DatumIngevuld = DateTime.Now;
            dbModel.Antwoorden = viewModel.answer.ConvertAll(a => AnswerMapping.FromWebToDatabaseModel(a));
            return dbModel;
                
        }

        public static AnswersViewModel FromDatabaseToWebModel(Reactie dbModel)
        {
            var webModel = new AnswersViewModel();
            webModel.Id = dbModel.FormulierId.ToString();
            webModel.AnswerDate = dbModel.DatumIngevuld;
            webModel.answer = dbModel.Antwoorden.ToList().ConvertAll(a => AnswerMapping.FromDatabaseToWebModel(a));
            return webModel;
        }

        public static AnswerListModel FromDatabaseToWebListModel(Reactie dbModel)
        {
            var webModel = new AnswerListModel();
            webModel.FormulierId = dbModel.FormulierId.ToString();
            webModel.AnswerDate = dbModel.DatumIngevuld;
            webModel.ReactieId = dbModel.Id;
            switch(webModel.FormulierId)
            {
                case "1":
                    webModel.FormulierNaam = "Aanmelden OPVANG in Twente";
                    break;
                case "2":
                    webModel.FormulierNaam = "Intake Gastgezin Oekraïne";
                    break;
                case "3":
                    webModel.FormulierNaam = "Intake Vluchteling Oekraïne";
                    break;
                default:
                    break;
            }
            return webModel;
        }
        
    }
}
