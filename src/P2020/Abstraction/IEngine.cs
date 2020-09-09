using System.Threading.Tasks;

namespace P2020.Abstraction
{
    public interface IEngine
    {
        Task RunAsync(string[] e2eFiles);
    }
}
