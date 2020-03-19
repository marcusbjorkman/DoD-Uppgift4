using System;
using System.Collections.Generic;
using System.Text;

namespace ClimateObservations.Models
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Country_id { get; set; }
        
        public List<Area> Areas { get; set; } = new List<Area>();
    }
}
