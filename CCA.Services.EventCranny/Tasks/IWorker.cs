using System.Threading;
using System.Threading.Tasks;

namespace CCA.Services.EventCranny.Tasks
{
    public interface IWorker
    {
        Task DoTheTask();
    }
}