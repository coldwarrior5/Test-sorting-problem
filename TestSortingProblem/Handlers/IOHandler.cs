namespace TestSortingProblem.Handlers
{
	public class IoHandler : IoAbstract
	{
		public override void GetParameters(string[] args)
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

	    protected override void HandleArguments(string[] args)
        {
			throw new System.NotImplementedException();
		}

	    protected override void ConsoleParameters()
		{
			throw new System.NotImplementedException();
		}

	    // ReSharper disable once UnusedMember.Local
		private static string FilenameFormatter(string path, string fileName, string extension)
		{
		    // ReSharper disable once SuggestVarOrType_BuiltInTypes
			string filename = path + "/ " + fileName + "." + extension;
			return filename;
		}
	}
}