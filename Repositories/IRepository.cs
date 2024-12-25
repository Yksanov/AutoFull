namespace AutoFull.Repositories;

public interface IRepository<T>
{ 
    public T? GetById(int? id);
    public IEnumerable<T> GetAll();
    public void Create(T entity);
}