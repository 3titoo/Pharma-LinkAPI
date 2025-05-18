namespace Pharma_LinkAPI.Repositries
{
    public interface Irepo<T>
    {
        Task<T?> GetById(int id);
        Task<IEnumerable<T?>> GetAll();
        Task Add(T? entity);
        Task Update(T? entity);
        Task Delete(int id);
    }
}
