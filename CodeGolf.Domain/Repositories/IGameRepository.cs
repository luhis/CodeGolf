namespace CodeGolf.Domain.Repositories
{
    using System;

    using Optional;

    public interface IGameRepository
    {
        Game GetGame();

        Option<Hole> GetByHoleId(Guid holeId);

        Option<Hole> GetAfter(Guid holeId);
    }
}
