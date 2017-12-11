using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Abstract
{
	public abstract class AlgorithmBuilder : IAlgorithm
	{
	    private Instance Instance { get; }
		private ExecutionTime Time { get; }

	    protected AlgorithmBuilder(Instance structure, ExecutionTime time)
		{
			Instance = structure;
			Time = time;
		}
		
		public abstract void Solve();

		protected abstract void Iterate();

		protected abstract void UpdateResult(Solution results);
	}
}