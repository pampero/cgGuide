using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model
{
    public class GroupCategory : AbstractUpdatableClass
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Category> Categories { get; set; }
    }
}