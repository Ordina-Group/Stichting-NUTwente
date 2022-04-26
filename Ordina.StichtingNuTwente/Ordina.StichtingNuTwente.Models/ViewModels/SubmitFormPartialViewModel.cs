using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class SubmitFormPartialViewModel
    {
        public bool UpdateMode { get; set; }
        public int FormId { get; set; }
        public int? GastgezinId { get; set; }
    }
}
