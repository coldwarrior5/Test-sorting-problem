namespace TestSortingProblem.Structures
{
    public class Schedule
    {
	    public int ResourceIndex { get; private set; }
	    public int Place { get; private set; }
		public int StartTime { get; private set; }

		public Schedule()
        {
            ResetSchedule();
        }

        public void Copy(Schedule newState)
        {
            ResourceIndex = newState.ResourceIndex;
            Place = newState.Place;
            StartTime = newState.StartTime;
        }

	    public void SetSchedule(int resourceIndex, int resourceLocation, int startTime)
	    {
			ResourceIndex = resourceIndex;
		    Place = resourceLocation;
		    StartTime = startTime;
		}

	    public void ResetSchedule()
	    {
			ResourceIndex = 0;
		    Place = 0;
		    StartTime = int.MaxValue;
		}
    }
}