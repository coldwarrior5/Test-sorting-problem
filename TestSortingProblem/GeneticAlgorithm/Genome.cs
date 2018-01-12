using System;
using System.Collections.Generic;
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

        public Genome(Instance instance)
        {
            Size = instance.Tests.Length;
            _instance = instance;
	        Fitness = int.MaxValue;
            InitArrays(_instance);
        }

	    private Genome(Genome other)
        {
	        Size = other.Size;
	        _instance = other._instance;
	        Fitness = other.Fitness;
			InitArrays(_instance);

			for (var i = 0; i < other.Size; i++)
	        {
		        _startingTimes[i] = other._startingTimes[i];
		        _endingTimes[i] = other._endingTimes[i];
		        _machines[i] = other._machines[i];
	        }

	        for (var i = 0; i < other._machineSchedulers.Length; i++)
	        {
		        _machineSchedulers[i] = other.CopyMachineScheduler(i);
	        }
	        for (var i = 0; i < other._resourceSchedulers.Length; i++)
	        {
		        _resourceSchedulers[i] = other.CopyResourceeScheduler(i);
	        }
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

        private void FindSchedule(int index)
        {
	        Schedule machineSchedule = new Schedule();
			List<Scheduler> dependencySchedulers = new List<Scheduler>();
	        List<Schedule> dependencySchedules = new List<Schedule>();
			Test chosenTest = _instance.Tests[index];
	        int machineIndex = -1;

			for (var i = 0; i < _machineSchedulers.Length; i++)
			{
				if (chosenTest.Machines.Count != 0 && !chosenTest.Machines.Contains(_machineSchedulers[i].Name))
					continue;

				FindSchedule(chosenTest, _machineSchedulers[i], out List<Scheduler> tempDependencySchedulers, out Schedule tempMachineSchedule, out List<Schedule> tempDependencySchedules);
				if (tempMachineSchedule.StartTime >= machineSchedule.StartTime) continue;

				machineIndex = i;
				machineSchedule.Copy(tempMachineSchedule);
				dependencySchedules = tempDependencySchedules;
				dependencySchedulers = tempDependencySchedulers;
				
			}
			
            _machineSchedulers[machineIndex].Add(chosenTest.Length, index, machineSchedule, dependencySchedulers, dependencySchedules);
	        _machines[index] = _machineSchedulers[machineIndex].Name;
	        _startingTimes[index] = machineSchedule.StartTime;
	        _endingTimes[index] = machineSchedule.StartTime + chosenTest.Length;
        }

	    private void FindSchedule(Test chosenTest, Scheduler scheduler, out List<Scheduler> dependencySchedulers, out Schedule machineSchedule, out List<Schedule> dependencySchedules)
	    {
		    machineSchedule = new Schedule();
			dependencySchedulers = new List<Scheduler>();
			dependencySchedules = new List<Schedule>();

		    foreach (Scheduler t in _resourceSchedulers)
		    {
			    if (!chosenTest.Resources.Contains(t.Name)) continue;
			    dependencySchedulers.Add(t);
		    }
			scheduler.CanFit(chosenTest.Length, dependencySchedulers, out machineSchedule, out dependencySchedules);
	    }

	    public void SwapPlaces(Random rand)
        {
	        foreach (Scheduler t in _machineSchedulers)
	        {
		        t.IndexesAfterFirstEmpties(out List<int> removeTestsIndexes);
		        for (int i = 0; i < removeTestsIndexes.Count; i++)
		        {
					RemoveTestFromMachine(removeTestsIndexes[i]);
			        if (_instance.Tests[removeTestsIndexes[i]].Resources.Count != 0)
				        RemoveTestFromResources(removeTestsIndexes[i]);
			        _startingTimes[removeTestsIndexes[i]] = -1;
			        _endingTimes[removeTestsIndexes[i]] = -1;
			        _machines[removeTestsIndexes[i]] = "";
				}
		        t.IndexesFindLastIndex(out removeTestsIndexes);
		        for (int i = 0; i < removeTestsIndexes.Count; i++)
		        {
			        RemoveTestFromMachine(removeTestsIndexes[i]);
			        if (_instance.Tests[removeTestsIndexes[i]].Resources.Count != 0)
				        RemoveTestFromResources(removeTestsIndexes[i]);
					_startingTimes[removeTestsIndexes[i]] = -1;
			        _endingTimes[removeTestsIndexes[i]] = -1;
			        _machines[removeTestsIndexes[i]] = "";
				}
			}
	        UpdateTestsAfterRemoval(rand);
		}

        public void ScrambleGenes(int firstIndex, int secondIndex, Random rand)
        {
            for(int i = firstIndex; i <= secondIndex; i++)
            {
                Test currentTest = _instance.Tests[i];
                RemoveTestFromMachine(i);
                if(currentTest.Resources.Count != 0)
                    RemoveTestFromResources(i);
            }
            int[] randomChoice = Enumerable.Range(0, secondIndex - firstIndex + 1).OrderBy(x => rand.Next()).ToArray();
            
            for (int i = 0; i < randomChoice.Length; i++)
            {
                FindSchedule(firstIndex + randomChoice[i]);
            }
        }

	    public void Randomize(Random rand, int time = -1)
        {
	        if (rand == null)
		        return;
	        time = time == -1 ? rand.Next(Fitness) : time;
            RemoveTestsAfter(time);
	        UpdateTestsAfterRemoval(rand);
        }

	    private void UpdateTestsAfterRemoval(Random rand)
	    {
		    int[] randomChoice = Enumerable.Range(0, Size).OrderBy(x => rand.Next()).ToArray();
		    for (int i = 0; i < Size; i++)
		    {
			    if (_startingTimes[randomChoice[i]] == -1)
			    {
				    FindSchedule(randomChoice[i]);
			    }
		    }
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

        public Scheduler[] GetMachineScheduler()
        {
            return _machineSchedulers;
        }
        
        private Scheduler CopyMachineScheduler(int index)
        {
            var newMachineScheduler = new Scheduler(_machineSchedulers[index].ResourceCount, _machineSchedulers[index].Name);
            newMachineScheduler.Copy(_machineSchedulers[index]);
            return newMachineScheduler;
        }
        
        public Scheduler[] GetResourceScheduler()
        {
            return _resourceSchedulers;
        }
        
        private Scheduler CopyResourceeScheduler(int index)
        {
            var newResourceScheduler = new Scheduler(_resourceSchedulers[index].ResourceCount, _resourceSchedulers[index].Name);
            newResourceScheduler.Copy(_resourceSchedulers[index]);
            return newResourceScheduler;
        }

        public void SetMachines(string[] machines)
        {
	        Random rand = new Random();
            if (machines.Length != Size)
                return;
            for (var i = 0; i < Size; i++)
				_machines[i] = machines[i];
	        foreach (Scheduler t in _machineSchedulers)
		        t.Clear();
	        foreach (Scheduler t in _resourceSchedulers)
				t.Clear();
			int[] randomChoice = Enumerable.Range(0, Size).OrderBy(x => rand.Next()).ToArray();
			int[] machineIndex = new int[Size];
	        for (int i = 0; i < Size; i++)
	        {
		        for (int j = 0; j < _instance.Machines.Length; j++)
		        {
			        if (!_machines[i].Equals(_instance.Machines[j])) continue;
			        machineIndex[i] = j;
			        break;
		        }
		        
	        }
			for (int i = 0; i < Size; i++)
	        {
		        Test chosenTest = _instance.Tests[randomChoice[i]];
		        FindSchedule(chosenTest, _machineSchedulers[machineIndex[randomChoice[i]]], out List<Scheduler> dependency, out Schedule machineSchedule, out List<Schedule> dependencySchedules);
		        _machineSchedulers[machineIndex[randomChoice[i]]].Add(chosenTest.Length, randomChoice[i], machineSchedule, dependency, dependencySchedules);
		        _startingTimes[randomChoice[i]] = machineSchedule.StartTime;
		        _endingTimes[randomChoice[i]] = machineSchedule.StartTime + chosenTest.Length;
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
        
        private void RemoveTestFromMachine(int testIndex)
        {
	        string machineName = _machines[testIndex];
	        int machineIndex = -1;
	        for (int i = 0; i < _instance.Machines.Length; i++)
	        {
		        if (_instance.Machines[i].Equals(machineName))
		        {
					machineIndex = i;
			        break;
		        }
	        }
	        _machineSchedulers[machineIndex].Remove(testIndex);
        }

        private void RemoveTestFromResources(int testIndex)
        {
			for (int i = 0; i < _instance.Resources.Length; i++)
            {
                if (!_instance.Tests[testIndex].Resources.Contains(_resourceSchedulers[i].Name)) continue;

	            _resourceSchedulers[i].Remove(testIndex);
            }
        }

        private void RemoveTestsAfter(int time)
        {
            foreach (Scheduler t in _machineSchedulers)
            {
	            t.RemoveAfter(time);
            }

            foreach (Scheduler t in _resourceSchedulers)
            {
	            t.RemoveAfter(time);
            }
            UpdateArrays(time);
        }

        private void UpdateArrays(int time)
        {
            for(int i = 0; i < Size; i++)
            {
	            if (_startingTimes[i] < time) continue;
	            _startingTimes[i] = -1;
	            _endingTimes[i] = -1;
	            _machines[i] = "";
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
			    _machineSchedulers[i] = newState.CopyMachineScheduler(i);
		    }
		    for (var i = 0; i < newState._resourceSchedulers.Length; i++)
		    {
			    _resourceSchedulers[i] = newState.CopyResourceeScheduler(i);
		    }
		    Fitness = newState.Fitness;
	    }

	    public Genome Copy()
	    {
		    return new Genome(this);
	    }

		public static string ParseTimes(Genome genome)
	    {
		    return "begins at: " + genome.FirstStart() + ", ends at: " + genome.LastEnd();
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