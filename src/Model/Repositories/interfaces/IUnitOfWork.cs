
namespace Model.Repositories.interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Route> RoutesRepository { get; }
        IGenericRepository<Seller> SellersRepository { get; }

        void Save();
    }
}
