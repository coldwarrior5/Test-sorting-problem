using System;
using System.Collections.Generic;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Handlers
{
    public class Scheduler
    {
	    public readonly string Name;
		public int ResourceCount { get; }
		public int ElementCount { get; private set; }
        private List<int>[] _starts;
        private List<int>[] _ends;
        private List<int>[] _tests;

        public Scheduler(int resourceCount, string name)
        {
	        ElementCount = 0;
			ResourceCount = resourceCount;
	        Name = name;
            Init();
        }

        private void Init()
        {
            _starts = new List<int>[ResourceCount];
            _ends = new List<int>[ResourceCount];
            _tests = new List<int>[ResourceCount];
            
            for (var i = 0; i < ResourceCount; i++)
            {
                _starts[i] = new List<int>();
                _ends[i] = new List<int>();
	            _tests[i] = new List<int>();
            }
        }

        public bool TryAdd(int length, int test, List<Scheduler> schedulers)
        {
            if (!CanFit(length, schedulers, out var our, out var another)) return false;
			
            Add(our, test, length);
	        for (int i = 0; i < schedulers.Count; i++)
		        schedulers[i].Add(another[i], test, length);
            return true;
        }
        
        public void Add(int length, int test, Schedule instance, List<Scheduler> schedulers, List<Schedule> dependencies)
        {
			Add(instance, test, length);
			if(schedulers is null || schedulers.Count != dependencies.Count) return;
	        for (int i = 0; i < schedulers.Count; i++)
		        schedulers[i].Add(dependencies[i], test, length);
        }

        public bool CanFit(int length, List<Scheduler> scheduler, out Schedule instance, out List<Schedule> schedulerInstances)
        {
            return CanFit(-1, length, scheduler, out instance, out schedulerInstances);
        }


        public bool CanFit(int startTime, int length, List<Scheduler> schedulers, out Schedule instance, out List<Schedule> schedulerInstances)
        {
            var found = false;
	        instance = new Schedule();
	        schedulerInstances = null;

			for (var i = 0; i < ResourceCount; i++)
            {
				schedulerInstances = new List<Schedule>();
				int currentMax = startTime;
				for (var j = 0; j < _starts[i].Count + 1; j++)
				{
					var startsAt = j == 0 ? 0 : _ends[i][j - 1];
					if(j == _starts[i].Count && currentMax > startTime)
						startTime = currentMax;
	                var endsAt = j == _starts[i].Count ? int.MaxValue : _starts[i][j];

                    // Checks if the Scheduler must respect other Scheduler timeline
                    if (startTime != -1 && endsAt < startTime)
                        continue;

	                if (startsAt < startTime)
						startsAt = startTime;

					// Update max starting point when looping
					if (startsAt > currentMax)
						currentMax = startsAt;
                    
                    // Now we need to ascertain time window for the Schedule to fit in
					if (endsAt - startsAt < length)
					{
						if(j == _starts[i].Count)
							j--;
						continue;
					}
					
                    // Now we need to define Schedule for our time window
                    if (schedulers is null || schedulers.Count == 0)
                    {
                        if (startsAt < instance.StartTime)
                        {
							instance.SetSchedule(i, j, startsAt, endsAt);
                            found = true;
	                        break;
                        }
                        continue;
                    }

					for (int k = 0; k < schedulers.Count; k++)
	                {
		                if (!schedulers[k].CanFit(startsAt, length, null, out var schedulerInstance, out _))
		                {
							schedulerInstance.SetStartTime(int.MinValue);
			                found = SchedulerError(schedulerInstance, i, ref currentMax, ref j);
			                break;
		                }

		                if (endsAt - schedulerInstance.StartTime < length)
		                {
							found = SchedulerError(schedulerInstance, i, ref currentMax, ref j);
							break;
		                }
						
		                int laterStart = Math.Max(startsAt, schedulerInstance.StartTime);
						int priorEnd = Math.Min(endsAt, schedulerInstance.EndTime);
						if (schedulerInstances.Count <= k || laterStart < schedulerInstances[k].StartTime)
						{
			                Tuple<int, int> designation = CheckWithOthers(laterStart, priorEnd, schedulerInstances);
							found = designation.Item2 - designation.Item1 >= length;
							if (!found)
							{
								if (j == _starts[i].Count)
								{
									j--;
									if(designation.Item1 > currentMax)
										currentMax = designation.Item1;
								}
								schedulerInstances.Clear();
								break;
							}
							instance.SetSchedule(i, j, designation.Item1, designation.Item2);
							schedulerInstance.SetStartTime(designation.Item1);
							schedulerInstance.SetEndTime(priorEnd);
							schedulerInstances.Add(schedulerInstance);
						}
	                }
	                if (found)
		                break;
                }
            }
            return found;
        }

	    private bool SchedulerError(Schedule schedulerInstance, int i, ref int currentMax, ref int j)
	    {
		    if (schedulerInstance.StartTime > currentMax)
			    currentMax = schedulerInstance.StartTime;
		    if (j == _starts[i].Count)
			    j--;
		    return false;
	    }

	    private static Tuple<int,int> CheckWithOthers(int startsAt, int endsAt, List<Schedule> schedulerInstances)
	    {
		    int absStart = int.MinValue;
		    int absEnd = int.MaxValue;
		    foreach (Schedule t in schedulerInstances)
		    {
			    if (t.StartTime > absStart)
				    absStart = t.StartTime;
			    if (t.EndTime < absEnd)
				    absEnd = t.EndTime;
		    }
		    if (startsAt > absStart)
			    absStart = startsAt;
		    if (endsAt < absEnd)
			    absEnd = endsAt;
		    foreach (Schedule t in schedulerInstances)
		    {
			    if (t.StartTime < absStart)
				    t.SetStartTime(absStart);
		    }
			return new Tuple<int, int>(absStart, absEnd);
	    }

	    private void Add(Schedule schedule, int test, int length)
        {
	        ElementCount++;
			_starts[schedule.ResourceIndex].Insert(schedule.Place, schedule.StartTime);
            _ends[schedule.ResourceIndex].Insert(schedule.Place, schedule.StartTime + length);
            _tests[schedule.ResourceIndex].Insert(schedule.Place, test);
        }

        public void Remove(int testIndex)
        {
	        ElementCount--;
            for(var i = 0; i < ResourceCount; i++)
            {
                for(var j = 0; j < _tests[i].Count; j++)
                {
                    if (_tests[i][j] != testIndex) continue;
                    _starts[i].RemoveAt(j);
                    _ends[i].RemoveAt(j);
                    _tests[i].RemoveAt(j);
	                return;
                }
            }
        }

        public void RemoveAfter(int time)
        {
	        for(int i = 0; i < ResourceCount; i++)
            {
	            int removeIndex = -1;
				for (int j = 0; j < _starts[i].Count; j++)
                {
                    if(_starts[i][j] >= time)
                    {
                        removeIndex = j;
                        break;
                    }
                }
	            if (removeIndex == -1) continue;

	            int difference = _starts[i].Count - removeIndex;
	            ElementCount -= difference;
                _starts[i].RemoveRange(removeIndex, difference);
                _ends[i].RemoveRange(removeIndex, difference);
                _tests[i].RemoveRange(removeIndex, difference);
			}
        }

	    public void Clear()
	    {
			for (int i = 0; i < ResourceCount; i++)
			{
				ElementCount = 0;
				_starts[i].Clear();
				_ends[i].Clear();
				_tests[i].Clear();
			}
		}

        public void Copy(Scheduler original)
        {
            if (ResourceCount != original.ResourceCount)
                return;

	        ElementCount = original.ElementCount;
            
            for (var i = 0; i < original.ResourceCount; i++)
            {
				_starts[i].Clear();
                _starts[i].AddRange(original._starts[i]);
				_ends[i].Clear();
                _ends[i].AddRange(original._ends[i]);
				_tests[i].Clear();
				_tests[i].AddRange(original._tests[i]);
            }
        }

	    public Schedule GetSchedule(int resourceIndex, int place)
	    {
			Schedule schedule = new Schedule();
			schedule.SetSchedule(_starts[resourceIndex][place], _ends[resourceIndex][place], _tests[resourceIndex][place], 0);
		    return schedule;
	    }
    }
}