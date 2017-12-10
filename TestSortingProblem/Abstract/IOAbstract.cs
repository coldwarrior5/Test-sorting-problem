using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Abstract
{
    public abstract class IoAbstract : IIoHandler
    {
        public abstract InputData GetParameters(string[] args);
        protected abstract InputData HandleArguments(string[] args);
        protected abstract InputData ConsoleParameters();
    }
}