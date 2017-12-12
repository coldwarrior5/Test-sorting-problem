namespace TestSortingProblem.Structures
{
    public class Schedule
    {
        public int ResourceIndex;
        public int Place;
        public int StartTime;
        
        public Schedule()
        {
            ResourceIndex = 0;
            Place = 0;
            StartTime = int.MaxValue;
        }

        public void Copy(Schedule newState)
        {
            ResourceIndex = newState.ResourceIndex;
            Place = newState.Place;
            StartTime = newState.StartTime;
        }
    }
}