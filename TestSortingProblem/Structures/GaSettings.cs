namespace TestSortingProblem.Structures
{
	public enum GaVariables
	{
		Mortality,
		PopulationSize,
		MutationProbability,
		MaxNoChange
	}

	public class GaSettings
	{
		public double Mortality { get; private set; }
		public int PopulationSize { get; private set; }
		public double MutationProbability { get; private set; }
		public double MaxIter { get; private set; }

		public GaSettings()
		{
			Mortality = 0.5;
			PopulationSize = 100;
			MutationProbability = 0.01;
			MaxIter = 1000;
		}

		public GaSettings(double mortality, int populationSize, double mutationProbability, int maxIter)
		{
			Mortality = mortality;
			PopulationSize = populationSize;
			MutationProbability = mutationProbability;
			MaxIter = maxIter;
		}

		public void SetMortality(double mortality)
		{
			Mortality = mortality;
		}

		public void SetPopulationSize(int populationSize)
		{
			PopulationSize = populationSize;
		}

		public void SetMutationProbability(double mutationProbability)
		{
			MutationProbability = mutationProbability;
		}

		public void SetMaxIter(int maxIter)
		{
			MaxIter = maxIter;
		}
	}	
}