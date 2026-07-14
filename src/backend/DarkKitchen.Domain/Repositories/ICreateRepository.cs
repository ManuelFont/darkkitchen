namespace DarkKitchen.Domain.Repositories;

public interface ICreateRepository<T>
{
    void Add(T entity);
}
