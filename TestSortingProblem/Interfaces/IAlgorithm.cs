using TestSortingProblem.Structures;

namespace TestSortingProblem.Interfaces
{
	public interface IAlgorithm
	{
		Solution Solve(bool consolePrint = false);
	}
}