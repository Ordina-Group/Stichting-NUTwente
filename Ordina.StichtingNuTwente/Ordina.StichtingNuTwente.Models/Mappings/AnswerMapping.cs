using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Mappings
{
    public static class AnswerMapping
    {
        public static Antwoord FromWebToDatabaseModel(Answers viewModel, Antwoord dbModel = null)
        {
            if(dbModel== null)
            {
                dbModel = new Antwoord();
            }
            int id = 0;
            int.TryParse(viewModel.Nummer, out id);
            dbModel.IdVanVraag = id;
            dbModel.Response = viewModel.Antwoord;
            return dbModel;
                
        }
    }
}
