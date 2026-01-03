using System;
using System.Collections.Generic;
using System.Text;

namespace EsmoChamps.Models
{
    public class Champion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int RangeTypeId { get; set; }
        public RangeType RangeType { get; set; }

        public int ChampTypeId { get; set; }
        public ChampType ChampType { get; set; }

        public int MechanicsMin { get; set; }
        public int MechanicsMax { get; set; }
        public int MacroMin { get; set; }
        public int MacroMax { get; set; }
        public int TacticalMin { get; set; }
        public int TacticalMax { get; set; }


        public int PowerCurveStart { get; set; }
        public int PowerCurveMid { get; set; }
        public int PowerCurveEnd { get; set; }

        public ICollection<ChampionStrength> Strengths { get; set; }
    }
}
