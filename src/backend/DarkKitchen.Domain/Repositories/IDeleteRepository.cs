namespace DarkKitchen.Domain.Repositories;

public interface IDeleteRepository<T>
{
    void Delete(Guid id);
}
