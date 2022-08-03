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
            VeiligeOpbergruimte = "";
            HuisdierenAanwezig = "";
            BezwaarTegenHuisdieren = "";
            Allergieen = "";
            VluchtelingOphalen = "";
            BasisscholenAanwezig = "";
            Beperkingen = "";
            AdresVanLocatie = "";
            PostcodeVanLocatie = "";
            PlaatsnaamVanLocatie = "";
            OverigeOpmerkingen = "";
            GezinsSamenstelling = "";
            TelefoonnummerVanLocatie = "";
            GezinsLeeftijden = "";
            SlaapplaatsOpmerking = "";
            EigenToegangsdeur = "";
            Sanitair = "";
            Toegankelijkheid = "";
            ElektraSpatwaterdicht = "";
            DaglichtVerblijfsruimte = "";
            RookmelderAanwezig = "";
            Whatsapp = "";
            Hobbys = "";
            Talen = "";
        }

        public string? Whatsapp { get; set; }
        public string? KleineKinderen { get; set; }
        public string? VolwassenenGrotereKinderen { get; set; }
        public string? SlaapkamerRuimte { get; set; }
        public string? Privacy { get; set; }
        public string? Opbergruimte { get; set; } //todo: remove
        public string? Faciliteiten { get; set; }
        public string? ZelfKoken { get; set; }
        public string? KoelkastRuimte { get; set; }
        public string? DaglichtSlaapkamer { get; set; }
        public string? Roken { get; set; }
        public string? VeiligeOpbergruimte { get; set; }
        public string? HuisdierenAanwezig { get; set; }
        public string? BezwaarTegenHuisdieren { get; set; }
        public string? Allergieen { get; set; }
        public string? VluchtelingOphalen { get; set; }
        public string? BasisscholenAanwezig { get; set; }
        public string? Beperkingen { get; set; }
        public string? AdresVanLocatie { get; set; }
        public string? PostcodeVanLocatie { get; set; }
        public string? PlaatsnaamVanLocatie { get; set; }
        public string? OverigeOpmerkingen { get; set; }
        public string? GezinsSamenstelling { get; set;}
        public string? TelefoonnummerVanLocatie { get; set; }
        public string? GezinsLeeftijden { get; set; }
        public string? SlaapplaatsOpmerking { get; set; }
        public string? EigenToegangsdeur {get; set; }
        public string? Sanitair { get; set; }
        public string? Toegankelijkheid { get; set; }
        public string? ElektraSpatwaterdicht { get; set; } //new
        public string? DaglichtVerblijfsruimte { get; set; } //new
        public string? RookmelderAanwezig { get; set; } //new
        public string? Hobbys { get; set; } //new
        public string? Talen { get; set; } //new


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
