using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Database.Repository;

namespace MonsterTradingUsers.Database.Repository;

public class UserMemoryRepository : IRepository<User>
{
    private Dictionary<int, User> Users = new Dictionary<int, User>();

    public User Get(int id)
    {
        return Users.GetValueOrDefault(id);
    }

    public IEnumerable<User> GetAll()
    {
        return Users.Values;
    }

    public void Add(User user)
    {
        Users.Add(user.UserId, user);
    }

    
    public void Update(User user, string[] parameters)
    {
        /*
        // update the item
        user.UserId = parameters[0] ?? throw new ArgumentNullException("UserId cannot be null");
        user.Username = int.Parse(parameters[1] ?? throw new ArgumentNullException("Username cannot be null"));
        user.PasswordHash = parameters[2];
        user.Token = parameters[3];

        // persist the updated item
        Users[user.UserId] = user;
           */
    }


    public void Delete(User user)
    {
        Users.Remove(user.UserId);
    }
}