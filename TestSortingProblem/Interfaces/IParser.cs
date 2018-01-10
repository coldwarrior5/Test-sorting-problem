using TestSortingProblem.Structures;

namespace TestSortingProblem.Interfaces
{
    public interface IParser
    {
        Instance ParseData();
	    GaSettings ParseSettings();
		void FormatAndSaveResult(Solution result);
    }
}