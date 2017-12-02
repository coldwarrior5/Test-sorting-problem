namespace TestSortingProblem.Interfaces
{
	public interface IIoHandler
	{
		void GetInputParametres(string[] args);
		void ReadTask();
		void SaveResults();
	}
}