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
            
            for (var i = 0; i < ResourceCount; i++)
            {
                _starts[i] = new List<int>{0};
                _ends[i] = new List<int>{0};
            }
        }

        public bool TryAdd(int length, Scheduler scheduler = null)
        {
            if (!CanFit(length, scheduler, out var our, out var another)) return false;
            
            Add(our, length);
            scheduler?.Add(another, length);
            return true;
        }
        
        public void Add(int length, Schedule instance, Scheduler scheduler, Schedule dependency)
        {
            Add(instance, length);
            scheduler?.Add(dependency, length);
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

        private void Add(Schedule schedule, int length)
        {
	        if (schedule.StartTime == 0)
	        {
				_starts[schedule.ResourceIndex].RemoveAt(0);
				_ends[schedule.ResourceIndex].RemoveAt(0);
			}

			_starts[schedule.ResourceIndex].Insert(schedule.Place, schedule.StartTime);
            _ends[schedule.ResourceIndex].Insert(schedule.Place, schedule.StartTime + length);
        }

        private void Remove(Scheduler dependecy, Schedule thisSchedule, Schedule dependentSchedule)
        {
            _starts[thisSchedule.ResourceIndex].RemoveAt(thisSchedule.Place);
            dependecy?.Remove(null, dependentSchedule, null);
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