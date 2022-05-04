using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class SearchQueryViewModel
    {
        public string Field { get; set; }
        public string SearchQuery { get; set; }
        public string OriginalQuery { get; set; }
        public int Results { get; set; }
    }
}
