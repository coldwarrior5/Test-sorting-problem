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
		private static readonly string userManual = "Usage:\n  TestSortingProblem [-?]\n\t\t     [-p] path_to_filename\n\t\t     path_to_filename [time_settings]\n\nOptions:\n  -?\t\tUser manual\n  -p\t\tpath_to_filename Check minimum possible time of the task.\n  time_settings [0, 1, 5] Default 0.\n    0 Algorithm will exit on it's own.\n    1 one minute execution time.\n    5 five minutes execution time.";
		private static readonly string requestManual = "?";
		private static readonly string requestChecker = "p";
		private static readonly string spacer = "\n\n";

		public override InputData GetParameters(string[] args)
		{
			InputData data = null;
			
			switch (args.Length)
			{
				case 0:
					data = ConsoleParameters();
					break;
				case 1:
				case 2:
					IoType type = GetType(args);
					switch (type)
					{
						case IoType.Manual:
							ErrorHandler.TerminateExecution(ErrorCode.EarlyExit, spacer + userManual);
							break;
						case IoType.Resource:
						case IoType.Program:
							data = HandleArguments(args);
							break;
						case IoType.InvalidOption:
							ErrorHandler.TerminateExecution(ErrorCode.InvalidNumInputParameters, "No such option " + args[0] + spacer + userManual);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				default:
					ErrorHandler.TerminateExecution(ErrorCode.InvalidNumInputParameters, spacer + userManual);
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
			string fileName;
			IoType type = GetType(args);

			int index = type == IoType.Program ? 0 : 1;
			if(args.Length <= index)
				ErrorHandler.TerminateExecution(ErrorCode.NoFileGiven);
			if (!TryExtensions(args[index], out fileName))
		        ErrorHandler.TerminateExecution(ErrorCode.NoSuchFile, args[index]);

	        ExecutionTime time;
	        if (args.Length == 1 || index == 1)
		        time = ExecutionTime.Unlimited;
	        else
	        {
		        if(!StringTime.Decode(args[1], out time))
			        ErrorHandler.TerminateExecution(ErrorCode.InvalidInputParameter, "Time is not well defined: " + args[1] + ".");
	        }
	        return new InputData(fileName, time, index == 0);
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
			return new InputData(fileName, time, true);
		}
		
		private static string AskForFileName()
		{
			string result;
			bool correctInput;
			do
			{
				result = ConsoleHandler.AskForInput<string>();
				FilenameFormatter(result, out var _, out var _, out var extension);
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
            var correctFile = false;

            foreach (var ext in Extensions)
	        {
	            newFileName = FilenameFormatter(path, tempFileName, ext);
	            correctFile = CheckIfFileExists(newFileName);
				if(correctFile)
					break;
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

		private IoType GetType(string[] args)
		{
			if (args[0].Contains("-"))
			{
				var splits = args[0].Split('-');
				if (splits.Length != 2)
					return IoType.InvalidOption;
				if (splits[1].Equals(requestManual))
					return IoType.Manual;
				if (splits[1].Equals(requestChecker))
					return IoType.Resource;
				return IoType.InvalidOption;
			}
			return IoType.Program;
		}

		public static string FilenameFormatter(string path, string fileName, string extension)
		{
		    // ReSharper disable once SuggestVarOrType_BuiltInTypes
			string fullFileName = path;
			fullFileName += fileName;
			fullFileName += extension != "" ? "." + extension : "";
			return fullFileName;
		}
		
		// ReSharper disable once UnusedMember.Local
		public static void FilenameFormatter(string fullFileName, out string path, out string fileName, out string extension)
		{
			var splits = fullFileName.Split(".");
			extension = splits.Length > 1 ? splits[splits.Length - 1] : "";
			
			splits = fullFileName.Split("/");
			fileName = splits[splits.Length - 1].Split(".")[0];

			splits[splits.Length - 1] = "";
			path = string.Join("/", splits);
		}
	}
}