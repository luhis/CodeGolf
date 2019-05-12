using System;
using System.Threading.Tasks;

namespace CodeGolf.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetById(Guid id);
    }
}