using System.Collections.Generic;
using TestSortingProblem.Structures;

namespace TestSortingProblem
{
	public class Algorithm : AlgorithmBuilder
	{
		private const double _mortality = 0.5;
		private const int _populationSize = 100;
		private const double _mutationProbability = 0.01;
		
		public Algorithm(Instance structure) : base(structure)
		{
		}

		public override void Solve(ExecutionTime time)
		{
			
			throw new System.NotImplementedException();
		}

		protected override void Iterate()
		{
			/*
			int i = 0;
			int howManyDies = (int)(_data.Mortality * _data.PopulationSize);
			Genome lastBest = new Genome(null);
			RandomPopulation(Functions.ParamSize);
			Console.Write(i + " iteration. Current best: ");
			Program.PrintParameters(BestGenome.Genes);
			Console.WriteLine("with fitness: " + BestGenome.Fitness.ToString("G10"));
			while (BestGenome.Fitness > _data.MinError && ++i < _data.MaxIterations)
			{
				lastBest.Copy(BestGenome);
				Parallel.For(0, howManyDies, ThreeTournament);	// Mortality determines how many times we should do the Tournaments
				DetermineBestFitness();
				if (!(BestGenome.Fitness < lastBest.Fitness)) continue;
				Console.Write(i + " iteration. Current best: ");
				Program.PrintParameters(BestGenome.Genes);
				Console.WriteLine("with fitness: " +  BestGenome.Fitness.ToString("G10"));
			}
			return BestGenome;
			 */
			throw new System.NotImplementedException();
		}

		protected override void UpdateResult(List<string> results)
		{
			throw new System.NotImplementedException();
		}
	}
}