using System;
using System.Collections.Generic;
using System.Text;

namespace EsmoChamps.Models
{
    public class StrengthTitle
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<ChampionStrength> ChampionStrengths { get; set; }
    }
}
