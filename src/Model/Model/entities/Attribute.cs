using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    // Acceso para discapacitados: Si | Aire Acondicionado: Si | Baño para discapacitados: Si | Reservas: Si
    public class Attribute : AbstractUpdatableClass
    {
        //public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool Value { get; set; }

        public List<Seller> Sellers { get; set; }
    }
}
