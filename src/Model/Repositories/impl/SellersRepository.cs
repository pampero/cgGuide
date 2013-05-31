using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Repositories.interfaces;

namespace Model.Repositories.impl
{
    public class BusinessesRepository : GenericUpdatableRepository<Business>, IGenericRepository<Business>
    {
        public BusinessesRepository(AppDbContext context)
            : base(context)
        {

        }


        public List<Business> GetAll()
        {
            // TODO: SOLO RETORNAR IQueryable. No es PERFORMANTE con el ToList en este punto.
            return context.Businesses.Where(x => !x.IsDeleted).OrderByDescending(x => x.Created).ToList();
        }
    }
}