using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IMaintenanceService
    {
        public void LoadDataFromExcel(Stream excel, int formId);
        void LoadPlaatsingDataFromExcel(Stream excelStream, ClaimsPrincipal User);
    }
}
