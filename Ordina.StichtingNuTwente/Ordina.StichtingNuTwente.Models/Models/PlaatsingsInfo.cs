using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class PlaatsingsInfo : BaseEntity
    {
        public PlaatsingsInfo()
        {
            Belemmering = "";
            KleineKinderen = "";
            VolwassenenGrotereKinderen = "";
            SlaapkamerRuimte = "";
            Privacy = "";
            Opbergruimte = "";
            Faciliteiten = "";
            ZelfKoken = "";
            KoelkastRuimte = "";
            DaglichtSlaapkamer = "";
            Roken = "";
            AlchoholEnDrugs = "";
            VeiligeOpbergruimte = "";
            HuisdierenAanwezig = "";
            HuisdierenMogelijk = "";
            Allergieen = "";
            VluchtelingOphalen = "";
            BasisscholenAanwezig = "";
            KinderenInDeBuurt = "";
            FaciliteitenVoorKinderen = "";
            OverigeOpmerkingen = "";
        }

        public string? Belemmering { get; set; }
        public string? KleineKinderen { get; set; }
        public string? VolwassenenGrotereKinderen { get; set; }
        public string? SlaapkamerRuimte { get; set; }
        public string? Privacy { get; set; }
        public string? Opbergruimte { get; set; }
        public string? Faciliteiten { get; set; }
        public string? ZelfKoken { get; set; }
        public string? KoelkastRuimte { get; set; }
        public string? DaglichtSlaapkamer { get; set; }
        public string? Roken { get; set; }
        public string? AlchoholEnDrugs { get; set; }
        public string? VeiligeOpbergruimte { get; set; }
        public string? HuisdierenAanwezig { get; set; }
        public string? HuisdierenMogelijk { get; set; }
        public string? Allergieen { get; set; }
        public string? VluchtelingOphalen { get; set; }
        public string? BasisscholenAanwezig { get; set; }
        public string? KinderenInDeBuurt { get; set; }
        public string? FaciliteitenVoorKinderen { get; set; }
        public string? Beperkingen { get; set; }
        public string? AdresVanLocatie { get; set; }
        public string? PostcodeVanLocatie { get; set; }
        public string? PlaatsnaamVanLocatie { get; set; }
        public string? OverigeOpmerkingen { get; set; }
        public string? GezinsSamenstelling { get; set;}
        public string? TelefoonnummerVanLocatie { get; set; }
        [ForeignKey("fkReactieId")]
        public virtual Reactie? Reactie { get; set; }

        public string? GetValueByFieldString(string fieldname)
        {
            var field = GetType()?.GetProperty(fieldname)?.GetValue(this, null);
            if (field != null)
                return field.ToString();
            return null;
        }
    }
}
