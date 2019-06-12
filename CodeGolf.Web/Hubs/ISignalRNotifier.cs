using System.Threading.Tasks;

namespace CodeGolf.Web.Hubs
{
    public interface ISignalRNotifier
    {
        Task NewRound();
        Task NewAnswer();
    }
}