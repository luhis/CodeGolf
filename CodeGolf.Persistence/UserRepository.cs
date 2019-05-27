using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;

namespace CodeGolf.Persistence
{
    public class UserRepository : IUserRepository
    {
        private static readonly List<User> Attempts = new List<User>();

        Task<User> IUserRepository.GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}