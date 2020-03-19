using System;
using System.Collections.Generic;
using System.Text;

namespace ClimateObservations.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Country1 { get; set; }

        public List<Country> Countries { get; set; } = new List<Country>();
    }
}
