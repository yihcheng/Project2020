using System.Threading.Tasks;

namespace CommonContracts
{
    public interface IEngine
    {
        Task RunAsync(string e2eFile);
    }
}
