using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface ITestE2E
    {
        string FullName { get; }
        string ShortName { get; }
        bool Skip { get; }
        string ProgramToLaunch { get; }
        bool MakeLaunchedProgramMaximized { get; }
        IReadOnlyList<ITestStep> Steps { get; }
    }

    public interface ITestStep
    {
        string Target { get; }
        string Search { get; }
        string Action { get; }
        string ActionArgument { get; }
        IReadOnlyList<ScreenSearchArea> SearchArea { get; }
        int WaitingSecond { get; }
        bool FailureReport { get; }
        int Retry { get; }
    }

    public interface ITestStepExecutor
    {
        Task<bool> ExecuteAsync();
    }
}
