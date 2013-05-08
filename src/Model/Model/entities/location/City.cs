﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class City : AbstractReadOnlyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public int StateId { get; set; }
        public State State { get; set; }
    }
}
