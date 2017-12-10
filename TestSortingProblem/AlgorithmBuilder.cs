using System.Collections.Generic;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem
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

		protected abstract void UpdateResult(List<string> results);
	}
}