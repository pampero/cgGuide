using System;
using System.Web;
using System.ComponentModel;

namespace Model
{
    public class Customer : AbstractUpdatableClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
