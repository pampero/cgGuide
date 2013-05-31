
namespace Model.Repositories.interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Route> RoutesRepository { get; }
        IGenericRepository<Business> BusinessesRepository { get; }

        void Save();
    }
}
