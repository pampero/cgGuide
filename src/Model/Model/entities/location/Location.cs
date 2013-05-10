using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Location : AbstractUpdatableClass
    {
        //public int Id { get; set; }
        public string Description { get; set; }

        //public int CountryId { get; set; }
        //public Country Country { get; set; }

        //[ForeignKey("State")]
        //public int StateId { get; set; }
        //public State State { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public City City { get; set; }

        public string Address { get; set; }
        public string ZipCode { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }

    }
 
}
