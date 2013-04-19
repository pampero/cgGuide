using System;
using System.Web;
using System.ComponentModel;

namespace Model
{
    public class Route: AbstractUpdatableClass
    {
        public string Name
        {
            get; set;
        }

        public int Distance
        {
            get; set;
        }

        public string Comments
        {
            get; set;
        }

        public Customer Customer
        {
            get;set;
        }
    }
}
