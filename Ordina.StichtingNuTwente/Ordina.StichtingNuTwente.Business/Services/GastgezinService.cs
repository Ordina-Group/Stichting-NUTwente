﻿using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class GastgezinService : IGastgezinService
    {
        private readonly NuTwenteContext _context;
        public GastgezinService(NuTwenteContext context)
        {
            _context = context;
        }

        public Gastgezin? GetGastgezin(int id)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            return gastgezinRepository.GetById(id, "Contact,Vluchtelingen,Begeleider");
        }

        public Gastgezin? GetGastgezinForReaction(int formID)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            var gastgezin = gastgezinRepository.GetAll("Contact.Reactie,Vluchtelingen,Begeleider").FirstOrDefault(g => g.Contact.Reactie.Id == formID);
            if(gastgezin == null)
            {
                var persoonRepository = new Repository<Persoon>(_context);
                var persoon = persoonRepository.GetAll("Reactie").FirstOrDefault(p => p.Reactie.Id == formID);
                gastgezin = new Gastgezin()
                {
                    Contact = persoon,
                    Status = GastgezinStatus.Aangemeld
                };

                var success = Save(gastgezin);
                if (!success)
                    gastgezin = null;
            }
            return gastgezin;
        }

        public ICollection<Gastgezin> GetGastgezinnenForVrijwilliger(Persoon vrijwilliger)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);

            var gastgezinnen = gastgezinRepository.GetAll("Contact,Vluchtelingen,Begeleider").Where(g => g.Begeleider.Id == vrijwilliger.Id);
            return gastgezinnen.ToList();
        }

        public bool Save(Gastgezin gastgezin)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            gastgezin.Id = -1;
            var dbModel = gastgezinRepository.Create(gastgezin);
            return dbModel.Id > 0;
        }

        public Gastgezin UpdateGastgezin(Gastgezin gastgezin, int id)
        {
            throw new NotImplementedException();
        }
    }
}