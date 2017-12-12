using System;
using System.Linq;
using TestSortingProblem.Handlers;
using TestSortingProblem.Structures;

namespace TestSortingProblem.GeneticAlgorithm
{
    public class Genome
    {
        public int Size { get; }
        private readonly Instance _instance;
        private int[] _startingTimes;
        private int[] _endingTimes;
        private string[] _machines;

        private Scheduler[] _machineSchedulers;
        private Scheduler[] _resourceSchedulers;
	    public int Fitness;

        private Schedule _instanceSchedule;
        private Schedule _dependencySchedule;

        public Genome(Instance instance)
        {
            Size = instance.Tests.Length;
            _instance = instance;
            _instanceSchedule = new Schedule();
            _dependencySchedule = new Schedule();
            InitArrays(instance);
        }

        public Genome(int[] startingTimes, int[] endingTimes, string[] machines, Instance instance)
        {
	        Size = instance.Tests.Length;
            _instance = instance;
            var startingLength = startingTimes.Length;
            var endingLength = endingTimes.Length;
            var machinesLength = machines.Length;
            if (Size != startingLength || Size != machinesLength || Size != endingLength)
                throw new ArgumentException("The arrays must be of the same size");

            Size = startingLength;
            InitArrays(instance);
            SetMachines(machines);
            SetStarts(startingTimes);
            SetEndings(endingTimes);
        }

        private void InitArrays(Instance instance)
        {
            _machines = new string[Size];
            _startingTimes = new int[Size];
            _endingTimes = new int[Size];
            InitSchedulers(instance.Machines, instance.Resources, instance.ResourcesCount);
        }

        private void InitSchedulers(string[] machines, string[] resources, int[] resourcesCountList)
        {
            _machineSchedulers = new Scheduler[machines.Length];
            _resourceSchedulers = new Scheduler[resources.Length];
            for (var i = 0; i < machines.Length; i++)
                _machineSchedulers[i] = new Scheduler(1, machines[i]);
            for (var i = 0; i < resources.Length; i++)
                _resourceSchedulers[i] = new Scheduler(resourcesCountList[i], resources[i]);
        }

        private bool FindSchedule(int index)
        {
            Schedule tempSchedule;
            Schedule tempDependancySchedule;
            bool success = false;
	        Test chosenTest = _instance.Tests[index];
            int length = chosenTest.Length;
	        int machineIndex = -1;
	        int resourceIndex = -1;

			for (int i = 0; i < _machineSchedulers.Length; i++)
			{
				if (chosenTest.Machines.Count != 0 && !chosenTest.Machines.Contains(_machineSchedulers[i].Name))
					continue;

				if (chosenTest.Resources.Count == 0)
				{
					if (!_machineSchedulers[i].CanFit(length, null, out tempSchedule, out _)) continue;
					success = true;
					machineIndex = i;
					if (tempSchedule.StartTime >= _instanceSchedule.StartTime) continue;
					_instanceSchedule.Copy(tempSchedule);
					_dependencySchedule = null;
				}
				else
				{
					for (int j = 0; j < _resourceSchedulers.Length; i++)
					{
						if (!chosenTest.Resources.Contains(_resourceSchedulers[j].Name)) continue;

						if (!_machineSchedulers[i].CanFit(length, _resourceSchedulers[j], out tempSchedule, out tempDependancySchedule)) continue;
						success = true;

						machineIndex = i;
						resourceIndex = j;
						if (tempSchedule.StartTime >= _instanceSchedule.StartTime) continue;
						_instanceSchedule.Copy(tempSchedule);
						_dependencySchedule?.Copy(tempDependancySchedule);
					}
				}
            }

	        Scheduler dependency = resourceIndex != -1 ? _resourceSchedulers[resourceIndex] : null;

            _machineSchedulers[machineIndex].Add(length, _instanceSchedule, dependency, _dependencySchedule);
	        _machines[index] = _machineSchedulers[machineIndex].Name;
	        _startingTimes[index] = _instanceSchedule.StartTime;
	        _endingTimes[index] = _instanceSchedule.StartTime + length;
            
            return success;
        }

        public int[] GetEndingTimes()
        {
            return _endingTimes;
        }

        public string[] GetMachines()
        {
            return _machines;
        }

        public int[] GetStartingTimes()
        {
            return _startingTimes;
        }

        public Scheduler GetMachineScheduler(int index)
        {
            return _machineSchedulers[index];
        }
        
        public Scheduler CopyMachineScheduler(int index)
        {
            var newMachineScheduler = new Scheduler(_machineSchedulers[index].ResourceCount, _machineSchedulers[index].Name);
            newMachineScheduler.Copy(_machineSchedulers[index]);
            return newMachineScheduler;
        }
        
        public Scheduler GetResourceScheduler(int index)
        {
            return _resourceSchedulers[index];
        }
        
        public Scheduler CopyResourceeScheduler(int index)
        {
            var newResourceScheduler = new Scheduler(_resourceSchedulers[index].ResourceCount, _resourceSchedulers[index].Name);
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

	    public int FirstStart()
	    {
			int minStart = int.MaxValue;

		    foreach (int start in _startingTimes)
		    {
			    if (start < minStart)
				    minStart = start;
		    }
		    return minStart;
	    }

	    public int LastEnd()
	    {
		    int maxEnd = int.MinValue;
			
		    foreach (int endingTime in _endingTimes)
		    {
			    if (endingTime > maxEnd)
				    maxEnd = endingTime;
		    }
		    return maxEnd;
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

	    public static string ParseTimes(Genome genome)
	    {
		    return "begins at: " + genome.FirstStart() + ", ends at:" + genome.LastEnd();
	    }

        public static Genome RandomGenome(Instance instance)
        {
            Random rand = new Random();
            Genome result = new Genome(instance);
            int[] randomChoice = Enumerable.Range(0, result.Size).OrderBy(x => rand.Next()).ToArray();
            for (int i = 0; i < result.Size; i++)
            {
                result.FindSchedule(randomChoice[i]);
            }
            return result;
        }
    }
}