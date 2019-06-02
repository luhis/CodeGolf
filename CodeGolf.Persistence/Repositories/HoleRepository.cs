using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Domain;
using CodeGolf.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace CodeGolf.Persistence.Repositories
{
    public class HoleRepository : IHoleRepository
    {
        private readonly CodeGolfContext context;

        public HoleRepository(CodeGolfContext context)
        {
            this.context = context;
        }


        async Task<Option<HoleInstance>> IHoleRepository.GetCurrentHole()
        {
            if (this.context.Hole.Any())
            {
                return Option.Some(await this.context.Hole.LastAsync());
            }
            else
            {
                return Option.None<HoleInstance>();
            }
        }

        Task IHoleRepository.AddHole(HoleInstance hole)
        {
            this.context.Hole.Add(hole);
            return this.context.SaveChangesAsync();
        }

        Task IHoleRepository.ClearAll()
        {
            this.context.Hole.RemoveRange(this.context.Hole);
            return this.context.SaveChangesAsync();
        }
    }
}