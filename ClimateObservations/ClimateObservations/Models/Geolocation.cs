using System;
using System.Collections.Generic;
using System.Text;

namespace ClimateObservations.Models
{
    public class Geolocation
    {
        public int Id { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int Area_id { get; set; }
        public List<Geolocation> Locations { get; set; } = new List<Geolocation>();
    }
}

