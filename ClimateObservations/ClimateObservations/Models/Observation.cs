using System;
using System.Collections.Generic;
using System.Text;

namespace ClimateObservations.Models
{
    public class Observation
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }// Er dette riktig datatype
        public int Observer_id  { get; set; }
        public int Geolocation_id { get; set; }

        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
