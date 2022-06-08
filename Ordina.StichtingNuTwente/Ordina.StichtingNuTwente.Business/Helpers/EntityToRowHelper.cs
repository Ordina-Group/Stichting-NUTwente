using FastExcel;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
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
            data.Add(new Cell(17, "Verwijderd"));
            data.Add(new Cell(18, "NoodOpvang"));
            data.Add(new Cell(19, "OnHold"));
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
                data.Add(new Cell(7, gastgezin.GetStatus().ToString()));
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
                data.Add(new Cell(17, gastgezin.Deleted));
                data.Add(new Cell(18, gastgezin.NoodOpvang));
                data.Add(new Cell(19, gastgezin.OnHold));
                rows.Add(new Row(number, data));
            }
            return rows;
        }


        public static ICollection<Row> ReactiesToDataRows(List<Reactie> reacties, int formId)
        {
            var rows = new List<Row>();

            var form = FormHelper.GetFormFromFileId(formId);
            var header = new List<Cell>();
            var offset = 4;
            header.Add(new Cell(1, "Id"));
            header.Add(new Cell(2, "Datum"));
            header.Add(new Cell(3, "Verwijderd"));
            header.Add(new Cell(4, "Comments"));
            var questions = new List<Question>();
            foreach (var section in form.Sections)
            {
                foreach(var question in section.Questions)
                {
                    questions.Add(question);
                }
            }
            questions = questions.OrderBy(x => x.Id).ToList(); 
            foreach(var question in questions)
            {
                header.Add(new Cell(question.Id + offset, question.Text));
            }
            rows.Add(new Row(1, header));
            var number = 2;
            foreach (var reactie in reacties)
            {
                var data = new List<Cell>();
                data.Add(new Cell(1, reactie.Id.ToString()));
                data.Add(new Cell(2, reactie.DatumIngevuld.ToString()));
                data.Add(new Cell(3, reactie.Deleted.ToString()));
                data.Add(new Cell(4, reactie.Comments == null ? "" : String.Join(";", reactie.Comments.Select(c => c.Id.ToString()))));
                var antwoorden = reactie.Antwoorden.OrderBy(x => x.IdVanVraag).ToList();
                foreach (var awnser in antwoorden)
                {
                    data.Add(new Cell(awnser.IdVanVraag + offset, awnser.Response));

                }
                rows.Add(new Row(number, data));
                number++;
            }
            return rows;
        }
    }
}
