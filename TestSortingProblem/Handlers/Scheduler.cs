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
                _starts[i] = new List<int>{0};
                _ends[i] = new List<int>{0};
                _tests[i] = new List<int>{-1};
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
	        schedulerInstances = schedulers is null || schedulers.Count == 0 ? null : new List<Schedule>();
			List<int> hardEnd = new List<int>();
			
            for (var i = 0; i < ResourceCount; i++)
            {
	            int currentMax = startTime;
				for (var j = 1; j < _starts[i].Count + 1; j++)
				{
					var startsAt = _ends[i][j - 1];
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
							j = j > 1 ? j - 1 : j;
						continue;
					}
					
                    // Now we need to define Schedule for our time window
                    if (schedulers is null || schedulers.Count == 0)
                    {
                        if (startsAt < instance.StartTime)
                        {
							int place = _tests[i][0] == -1 ? 0 : j;
							instance.SetSchedule(i, place, startsAt);
                            found = true;
	                        break;
                        }
                        continue;
                    }

					for (int k = 0; k < schedulers.Count; k++)
	                {
		                if (!schedulers[k].CanFit(startsAt, length, null, out var schedulerInstance, out _))
		                {
			                found = SchedulerError(schedulerInstance, i, ref currentMax, ref j);
			                break;
		                }

		                if (endsAt - schedulerInstance.StartTime < length)
		                {
							found = SchedulerError(schedulerInstance, i, ref currentMax, ref j);
							break;
		                }

		                int place = _tests[i][0] == -1 ? 0 : j;
		                int time = Math.Max(startsAt, schedulerInstance.StartTime);
						if (schedulerInstances != null && (schedulerInstances.Count <= k || time < schedulerInstances[k].StartTime))
						{
			                Tuple<int, int> designation = CheckWithOthers(time, endsAt, schedulerInstances, hardEnd, out int maxTime);
							found = designation.Item2 - designation.Item1 >= length;
							if (!found)
							{
								if (endsAt == int.MaxValue)
								{
									j = j > 1 ? j - 1 : j;
									currentMax = maxTime;
									schedulerInstances.Clear();
									hardEnd.Clear();
								}
								break;
							}
			                instance.SetSchedule(i, place, designation.Item1);
							if (schedulerInstances.Count <= k)
							{
								schedulerInstances?.Add(schedulerInstance);
								hardEnd.Add(designation.Item2);
							}
							else
							{
								schedulerInstances[k].Copy(schedulerInstance);
								hardEnd[k] = designation.Item2;
							}
								
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
			    j = j > 1 ? j - 1 : j;
		    return false;
	    }

	    private static Tuple<int,int> CheckWithOthers(int startsAt, int endsAt, List<Schedule> schedulerInstances, List<int> hardEnd, out int maxTime)
	    {
		    int absStart = int.MinValue;
		    int absEnd = int.MaxValue;
		    for (int i = 0; i < schedulerInstances.Count; i++)
		    {
			    if (schedulerInstances[i].StartTime > absStart)
				    absStart = schedulerInstances[i].StartTime;
			    if (hardEnd[i] < absEnd)
				    absEnd = hardEnd[i];
		    }
		    if (startsAt > absStart)
			    absStart = startsAt;
		    if (endsAt < absEnd)
			    absEnd = endsAt;
		    maxTime = absStart;
			return new Tuple<int, int>(absStart, absEnd);
	    }

	    private void Add(Schedule schedule, int test, int length)
        {
	        if (_tests[schedule.ResourceIndex][0] == -1)
	        {
				_starts[schedule.ResourceIndex].RemoveAt(0);
				_ends[schedule.ResourceIndex].RemoveAt(0);
				_tests[schedule.ResourceIndex].RemoveAt(0);
			}
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
	                if (_tests[i].Count != 0) return;
	                _starts[i].Add(0);
	                _ends[i].Add(0);
	                _tests[i].Add(-1);
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

				if(_starts[i].Count != 0) continue;
	            _starts[i].Add(0);
	            _ends[i].Add(0);
	            _tests[i].Add(-1);
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
			schedule.SetSchedule(_starts[resourceIndex][place], _ends[resourceIndex][place], _tests[resourceIndex][place]);
		    return schedule;
	    }
    }
}