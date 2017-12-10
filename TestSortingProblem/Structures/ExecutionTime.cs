namespace TestSortingProblem.Structures
{
	public enum ExecutionTime
	{
		OneMinute = 1,
		FiveMutes = 5,
		Unlimited = 0
	}
	
	public static class StringTime
	{
		public static bool Decode(string choice, out ExecutionTime time)
		{
			var correct = true;
			switch (choice)
			{
				case "1":
					time = ExecutionTime.OneMinute;
					break;
				case "5":
					time = ExecutionTime.FiveMutes;
					break;
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
	}
}