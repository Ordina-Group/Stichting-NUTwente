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
    }
}
