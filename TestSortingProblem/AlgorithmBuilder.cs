using System.Collections.Generic;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem
{
	public abstract class AlgorithmBuilder : IAlgorithm
	{
	    private TaskStructure Structure { get; }

	    protected AlgorithmBuilder(TaskStructure structure)
		{
			Structure = structure;
		}
		
		public abstract void Solve(ExecutionTime time);

		protected abstract void Iterate();

		protected abstract void UpdateResult(List<string> results);
	}
}