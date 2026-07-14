namespace DarkKitchen.Domain.Repositories;

public interface IRepository<T> :
    ICreateRepository<T>,
    IReadRepository<T>,
    IUpdateRepository<T>,
    IDeleteRepository<T>
{
}
