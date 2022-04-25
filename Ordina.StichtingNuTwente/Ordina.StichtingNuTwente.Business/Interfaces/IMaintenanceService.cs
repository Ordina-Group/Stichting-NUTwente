using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ordina.StichtingNuTwente.Business.Services;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IMaintenanceService
    {
        public void LoadDataFromExcel(Stream excel, int formId);
        void LoadPlaatsingDataFromExcel(Stream excelStream, ClaimsPrincipal User);
        public void UpdateDataFromExcel(Stream excelStream, int formId);
        List<MaintenanceMessage> ImportGastgezinnen(Stream excelStream);
    }
}
