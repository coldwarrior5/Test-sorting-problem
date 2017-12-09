using TestSortingProblem.Interfaces;

namespace TestSortingProblem.Handlers
{
    public abstract class IoAbstract : IIoHandler
    {
        public abstract void GetParameters(string[] args);
        protected abstract void HandleArguments(string[] args);
        protected abstract void ConsoleParameters();
    }
}