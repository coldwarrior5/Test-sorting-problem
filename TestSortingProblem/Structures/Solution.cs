using System;

namespace TestSortingProblem.Structures
{
    public class Solution
    {
        public int Size { get; }
        private string[] _tests;
        private int[] _startingTimes;
        private string[] _machines;

        public Solution(int size)
        {
            Size = size;
        }

        public Solution(string[] tests, int[] startingTimes, string[] machines)
        {
            var testLength = tests.Length;
            var startingLength = startingTimes.Length;
            var machinesLength = machines.Length;
            if (testLength != startingLength || testLength != machinesLength)
                throw new ArgumentException("The arrays must be of the same size");

            Size = testLength;
            InitArrays();
            SetTests(tests);
            SetMachines(machines);
            SetTimes(startingTimes);
        }

        private void InitArrays()
        {
            _tests = new string[Size];
            _machines = new string[Size];
            _startingTimes = new int[Size];
        }

        public string[] GetTests()
        {
            return _tests;
        }

        public string[] GetMachines()
        {
            return _machines;
        }

        public int[] GetTimes()
        {
            return _startingTimes;
        }

        public void SetTests(string[] tests)
        {
            if (tests.Length != Size)
                return;
            for (var i = 0; i < Size; i++)
            {
                _tests[i] = tests[i];
            }
        }

        public void SetMachines(string[] machines)
        {
            if (machines.Length != Size)
                return;
            for (var i = 0; i < Size; i++)
            {
                _machines[i] = machines[i];
            }
        }

        public void SetTimes(int[] startingTimes)
        {
            if (startingTimes.Length != Size)
                return;
            for (var i = 0; i < Size; i++)
            {
                _startingTimes[i] = startingTimes[i];
            }
        }
    }
}