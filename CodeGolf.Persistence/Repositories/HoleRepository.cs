namespace CodeGolf.Persistence.Repositories
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeGolf.Domain;
    using CodeGolf.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Optional;

    public class HoleRepository : IHoleRepository
    {
        private readonly CodeGolfContext context;

        public HoleRepository(CodeGolfContext context)
        {
            this.context = context;
        }

        async Task<Option<HoleInstance>> IHoleRepository.GetCurrentHole(CancellationToken cancellationToken)
        {
            if (await this.context.Holes.AnyAsync(cancellationToken))
            {
                return Option.Some(await this.context.Holes.OrderByDescending(a => a.Start).FirstAsync(cancellationToken));
            }
            else
            {
                return Option.None<HoleInstance>();
            }
        }

        async Task IHoleRepository.EndHole(Guid holeId, DateTime closeTime, CancellationToken cancellationToken)
        {
            var hole = await this.context.Holes.SingleAsync(a => a.HoleId == holeId);
            this.context.Update(new HoleInstance(hole.HoleId, hole.Start, closeTime));
            await this.context.SaveChangesAsync(cancellationToken);
        }

        Task IHoleRepository.AddHole(HoleInstance hole, CancellationToken cancellationToken)
        {
            this.context.Holes.Add(hole);
            return this.context.SaveChangesAsync(cancellationToken);
        }

        Task IHoleRepository.ClearAll(CancellationToken cancellationToken)
        {
            this.context.Holes.RemoveRange(this.context.Holes);
            return this.context.SaveChangesAsync(cancellationToken);
        }
    }
}
