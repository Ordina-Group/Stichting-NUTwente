﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Antwoord : BaseEntity
    {
        public string Response { get; set; }

        public int IdVanVraag { get; set; }

        [ForeignKey("ReactieId")]
        public virtual Reactie? Reactie { get; set; }

    }
}
