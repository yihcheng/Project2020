using P2020.Abstraction;

namespace P2020.Engine
{
    internal static class ComputerSelector
    {
        public static IComputer GetCurrentComputer()
        {
            // a way to determine which OS is in current computer
            // for now, we return Windows Computer only for simplicity
            return new WindowsOS();

            // TODO : extend to MacOs and Linux here. Create independent project for each OS.
        }
    }
}
