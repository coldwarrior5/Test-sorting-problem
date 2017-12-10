using System.Collections.Generic;
namespace TestSortingProblem.Structures
{
	public class Instance
	{
		public readonly List<Test> Tests;
		public readonly List<string> Machines;
		public readonly List<string> Resources;

		protected Instance(List<Test> tests, List<string> machines, List<string> resources)
		{
			Tests = tests;
			Machines = machines;
			Resources = resources;
		}

	}
}
