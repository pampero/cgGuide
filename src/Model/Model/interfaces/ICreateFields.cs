using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface ICreateFields : IEntity
    {
        string CreatedBy { get; set; }

        DateTime Created { get; set; }
    }
}
