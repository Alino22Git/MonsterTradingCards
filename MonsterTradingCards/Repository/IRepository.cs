using System.Collections.Generic;

namespace MonsterTradingCards.Repository;

// Implementation of the Repository Design Pattern
// Repository overview see: https://dotnettutorials.net/lesson/repository-design-pattern-csharp/
public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    void Update(T t);
    void Delete(T t);
}
