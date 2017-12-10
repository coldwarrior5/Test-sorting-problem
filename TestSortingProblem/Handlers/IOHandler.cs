using System;
using System.Collections.Generic;
using System.IO;
using TestSortingProblem.Abstract;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Handlers
{
	public class IoHandler : IoAbstract
	{
		private static readonly List<string> Extensions = new List<string> { "", "txt" };
			
		public override InputData GetParameters(string[] args)
		{
			InputData data;
			
			switch (args.Length)
			{
				case 0:
					data = ConsoleParameters();
					break;
				case 1:
				case 2:
					data = HandleArguments(args);
					break;
				default:
					ErrorHandler.TerminateExecution(ErrorCode.InvalidNumInputParameters);
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
            if (!TryExtensions(args[0], out string fileName))
		        ErrorHandler.TerminateExecution(ErrorCode.NoSuchFile, args[0]);

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
			ConsoleHandler.NotifyUserOfTermination();
			Console.WriteLine("Filename, including path, of test data.");
			var fileName = AskForFileName();
			Console.WriteLine("Execution limit? (1, 5, or 0 for unlimited. Default is unlimited)");
			var time = AskForTime();
			return new InputData(fileName, time);
		}
		
		private static string AskForFileName()
		{
			string result;
			bool correctInput;
			do
			{
				result = ConsoleHandler.AskForInput<string>();
				FilenameFormatter(result, out var path, out var fileName, out var extension);
				correctInput = extension != "" ? CheckIfFileExists(result) : TryExtensions(result, out result);
				if(!correctInput)
					Console.WriteLine("Such file does not exist");
			} while (!correctInput);
			return result;
		}

	    private static bool TryExtensions(string fileName, out string newFileName)
	    {
	        newFileName = fileName; 
	        FilenameFormatter(fileName, out var path, out var tempFileName, out _);
            bool correctFile = false;

            foreach (var ext in Extensions)
	        {
	            newFileName = FilenameFormatter(path, tempFileName, ext);
	            correctFile = CheckIfFileExists(newFileName);
	        }
	        return correctFile;
	    }

	    private static bool CheckIfFileExists(string fileName)
		{
			return File.Exists(fileName);
		}

		private static ExecutionTime AskForTime()
		{
			ExecutionTime time;
			var correctInput = false;
			do
			{
				var result = ConsoleHandler.AskForInput("");
				if (!StringTime.Decode(result, out time))
				{
					Console.WriteLine("Such time is not valid.");
					continue;
				}
				correctInput = true;
			} while (!correctInput);
			return time;
		}

	    // ReSharper disable once UnusedMember.Local
		public static string FilenameFormatter(string path, string fileName, string extension)
		{
		    // ReSharper disable once SuggestVarOrType_BuiltInTypes
			string fullFileName = (path != "") ? path + "/" : "";
			fullFileName += fileName;
			fullFileName += (extension != "") ? "." + extension : "";
			return fullFileName;
		}
		
		// ReSharper disable once UnusedMember.Local
		public static void FilenameFormatter(string fullFileName, out string path, out string fileName, out string extension)
		{
			var splits = fullFileName.Split(".");
			extension = (splits.Length > 1) ? splits[splits.Length - 1] : "";
			
			splits = fullFileName.Split("/");
			fileName = splits[splits.Length - 1].Split(".")[0];

			splits[splits.Length - 1] = "";
			path = string.Join("/", splits);
		}
	}
}