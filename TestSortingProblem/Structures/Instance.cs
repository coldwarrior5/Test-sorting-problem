namespace TestSortingProblem.Structures
{
	public class Instance
	{
		public readonly Test[] Tests;
		public readonly string[] Machines;
		public readonly string[] Resources;
		public readonly int[] ResourcesCount;

		public Instance(Test[] tests, string[] machines, string[] resources, int[] resourcesCount)
		{
			Tests = tests;
			Machines = machines;
			Resources = resources;
			ResourcesCount = resourcesCount;
		}

	}
}
