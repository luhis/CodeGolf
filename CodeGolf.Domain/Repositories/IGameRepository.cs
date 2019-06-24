namespace CodeGolf.Domain.Repositories
{
    using System;

    using Optional;

    public interface IGameRepository
    {
        Game GetGame();

        Option<Hole> GetById(Guid id);

        Option<Hole> GetAfter(Guid id);
    }
}