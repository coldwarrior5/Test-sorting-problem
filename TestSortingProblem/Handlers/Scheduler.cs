using System;
using System.Collections.Generic;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Handlers
{
    public class Scheduler
    {
	    public readonly string Name;
		public int ResourceCount { get; }
        private List<int>[] _starts;
        private List<int>[] _ends;
        private List<int>[] _tests;

        public Scheduler(int resourceCount, string name)
        {
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
                _tests[i] = new List<int>();
            }
        }

        public bool TryAdd(int length, int test, Scheduler scheduler = null)
        {
            if (!CanFit(length, scheduler, out var our, out var another)) return false;
            
            Add(our, test, length);
            scheduler?.Add(another, test, length);
            return true;
        }
        
        public void Add(int length, int test, Schedule instance, Scheduler scheduler, Schedule dependency)
        {
            Add(instance, test, length);
            scheduler?.Add(dependency, test, length);
        }

        public bool CanFit(int length, Scheduler scheduler, out Schedule instance, out Schedule schedulerInstance)
        {
            return CanFit(-1, length, scheduler, out instance, out schedulerInstance);
        }


        public bool CanFit(int startTime, int length, Scheduler scheduler, out Schedule instance, out Schedule schedulerInstance)
        {
            var found = false;
            instance = new Schedule();
            schedulerInstance = scheduler is null ? null : new Schedule();
            
            for (var i = 0; i < ResourceCount; i++)
            {
                for (var j = 1; j < _starts[i].Count + 1; j++)
                {
	                var startsAt = _ends[i][j - 1];
	                var endsAt = j == _starts[i].Count ? int.MaxValue : _starts[i][j];

                    // Checks if the Scheduler must respect other Scheduler timeline
                    if (startTime != -1 && endsAt < startTime)
                        continue;

	                if (startTime > startsAt)
		                startsAt = startTime;
                    
                    // Now we need to ascertain time window for the Schedule to fit in
					if (endsAt - startsAt < length)
                        continue;
                    
                    // Now we need to define Schedule for our time window
                    if (scheduler == null)
                    {
                        if (startsAt < instance.StartTime)
                        {
                            instance.ResourceIndex = i;
                            instance.Place = startsAt == 0 ? 0 : j;
                            instance.StartTime = startsAt;
                            found = true;
	                        break;
                        }
                        continue;
                    }

                    if (!scheduler.CanFit(endsAt == int.MaxValue ? -1 : startsAt, length, null, out schedulerInstance, out _))
                        continue;

                    if (endsAt - schedulerInstance.StartTime >= length)
                    {
                        instance.ResourceIndex = i;
						instance.Place = startsAt == 0 ? 0 : j;
						instance.StartTime = Math.Max(startsAt, schedulerInstance.StartTime);
                        found = true;
	                    break;
                    }
                }
            }
            return found;
        }

        private void Add(Schedule schedule, int test, int length)
        {
	        if (schedule.StartTime == 0)
	        {
				_starts[schedule.ResourceIndex].RemoveAt(0);
				_ends[schedule.ResourceIndex].RemoveAt(0);
			}

			_starts[schedule.ResourceIndex].Insert(schedule.Place, schedule.StartTime);
            _ends[schedule.ResourceIndex].Insert(schedule.Place, schedule.StartTime + length);
            _tests[schedule.ResourceIndex].Insert(schedule.Place, test);
        }

        public bool Remove(Test test)
        {
            for(int i = 0; i < ResourceCount; i++)
            {
                for(int j = 0; j < _tests[i].Count; j++)
                {
                    if(_tests[i][j].Equals(test.Name))
                    {
                        _starts[i].RemoveAt(j);
                        _ends[i].RemoveAt(j);
                        _tests[i].RemoveAt(j);
                        return true;
                    }
                }
            }
            return false;
        }

        public void RemoveAfter(int time)
        {
            int removeIndex;
            for(int i = 0; i < ResourceCount; i++)
            {
                removeIndex = _starts[i].Count;
                for(int j = 0; j < _starts[i].Count; j++)
                {
                    if(_starts[i][j] >= time)
                    {
                        removeIndex = j;
                        break;
                    }
                }
                _starts[i].RemoveRange(removeIndex, _starts[i].Count - removeIndex);
                _ends[i].RemoveRange(removeIndex, _starts[i].Count - removeIndex);
                _tests[i].RemoveRange(removeIndex, _starts[i].Count - removeIndex);
            }
        }

        public void Copy(Scheduler original)
        {
            if (ResourceCount != original.ResourceCount)
                return;
            
            for (var i = 0; i < original.ResourceCount; i++)
            {
                _starts[i].AddRange(original._starts[i]);
                _ends[i].AddRange(original._ends[i]);
            }
        }
    }
}