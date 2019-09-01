namespace CodeGolf.Service
{
    using System.Threading.Tasks;

    public interface ISignalRNotifier
    {
        Task NewRound();

        Task NewAnswer();

        Task NewTopScore(string userName, int score, string avatarUri);
    }
}
