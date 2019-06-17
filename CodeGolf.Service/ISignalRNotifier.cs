using System.Threading.Tasks;

namespace CodeGolf.Service
{
    public interface ISignalRNotifier
    {
        Task NewRound();

        Task NewAnswer();

        Task NewTopScore(string userName, int score, string avatarUri);
    }
}