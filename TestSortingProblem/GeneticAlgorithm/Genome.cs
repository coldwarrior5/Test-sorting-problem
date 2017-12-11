using System;
using TestSortingProblem.Structures;

namespace TestSortingProblem.GeneticAlgorithm
{
    public class Genome
    {
        public int Size { get; }
        private int[] _startingTimes;
        private int[] _endingTimes;
        private string[] _machines;

        private Scheduler[] _machineSchedulers;
        private Scheduler[] _resourceSchedulers;
        public int Fitness { get; }

        public Genome(int size, string[] machinesList, string[] resourcesList, int[] resourcesCountList)
        {
            Size = size;
            InitArrays(machinesList, resourcesList, resourcesCountList);
        }

        public Genome(int[] startingTimes, int[] endingTimes, string[] machines, string[] machinesList, string[] resourcesList, int[] resourcesCountList)
        {
            var startingLength = startingTimes.Length;
            var endingLength = endingTimes.Length;
            var machinesLength = machines.Length;
            if (endingLength != startingLength || endingLength != machinesLength)
                throw new ArgumentException("The arrays must be of the same size");

            Size = startingLength;
            InitArrays(machinesList, resourcesList, resourcesCountList);
            SetMachines(machines);
            SetStarts(startingTimes);
            SetEndings(endingTimes);
        }

        private void InitArrays(string[] machines, string[] resources, int[] resourcesCountList)
        {
            _machines = new string[Size];
            _startingTimes = new int[Size];
            _endingTimes = new int[Size];
            InitSchedulers(machines, resources, resourcesCountList);
        }

        private void InitSchedulers(string[] machines, string[] resources, int[] resourcesCountList)
        {
            _machineSchedulers = new Scheduler[machines.Length];
            _resourceSchedulers = new Scheduler[resources.Length];
            for (var i = 0; i < machines.Length; i++)
                _machineSchedulers[i] = new Scheduler(1);
            for (var i = 0; i < resources.Length; i++)
                _resourceSchedulers[i] = new Scheduler(resourcesCountList[i]);
        }

        public int[] GetEndings()
        {
            return _endingTimes;
        }

        public string[] GetMachines()
        {
            return _machines;
        }

        public int[] GetTimes()
        {
            return _startingTimes;
        }

        public Scheduler GetMachineScheduler(int index)
        {
            return _machineSchedulers[index];
        }
        
        public Scheduler CopyMachineScheduler(int index)
        {
            var newMachineScheduler = new Scheduler(_machineSchedulers[index].ResourceCount);
            newMachineScheduler.Copy(_machineSchedulers[index]);
            return newMachineScheduler;
        }
        
        public Scheduler GetResourceScheduler(int index)
        {
            return _resourceSchedulers[index];
        }
        
        public Scheduler CopyResourceeScheduler(int index)
        {
            var newResourceScheduler = new Scheduler(_resourceSchedulers[index].ResourceCount);
            newResourceScheduler.Copy(_resourceSchedulers[index]);
            return newResourceScheduler;
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

        public void SetStarts(int[] startingTimes)
        {
            if (startingTimes.Length != Size)
                return;
            for (var i = 0; i < Size; i++)
            {
                _startingTimes[i] = startingTimes[i];
            }
        }
        
        public void SetEndings(int[] endingTimes)
        {
            if (endingTimes.Length != Size)
                return;
            for (var i = 0; i < Size; i++)
            {
                _endingTimes[i] = endingTimes[i];
            }
        }

        public void Copy(Genome newState)
        {
            if (Size != newState.Size)
                return;
            for (var i = 0; i < newState.Size; i++)
            {
                _startingTimes[i] = newState._startingTimes[i];
                _endingTimes[i] = newState._endingTimes[i];
                _machines[i] = newState._machines[i];
            }

            for (var i = 0; i < newState._machineSchedulers.Length; i++)
            {
                _machineSchedulers[i].Copy(newState.CopyMachineScheduler(i));
                _resourceSchedulers[i].Copy(newState.CopyResourceeScheduler(i));
            }
        }
    }
}