using System;
using System.Collections.Generic;
using System.Text;

namespace EsmoChamps.Models
{
    public class ChampionStrength
    {
        public int ChampionId { get; set; }
        public Champion Champion { get; set; }

        public int StrengthTitleId { get; set; }
        public StrengthTitle StrengthTitle { get; set; }

        public int Value { get; set; }
    }
}
