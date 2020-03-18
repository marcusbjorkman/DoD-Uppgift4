using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservations.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public double? Value { get; set; }
        public Category Category { get; set; }
    }
}
