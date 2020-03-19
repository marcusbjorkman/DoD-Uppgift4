using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservations.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category BaseCategory { get; set; }
        public Unit Unit { get; set; }

        public string ToDisplay
        {
            get
            {
                return Name + (Unit == null ? "" : ("/" + Unit.Abbreviation));
            }
        }
    }
}
