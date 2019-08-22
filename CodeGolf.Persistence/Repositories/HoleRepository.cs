namespace CodeGolf.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
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

        async Task<Option<Hole>> IHoleRepository.GetCurrentHole(CancellationToken cancellationToken)
        {
            if (await this.context.Holes.AnyAsync(cancellationToken))
            {
                return Option.Some(await this.context.Holes.OrderByDescending(a => a.Start).FirstAsync(cancellationToken));
            }
            else
            {
                return Option.None<Hole>();
            }
        }

        async Task IHoleRepository.EndHole(Guid holeId, DateTime closeTime, CancellationToken cancellationToken)
        {
            var hole = await this.context.Holes.SingleAsync(a => a.HoleId == holeId, cancellationToken);
            hole.SetEnd(closeTime);
            this.context.Update(hole);
            await this.context.SaveChangesAsync(cancellationToken);
        }

        Task IHoleRepository.AddHole(Hole hole, CancellationToken cancellationToken)
        {
            this.context.Holes.Add(hole);
            return this.context.SaveChangesAsync(cancellationToken);
        }

        Task IHoleRepository.ClearAll(CancellationToken cancellationToken)
        {
            this.context.Holes.RemoveRange(this.context.Holes);
            return this.context.SaveChangesAsync(cancellationToken);
        }

        Task IHoleRepository.Update(Hole hole, CancellationToken cancellationToken)
        {
            this.context.Attach(hole);
            this.context.Entry(hole).State = EntityState.Modified;
            this.context.Update(hole);
            return this.context.SaveChangesAsync(cancellationToken);
        }

        Task<IReadOnlyList<Hole>> IHoleRepository.GetGameHoles(Guid gameId, CancellationToken cancellationToken)
        {
            return this.context.Holes.Where(a => a.GameId == gameId).ToReadOnlyAsync(cancellationToken);
        }
    }
}
