using System.Collections.Generic;

namespace TestSortingProblem.Structures
{
    public class Test
    {
        public readonly string Name;
        public readonly int Length;
        public readonly List<string> Machines;
        public readonly List<string> Resources;

        public Test(string name, int length, List<string> machines, List<string> resources)
        {
            Name = name;
            Length = length;
            Machines = machines;
            Resources = resources;
        }
    }
}