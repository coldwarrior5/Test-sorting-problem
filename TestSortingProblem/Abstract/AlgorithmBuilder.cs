using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Abstract
{
	public abstract class AlgorithmBuilder : IAlgorithm
	{
		protected Instance Instance;
		protected ExecutionTime Time;

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