namespace TestSortingProblem.Structures
{
    public class Schedule
    {
	    public int ResourceIndex { get; private set; }
	    public int Place { get; private set; }
		public int StartTime { get; private set; }
	    public int EndTime { get; private set; }

		public Schedule()
        {
            ResetSchedule();
        }

        public void Copy(Schedule newState)
        {
            ResourceIndex = newState.ResourceIndex;
            Place = newState.Place;
            StartTime = newState.StartTime;
	        EndTime = newState.EndTime;
        }

	    public void SetSchedule(int resourceIndex, int resourceLocation, int startTime, int endTime)
	    {
			ResourceIndex = resourceIndex;
		    Place = resourceLocation;
		    StartTime = startTime;
		    EndTime = endTime;
	    }

	    public void ResetSchedule()
	    {
			ResourceIndex = 0;
		    Place = 0;
		    StartTime = int.MaxValue;
		    EndTime = int.MinValue;
	    }

	    public void SetStartTime(int startTime)
	    {
		    StartTime = startTime;
	    }

	    public void SetEndTime(int endTime)
	    {
			EndTime = endTime;
		}
    }
}