using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Category : AbstractUpdatableClass
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GroupCategory GroupCategory { get; set; }
        public List<Business> Sellers { get; set; }
        public List<Attribute> Attributes { get; set; }
        public Category Parent { get; set; }
    }
}
