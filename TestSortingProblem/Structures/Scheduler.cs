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

        public bool CanFit(int length, Scheduler scheduler, out Schedule instance, out Schedule schedulerInstance)
        {
            var found = false;
            instance = new Schedule();
            schedulerInstance = (scheduler is null) ? null : new Schedule();
            
            for (var i = 0; i < ResourceCount; i++)
            {
                for (var j = 0; j < ResourceSize[i]; j++)
                {
                    var ends = (j == 0) ? 0 : _ends[i][j - 1];
                    if (_starts[i][j] - ends - 1 < length)
                        continue;
                    
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
                    if (!scheduler.CanFit(length, null, out schedulerInstance, out _))
                        continue;
                    if (_starts[i][j] < instance.StartTime)
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
    }
}