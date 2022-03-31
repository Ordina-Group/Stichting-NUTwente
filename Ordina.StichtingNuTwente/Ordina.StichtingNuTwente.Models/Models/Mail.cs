using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Mail : BaseEntity
    {
        public string Message { get; set; }
        public string Subject { get; set; }
        public string MailToAdress { get; set; }
        public string MailToName { get; set; }
        public string MailFromName { get; set; }
    }
}
