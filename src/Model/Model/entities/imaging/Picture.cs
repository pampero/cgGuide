using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Picture : AbstractUpdatableClass
    {
        //public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public PictureSettings PictureSettings { get; set; }
    }
}
