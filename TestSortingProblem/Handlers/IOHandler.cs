using System;
using System.IO;
using System.Linq;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Handlers
{
	public class IoHandler : IoAbstract
	{
		private static readonly string[] TerminationExpressions = {"quit", "stop", "exit", "terminate", "q"};

		public override InputData GetParameters(string[] args)
		{
			InputData data;
			
			switch (args.Length)
			{
				case 0:
					data = ConsoleParameters();
					break;
				case 1:
					data = HandleArguments(args);
					break;
				case 2:
					data = HandleArguments(args);
					break;
				default:
					Console.WriteLine("Arguments cannot have other posibilities.");
					data = null;
					break;
			}
			return data;
		}

		/// <summary>
		/// Special method that ensures correct input parameters for specified problem from console arguments
		/// </summary>
		/// <param name="args">Console arguments</param>
		/// <returns>Struct defining input parameters</returns>
	    protected override InputData HandleArguments(string[] args)
        {
	        if (!File.Exists(args[0]))
		        ErrorHandler.TerminateExecution(ErrorCode.NoSuchFile, args[0]);
	        var fileName = args[0];

	        ExecutionTime time;
	        if (args.Length == 1)
		        time = ExecutionTime.Unlimited;
	        else
	        {
		        if(!StringTime.Decode(args[1], out time))
			        ErrorHandler.TerminateExecution(ErrorCode.InvalidInputParameter, "Time is not well defined");
	        }
	        return new InputData(fileName, time);
        }

		/// <summary>
		/// Special method that ensures correct input parameters for specified problem from user
		/// </summary>
		/// <returns>Struct defining input parameters</returns>
	    protected override InputData ConsoleParameters()
		{
			throw new System.NotImplementedException();
		}
		
		// ReSharper disable once UnusedMember.Local
		private static void CheckIfTerminating(string input)
		{
			if(TerminationExpressions.Contains(input))
				ErrorHandler.TerminateExecution(ErrorCode.UserTermination);
		}

	    // ReSharper disable once UnusedMember.Local
		private static string FilenameFormatter(string path, string fileName, string extension)
		{
		    // ReSharper disable once SuggestVarOrType_BuiltInTypes
			string filename = path + "/" + fileName + "." + extension;
			return filename;
		}
		
		// ReSharper disable once UnusedMember.Local
		private static void FilenameFormatter(string fullFileName, out string fileName, out string path, out string extension)
		{
			var splits = fullFileName.Split(".");
			extension = (splits.Length > 1) ? splits[splits.Length] : "";
			
			splits = fullFileName.Split("/");
			fileName = splits[splits.Length - 1].Split(".")[0];

			splits[splits.Length - 1] = "";
			path = string.Join("/", splits);
		}
	}
}