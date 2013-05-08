using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Location : AbstractUpdatableClass
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public Country Country { get; set; }
        public State State { get; set; }
        public City City { get; set; }
        public Region Region { get; set; }

        public string Address { get; set; }
        public string ZipCode { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }

    }
 
}
