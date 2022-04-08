using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class PersoonViewModel
    {
        public PersoonViewModel()
        {
            Naam = "";
            GeboorteDatum = "";
            Geboorteplaats = "";
            Email = "";
            Telefoonnummer = "";
            Mobiel = "";
            Nationaliteit = "";
            Talen = "";
        }

        public string Naam { get; set; }
        public string GeboorteDatum { get; set; }
        public string Geboorteplaats { get; set; }
        public string Email { get; set; }
        public string Telefoonnummer { get; set; }
        public string Mobiel { get; set; }
        public string Nationaliteit { get; set; }
        public string Talen { get; set; }

        public Gastgezin? Gastgezin { get; set; }
        public List<Reactie>? Reactie { get; set; }
        public Adres? Adres { get; set; }
    }

    public class VluchtelingListView
    {
        public VluchtelingListView(Persoon vluchteling) {
            Naam = vluchteling.Naam;
            Email = vluchteling.Email;
            Telefoonnummer = vluchteling.Telefoonnummer;
            Gastgezin = vluchteling.Gastgezin;
            Reactie = vluchteling.Reactie;
        }
        public string Naam { get; set; }
        public string Email { get; set; }
        public string Telefoonnummer { get; set; }
        public Gastgezin? Gastgezin { get; set; }
        public Reactie? Reactie { get; set; }

    }

    public class VluchtelingViewModel
    {
        public VluchtelingViewModel()
        {
            Naam = "";
            GeboorteDatum = "";
            Geboorteplaats = "";
            Email = "";
            Telefoonnummer = "";
            Mobiel = "";
            Nationaliteit = "";
            Talen = "";
        }

        public string Naam { get; set; }
        public string GeboorteDatum { get; set; }
        public string Geboorteplaats { get; set; }
        public string Email { get; set; }
        public string Telefoonnummer { get; set; }
        public string Mobiel { get; set; }
        public string Nationaliteit { get; set; }
        public string Talen { get; set; }
        public Gastgezin? Gastgezin { get; set; }
        public List<Reactie>? Reactie { get; set; }
        public Adres? Adres { get; set; }
    }
}
