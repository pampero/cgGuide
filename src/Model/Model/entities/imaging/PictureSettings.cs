using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PictureSettings : AbstractReadOnlyClass
    {
        public int Width { get; set; }
        public int Heigth { get; set; }
        public string WatermarkPath { get; set; }
        public List<Picture> Pictures { get; set; }
    }
}
