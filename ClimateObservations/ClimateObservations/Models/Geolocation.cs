using System;
using System.Collections.Generic;
using System.Text;

namespace ClimateObservations.Models
{
    public class Geolocation
    {
        public int Id { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public int Area_id { get; set; }
        public List<Geolocation> Locations { get; set; } = new List<Geolocation>();
    }
}

