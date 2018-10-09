using System.Threading.Tasks;

namespace Abstractions
{
    public interface IEngine
    {
        Task RunAsync(string[] e2eFiles);
    }
}
