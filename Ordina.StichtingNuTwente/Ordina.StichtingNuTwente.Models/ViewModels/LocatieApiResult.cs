using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class LocatieApiResult
    {
        public LocatieApiResponse Response { get; set; }
    }

    public class LocatieApiResponse
    {
        public List<LocatieApiDoc> Docs { get; set; }
    }

    public class LocatieApiDoc
    {
        public string Gemeentenaam { get; set; }
    }
}
