using System.Collections.Generic;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem
{
	public abstract class AlgorithmBuilder : IAlgorithm
	{
	    private Instance Structure { get; }

	    protected AlgorithmBuilder(Instance structure)
		{
			Structure = structure;
		}
		
		public abstract void Solve(ExecutionTime time);

		protected abstract void Iterate();

		protected abstract void UpdateResult(List<string> results);
	}
}