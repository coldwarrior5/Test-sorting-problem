using System;
using System.Collections.Generic;

namespace TestSortingProblem.Structures
{
    public class Scheduler
    {
        public int ResourceCount { get; }
        public int[] ResourceSize { get; set; }
        private List<int>[] _starts;
        private List<int>[] _ends;

        public Scheduler(int resourceCount)
        {
            ResourceCount = resourceCount;
            Init();
        }

        private void Init()
        {
            ResourceSize = new int[ResourceCount];
            _starts = new List<int>[ResourceCount];
            _ends = new List<int>[ResourceCount];
            
            for (var i = 0; i < ResourceCount; i++)
            {
                ResourceSize[i] = 0;
                _starts[i] = new List<int>();
                _ends[i] = new List<int>();
            }
        }

        public bool TryAdd(int length, Scheduler scheduler = null)
        {
            if (!CanFit(length, scheduler, out var our, out var another)) return false;
            
            Add(our, length);
            scheduler?.Add(another, length);
            return true;
        }
        
        public bool TryAdd(int length, Schedule instance, Scheduler scheduler, Schedule dependency)
        {
            Add(instance, length);
            scheduler?.Add(dependency, length);
            return true;
        }

        public bool CanFit(int length, Scheduler scheduler, out Schedule instance, out Schedule schedulerInstance)
        {
            return CanFit(-1, length, scheduler, out instance, out schedulerInstance);
        }


        public bool CanFit(int startTime, int length, Scheduler scheduler, out Schedule instance, out Schedule schedulerInstance)
        {
            var found = false;
            instance = new Schedule();
            schedulerInstance = (scheduler is null) ? null : new Schedule();
            
            for (var i = 0; i < ResourceCount; i++)
            {
                for (var j = 0; j < ResourceSize[i]; j++)
                {
                    // Checks if the Scheduler must respect other Scheduler timeline
                    if (startTime != -1 && _starts[i][j] < startTime)
                        continue;
                    
                    // Now we need to ascertain time window for the Schedule to fit in
                    var ends = (j == 0) ? 0 : _ends[i][j - 1];
                    if (_starts[i][j] - ends - 1 < length)
                        continue;
                    
                    // Now we need to define Schedule for our time window
                    if (scheduler == null)
                    {
                        if (_starts[i][j] < instance.StartTime)
                        {
                            instance.ResourceIndex = i;
                            instance.Place = j;
                            instance.StartTime = _starts[i][j];
                            found = true;
                        }
                        break;
                    }
                    if (!scheduler.CanFit(_starts[i][j], length, null, out schedulerInstance, out _))
                        continue;
                    if (ends - instance.StartTime -1 < length)
                    {
                        instance.ResourceIndex = i;
                        instance.Place = j;
                        instance.StartTime = Math.Max(_starts[i][j], schedulerInstance.StartTime);
                        found = true;
                    }
                    break;
                }
            }
            return found;
        }

        private void Add(Schedule schedule, int length)
        {
            _starts[schedule.ResourceIndex].Insert(schedule.Place, schedule.StartTime);
            _ends[schedule.ResourceIndex].Insert(schedule.Place, schedule.StartTime + length);
            ResourceSize[schedule.ResourceIndex]++;
        }

        private void Remove(Scheduler dependecy, Schedule thisSchedule, Schedule dependentSchedule)
        {
            _starts[thisSchedule.ResourceIndex].RemoveAt(thisSchedule.Place);
            ResourceSize[thisSchedule.ResourceIndex]--;
            dependecy?.Remove(null, dependentSchedule, null);
        }

        public void Copy(Scheduler original)
        {
            if (ResourceCount != original.ResourceCount)
                return;
            
            for (var i = 0; i < original.ResourceCount; i++)
            {
                ResourceSize[i] = original.ResourceSize[i];
                _starts[i].AddRange(original._starts[i]);
                _ends[i].AddRange(original._ends[i]);
            }
        }
    }
}