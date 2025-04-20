namespace Pharma_LinkAPI.Repositries
{
    public interface Irepo<T>
    {
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
