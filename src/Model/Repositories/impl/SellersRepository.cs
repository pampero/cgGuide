using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Repositories.interfaces;

namespace Model.Repositories.impl
{
    public class SellersRepository : GenericUpdatableRepository<Seller>, IGenericRepository<Seller>
    {
        public SellersRepository(AppDbContext context)
            : base(context)
        {

        }


        public List<Seller> GetAll()
        {
            // TODO: SOLO RETORNAR IQueryable. No es PERFORMANTE con el ToList en este punto.
            return context.Sellers.Where(x => !x.IsDeleted).OrderByDescending(x => x.Created).ToList();
        }
    }
}