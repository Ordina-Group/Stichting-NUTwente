using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ordina.StichtingNuTwente.Models.ViewModels;
using MaintenanceMessage = Ordina.StichtingNuTwente.Business.Services.MaintenanceMessage;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IMaintenanceService
    {
        public List<MaintenanceMessage> LoadDataFromExcel(Stream excel, int formId);
        public List<MaintenanceMessage> LoadPlaatsingDataFromExcel(Stream excelStream, ClaimsPrincipal User);
        public List<MaintenanceMessage> UpdateDataFromExcel(Stream excelStream, int formId);
        public List<MaintenanceMessage> ImportGastgezinnen(Stream excelStream);
        public List<MaintenanceMessage> LinkBegeleiderToGastgezin();
        public List<MaintenanceMessage> UpdateAanmeldingFromIntakeId(Stream excelStream);
        DatabaseIntegrityModel TestDatabaseIntegrity();
        public List<MaintenanceMessage> LoadCapacityFromExcel(Stream excelStream);
        public byte[] GenerateDataDumpToExcel();
        public List<MaintenanceMessage> UpdateStatus();
    }
}
