using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonContracts
{
    public interface ITestE2E
    {
        string FullName { get; }
        string ShortName { get; }
        bool Skip { get; }
        string ProgramToLaunch { get; }
        IList<ITestStep> Steps { get; }
    }

    public interface ITestStep
    {
        string Target { get; }
        string Search { get; }
        string Action { get; }
        string ActionArgument { get; }
        ScreenSearchArea SearchArea { get; }
        int WaitingSecond { get; }
        bool FailureReport { get; }
    }

    public interface ITestStepExecutor
    {
        Task<bool> ExecuteAsync();
    }
}
