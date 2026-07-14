using System.Linq.Expressions;

namespace DarkKitchen.Domain.Repositories;

public interface IReadRepository<T>
{
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
    bool Exists(Expression<Func<T, bool>> predicate);
}
