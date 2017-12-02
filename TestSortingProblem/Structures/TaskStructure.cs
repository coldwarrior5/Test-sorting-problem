using System;
using System.Collections.Generic;
namespace TestSortingProblem.Structures
{
	public class TaskStructure
	{
		private readonly List<string> _tests;
		private readonly List<string> _machines;
		private readonly List<string> _resources;

		protected TaskStructure(List<string> tests, List<string> machines, List<string> resources)
		{
			_tests = tests;
			_machines = machines;
			_resources = resources;
		}

		public List<string> GetTests()
		{
			return _tests;
		}

		public List<string> GetMachines()
		{
			return _machines;
		}

		public List<string> GetResources()
		{
			return _resources;
		}
	}
}
