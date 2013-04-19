using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IDeleteFields : IEntity
    {
        string DeletedBy { get; set; }
        
        DateTime? Deleted { get; set; }
    }
}
