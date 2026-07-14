namespace DarkKitchen.Domain.Repositories;

public interface IUpdateRepository<T>
{
    void Update(T entity);
}
