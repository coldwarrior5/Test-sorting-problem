namespace TestSortingProblem.Structures
{
    public class InputData
    {
        public readonly string FileName;
        public readonly ExecutionTime Time;
	    public readonly bool ExecuteAlgorithm;

        public InputData(string fileName, ExecutionTime time, bool execute)
        {
            FileName = fileName;
            Time = time;
	        ExecuteAlgorithm = execute;
        }
    }
}