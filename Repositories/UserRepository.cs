using AutoFull.Models;

namespace AutoFull.Repositories;

public class UserRepository : IUserRepository
{
    public MyUser? GetById(int? id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<MyUser> GetAll()
    {
        throw new NotImplementedException();
    }

    public void Create(MyUser entity)
    {
        throw new NotImplementedException();
    }
}