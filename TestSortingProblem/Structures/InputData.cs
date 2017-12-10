namespace TestSortingProblem.Structures
{
    public class InputData
    {
        public readonly string FileName;
        public readonly ExecutionTime Time;

        public InputData(string fileName, ExecutionTime time)
        {
            FileName = fileName;
            Time = time;
        }
    }
}