using System;
using System.Collections.Generic;
using System.Text;

namespace ClimateObservations.Models
{
    public class Observer
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string FullName
        {
            get
            {
                return Firstname + " " + Lastname;
            }
        }
        public List<Observer> Locations { get; set; } = new List<Observer>();
    }
}
