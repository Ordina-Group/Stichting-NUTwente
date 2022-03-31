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
        public Task<bool> sendMail(Mail mail);

        public void setFromMail(string mailAdress);

        public void setApiKey(string apiKey);
    }
}
