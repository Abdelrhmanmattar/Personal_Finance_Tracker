namespace Core.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        IBaseRepo<T> Repository<T>() where T : class;
        int SaveChanges();

    }
}
