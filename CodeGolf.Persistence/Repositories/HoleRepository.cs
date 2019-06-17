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
            if (this.context.Holes.Any())
            {
                return Option.Some(await this.context.Holes.LastAsync());
            }
            else
            {
                return Option.None<HoleInstance>();
            }
        }

        Task IHoleRepository.AddHole(HoleInstance hole)
        {
            this.context.Holes.Add(hole);
            return this.context.SaveChangesAsync();
        }

        Task IHoleRepository.ClearAll()
        {
            this.context.Holes.RemoveRange(this.context.Holes);
            return this.context.SaveChangesAsync();
        }
    }
}