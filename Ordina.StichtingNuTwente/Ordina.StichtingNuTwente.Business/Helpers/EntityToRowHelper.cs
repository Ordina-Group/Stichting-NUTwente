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
            var cell = 1;
            data.Add(new Cell(cell++, "Id"));
            data.Add(new Cell(cell++, "Verwijderd"));
            data.Add(new Cell(cell++, "AanmeldFormulierId"));
            data.Add(new Cell(cell++, "IntakeFormulierId"));
            data.Add(new Cell(cell++, "ContactId"));
            data.Add(new Cell(cell++, "MaxVolwassenen"));
            data.Add(new Cell(cell++, "MaxKinderen"));
            data.Add(new Cell(cell++, "Status"));
            data.Add(new Cell(cell++, "HeeftVOG"));
            data.Add(new Cell(cell++, "Notitie"));
            data.Add(new Cell(cell++, "PlaatsingsInfoId"));
            data.Add(new Cell(cell++, "Intaker"));
            data.Add(new Cell(cell++, "BekekenDoorInaker"));
            data.Add(new Cell(cell++, "Buddy"));
            data.Add(new Cell(cell++, "BekekenDoorBuddy"));
            data.Add(new Cell(cell++, "PlaatsingIds"));
            data.Add(new Cell(cell++, "OpmerkingIds"));
            data.Add(new Cell(cell++, "NoodOpvang"));
            data.Add(new Cell(cell++, "OnHold"));
            data.Add(new Cell(cell++, "VrijwilligerOpmerking"));
            data.Add(new Cell(cell++, "ContactLogs"));
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
                var cell = 1;
                data.Add(new Cell(cell++, gastgezin.Id.ToString()));
                data.Add(new Cell(cell++, gastgezin.Deleted));
                data.Add(new Cell(cell++, gastgezin.AanmeldFormulier != null ? gastgezin.AanmeldFormulier.Id.ToString() : ""));
                data.Add(new Cell(cell++, gastgezin.IntakeFormulier != null ? gastgezin.IntakeFormulier.Id.ToString() : ""));
                data.Add(new Cell(cell++, gastgezin.Contact != null ? gastgezin.Contact.Id.ToString() : ""));
                data.Add(new Cell(cell++, gastgezin.MaxAdults != null ? gastgezin.MaxAdults.ToString() : ""));
                data.Add(new Cell(cell++, gastgezin.MaxChildren != null ? gastgezin.MaxChildren.ToString() : ""));
                data.Add(new Cell(cell++, gastgezin.GetStatus().ToString()));
                data.Add(new Cell(cell++, gastgezin.HasVOG != null ? gastgezin.HasVOG.ToString() : ""));
                data.Add(new Cell(cell++, gastgezin.Note != null ? gastgezin.Note : ""));
                data.Add(new Cell(cell++, gastgezin.PlaatsingsInfo != null ? gastgezin.PlaatsingsInfo.Id.ToString() : ""));
                data.Add(new Cell(cell++, gastgezin.Begeleider != null ? gastgezin.Begeleider.FirstName : ""));
                data.Add(new Cell(cell++, gastgezin.BekekenDoorIntaker.ToString()));
                data.Add(new Cell(cell++, gastgezin.Buddy != null ? gastgezin.Buddy.FirstName : ""));
                data.Add(new Cell(cell++, gastgezin.BekekenDoorBuddy.ToString()));
                if (gastgezin.Vluchtelingen != null)
                {
                    var test = gastgezin.Plaatsingen.Select(v => v.Id.ToString());
                }

                data.Add(new Cell(cell++, gastgezin.Plaatsingen == null ? "" : String.Join(";", gastgezin.Plaatsingen.Select(v => v.Id.ToString()))));
                data.Add(new Cell(cell++, gastgezin.Comments == null ? "" : String.Join(";", gastgezin.Comments.Select(c => c.Id.ToString()))));
                data.Add(new Cell(cell++, gastgezin.NoodOpvang));
                data.Add(new Cell(cell++, gastgezin.OnHold));
                data.Add(new Cell(cell++, gastgezin.VrijwilligerOpmerkingen));
                data.Add(new Cell(cell++, String.Join(";", gastgezin.ContactLogs.Select(c => c.Id.ToString()))));
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
            var row = 2;
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
                rows.Add(new Row(row, data));
                row++;
            }
            return rows;
        }

        public static ICollection<Row> PlaatsingenToDataRows(List<Plaatsing> plaatsingen)
        {
            var rows = new List<Row>();
            var header = new List<Cell>();
            var cell = 1;
            header.Add(new Cell(cell++, "Id"));
            header.Add(new Cell(cell++, "GastgezinId"));
            header.Add(new Cell(cell++, "Aantal"));
            header.Add(new Cell(cell++, "LeeftijdsGroep"));
            header.Add(new Cell(cell++, "PlaatsingsType"));
            header.Add(new Cell(cell++, "Datum"));
            header.Add(new Cell(cell++, "GeplaatstDoor"));
            header.Add(new Cell(cell++, "Leeftijd"));
            header.Add(new Cell(cell++, "Gender"));
            header.Add(new Cell(cell++, "Actief"));
            rows.Add(new Row(1, header));
            var row = 2;
            foreach (var plaatsing in plaatsingen)
            {
                var data = new List<Cell>();
                cell = 1;
                data.Add(new Cell(cell++, plaatsing.Id));
                data.Add(new Cell(cell++, plaatsing.Gastgezin.Id));
                data.Add(new Cell(cell++, plaatsing.Amount));
                data.Add(new Cell(cell++, plaatsing.AgeGroup));
                data.Add(new Cell(cell++, plaatsing.PlacementType));
                data.Add(new Cell(cell++, plaatsing.DateTime.ToShortDateString()));
                data.Add(new Cell(cell++, plaatsing.Vrijwilliger.FirstName));
                data.Add(new Cell(cell++, plaatsing.Age));
                data.Add(new Cell(cell++, plaatsing.Gender));
                data.Add(new Cell(cell++, plaatsing.Active));
                rows.Add(new Row(row, data));
                row++;
            }
            return rows;
        }

        public static ICollection<Row> VrijwilligersToDataRows(List<UserDetails> userDetails)
        {
            var rows = new List<Row>();
            var header = new List<Cell>();
            var cell = 1;
            header.Add(new Cell(cell++, "Id"));
            header.Add(new Cell(cell++, "AADId"));
            header.Add(new Cell(cell++, "Voornaam"));
            header.Add(new Cell(cell++, "Achternaam"));
            header.Add(new Cell(cell++, "Email"));
            header.Add(new Cell(cell++, "PhoneNumber"));
            header.Add(new Cell(cell++, "InVrijwilligerDropdown"));
            header.Add(new Cell(cell++, "Rollen"));
            header.Add(new Cell(cell++, "Reacties"));
            rows.Add(new Row(1, header));
            var row = 2;
            foreach (var user in userDetails)
            {
                var data = new List<Cell>();
                cell = 1;
                data.Add(new Cell(cell++, user.Id));
                data.Add(new Cell(cell++, user.AADId));
                data.Add(new Cell(cell++, user.FirstName));
                data.Add(new Cell(cell++, user.LastName));
                data.Add(new Cell(cell++, user.Email));
                data.Add(new Cell(cell++, user.PhoneNumber));
                data.Add(new Cell(cell++, user.InDropdown));
                data.Add(new Cell(cell++, String.Join(";", user.Roles)));
                data.Add(new Cell(cell++, String.Join(";", user.Reacties.Select(c => c.Id.ToString()))));
                rows.Add(new Row(row, data));
                row++;
            }
            return rows;
        }

        public static ICollection<Row> ContactLogsToDataRows(List<ContactLog> contactLogs)
        {
            var rows = new List<Row>();
            var header = new List<Cell>();
            var cell = 1;
            header.Add(new Cell(cell++, "Id"));
            header.Add(new Cell(cell++, "Datum"));
            header.Add(new Cell(cell++, "Contacter"));
            header.Add(new Cell(cell++, "Notities"));
            rows.Add(new Row(1, header));
            var row = 2;
            foreach (var contactLog in contactLogs)
            {
                var data = new List<Cell>();
                cell = 1;
                data.Add(new Cell(cell++, contactLog.Id));
                data.Add(new Cell(cell++, contactLog.DateTime.ToShortDateString()));
                data.Add(new Cell(cell++, contactLog.Contacter.FirstName));
                data.Add(new Cell(cell++, contactLog.Notes));
                
                rows.Add(new Row(row, data));
                row++;
            }
            return rows;
        }

        public static ICollection<Row> CommentsToDataRows(List<Comment> comments)
        {
            var rows = new List<Row>();
            var header = new List<Cell>();
            var cell = 1;
            header.Add(new Cell(cell++, "Id"));
            header.Add(new Cell(cell++, "Text"));
            header.Add(new Cell(cell++, "Commenter"));
            header.Add(new Cell(cell++, "Datum"));
            header.Add(new Cell(cell++, "CommentType"));
            rows.Add(new Row(1, header));
            var row = 2;
            foreach (var comment in comments)
            {
                var data = new List<Cell>();
                cell = 1;
                data.Add(new Cell(cell++, comment.Id));
                data.Add(new Cell(cell++, comment.Text));
                data.Add(new Cell(cell++, comment.Commenter.FirstName));
                data.Add(new Cell(cell++, comment.Created.ToShortDateString()));
                data.Add(new Cell(cell++, comment.CommentType));

                rows.Add(new Row(row, data));
                row++;
            }
            return rows;
        }
    }
}
