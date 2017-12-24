using System;

namespace TestSortingProblem.Structures
{
	public enum ExecutionTime
	{
		OneMinute = 1,
		FiveMinutes = 5,
		Unlimited = 0
	}
	
	public static class StringTime
	{
		public static string ToString(ExecutionTime time)
		{
			string strRepresentation;
			
			switch (time)
			{
				case ExecutionTime.OneMinute:
					strRepresentation = "1m";
					break;
				case ExecutionTime.FiveMinutes:
					strRepresentation = "5m";
					break;
				case ExecutionTime.Unlimited:
					strRepresentation = "ne";
					break;
				default:
					throw new ArgumentException("No such argument");
			}
			return strRepresentation;
		}
		
		public static bool Decode(string choice, out ExecutionTime time)
		{
			var correct = true;
			switch (choice)
			{
				case "1":
					time = ExecutionTime.OneMinute;
					break;
				case "5":
					time = ExecutionTime.FiveMinutes;
					break;
				case "":
				case "0":
					time = ExecutionTime.Unlimited;
					break;
				default:
					time = ExecutionTime.Unlimited;
					correct = false;
					break;
			}
			return correct;
		}

		public static int Miliseconds(ExecutionTime time)
		{
			var timer = -1;
			switch (time)
			{
				case ExecutionTime.OneMinute:
					timer = 1 * 60 * 1000;
					break;
				case ExecutionTime.FiveMinutes:
					timer = 5 * 60 * 1000;
					break;
				case ExecutionTime.Unlimited:
					timer = 0;
					break;
				default:
					throw new ArgumentException("No such argument");
			}
			return timer;
		}
	}
}