namespace Plyfood.Repository;

public interface IRepository<T,TGiud>
{
    IEnumerable<T> GetAll();
    T GetById(TGiud id);
    void Add(T entity);
    void Update(T entity);
    void Delete(TGiud id);
}