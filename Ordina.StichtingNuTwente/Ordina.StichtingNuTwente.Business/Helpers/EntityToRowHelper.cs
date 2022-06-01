using FastExcel;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Helpers
{
    public class EntityToRowHelper
    {

        public static new List<Cell> GastgezinHeader()
        {
            var data = new List<Cell>();

            data.Add(new Cell(1, "Id"));
            data.Add(new Cell(2, "AanmeldFormulierId"));
            data.Add(new Cell(3, "IntakeFormulierId"));
            data.Add(new Cell(4, "ContactId"));
            data.Add(new Cell(5, "MaxVolwassenen"));
            data.Add(new Cell(6, "MaxKinderen"));
            data.Add(new Cell(7, "Status"));
            data.Add(new Cell(8, "HeeftVOG"));
            data.Add(new Cell(9, "Notitie"));
            data.Add(new Cell(10, "PlaatsingsInfoId"));
            data.Add(new Cell(11, "Intaker"));
            data.Add(new Cell(12, "BekekenDoorInaker"));
            data.Add(new Cell(13, "Buddy"));
            data.Add(new Cell(14, "BekekenDoorBuddy"));
            data.Add(new Cell(15, "VluchtelingIds"));
            data.Add(new Cell(16, "OpmerkingIds"));

            return data;
        }


        public static ICollection<Row> GastgezinToDataRow(List<Gastgezin> gastgezinnen)
        {
            var rows = new List<Row>();
            rows.Add(new Row(1, GastgezinHeader()));
            var number = 1;
            foreach(var gastgezin in gastgezinnen) {
                number++;
                var data = new List<Cell>();

                data.Add(new Cell(1, gastgezin.Id.ToString()));
                data.Add(new Cell(2, gastgezin.AanmeldFormulier != null ? gastgezin.AanmeldFormulier.Id.ToString() : ""));
                data.Add(new Cell(3, gastgezin.IntakeFormulier != null ? gastgezin.IntakeFormulier.Id.ToString() : ""));
                data.Add(new Cell(4, gastgezin.Contact != null ? gastgezin.Contact.Id.ToString() : ""));
                data.Add(new Cell(5, gastgezin.MaxAdults != null ? gastgezin.MaxAdults.ToString() : ""));
                data.Add(new Cell(6, gastgezin.MaxChildren != null ? gastgezin.MaxChildren.ToString() : ""));
                data.Add(new Cell(7, gastgezin.Status != null ? gastgezin.Status.ToString() : ""));
                data.Add(new Cell(8, gastgezin.HasVOG != null ? gastgezin.HasVOG.ToString() : ""));
                data.Add(new Cell(9, gastgezin.Note != null ? gastgezin.Note : ""));
                data.Add(new Cell(10, gastgezin.PlaatsingsInfo != null ? gastgezin.PlaatsingsInfo.Id.ToString() : ""));
                data.Add(new Cell(11, gastgezin.Begeleider != null ? gastgezin.Begeleider.FirstName : ""));
                data.Add(new Cell(12, gastgezin.BekekenDoorIntaker.ToString()));
                data.Add(new Cell(13, gastgezin.Buddy != null ? gastgezin.Buddy.FirstName : ""));
                data.Add(new Cell(14, gastgezin.BekekenDoorBuddy.ToString()));
                if (gastgezin.Vluchtelingen != null)
                {
                    var test = gastgezin.Plaatsingen.Select(v => v.Id.ToString());
                }

                data.Add(new Cell(15, gastgezin.Plaatsingen == null ? "" : String.Join(";", gastgezin.Plaatsingen.Select(v => v.Id.ToString()))));
                data.Add(new Cell(16, gastgezin.Comments == null ? "" : String.Join(";", gastgezin.Comments.Select(c => c.Id.ToString()))));
                rows.Add(new Row(number, data));
            }
            return rows;
        }
    }
}
