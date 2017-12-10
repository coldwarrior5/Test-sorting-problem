using System.Collections.Generic;
using TestSortingProblem.Structures;

namespace TestSortingProblem
{
	public class Algorithm : AlgorithmBuilder
	{
		private const double _mortality = 0.5;
		private const int _populationSize = 100;
		private const double _mutationProbability = 0.01;
		private List<string> _solution;
		
		public Algorithm(Instance instance, ExecutionTime time) : base(instance, time)
		{
		}

		public override void Solve()
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
		
		private void ThreeTournament(int index)
		{
			/*
			Random rnd = new Random(index);
			List<int> choices = new List<int>(3);
			while (true)
			{
				int randNum = rnd.Next(1, _data.PopulationSize);
				if (choices.Contains(randNum)) continue;
				choices.Add(randNum);
				if(choices.Count == 3)
					break;
			}

			List<Genome> order = new List<Genome>(3);
			for (int i = 0; i < 3; i++)
			{
				Genome choice = Population[choices[i]];
				order.Add(choice);
			}

			Genome temp = new Genome(order[2].Genes);
			Order(order);
			temp.Copy(order[2]);

			Crossover(order[0], order[1], ref temp);
			for (int i = 0; i < temp.Genes.Length; i++)
			{
				if (Rand.NextDouble() < _data.MutationProbability)
					Mutation(ref temp, i);
			}
			DetermineGenomeFitness(ref temp);
			order[2].Copy(temp);
			*/
		}

		protected override void UpdateResult(List<string> results)
		{
			_solution.Clear();
			_solution.AddRange(results);
		}
	}
}