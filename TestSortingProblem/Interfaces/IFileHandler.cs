namespace TestSortingProblem.Interfaces
{
    public interface IFileHandler
    {
        string[] ReadFile();
        void SaveFile(string[] outputBuffer);
    }
}