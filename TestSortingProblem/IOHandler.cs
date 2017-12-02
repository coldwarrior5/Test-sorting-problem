using TestSortingProblem.Interfaces;

namespace TestSortingProblem
{
	public class IoHandler : IIoHandler
	{
		public void GetInputParametres(string[] args)
		{
			if (args.Length == 0)
			{
				
			}
			else if(args.Length == 2)
			{
				
			}
			else if (args.Length == 3)
			{
				
			}
			else
			{
				
			}
		}

		public void ReadTask()
		{
			throw new System.NotImplementedException();
		}

		public void SaveResults()
		{
			throw new System.NotImplementedException();
		}

		private static string FilenameFormatter()
		{
			string filename = "";

			return filename;
		}
	}
}