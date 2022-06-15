using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IDocumentService
    {
        public byte[] GenerateDataDumpToExcel();
        public byte[] GenerateGastgezinnenPerGemeente();
    }
}
