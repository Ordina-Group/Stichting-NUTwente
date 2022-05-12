using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IMailService
    {
        public Task<bool> SendMail(Mail mail);

        public void SetFromMail(string mailAdress);

        public Task<bool> SendGroupMail(string subject, string message, List<string> mailAdresses);

        public void SetApiKey(string apiKey);

    }
}
