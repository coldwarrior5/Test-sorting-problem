using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestSortingProblem.Abstract;
using TestSortingProblem.Handlers;
using TestSortingProblem.Structures;

namespace TestSortingProblem.GeneticAlgorithm
{
	public class Algorithm : GA
	{
		private const double Mortality = 0.5;
		private const int PopulationSize = 100;
		private const double MutationProbability = 0.01;
		private Solution _solution;
		
		public Algorithm(Instance instance, ExecutionTime time) : base(instance, time)
		{
			_solution = new Solution(instance.Tests.Length);
		}

		public override void Solve()
		{
			
			throw new System.NotImplementedException();
		}

		protected override void Start()
		{
			int i = 0;
			int howManyDies = (int)(Mortality * PopulationSize);
			Genome lastBest = new Genome(Instance);
			//RandomPopulation(ParamSize);
			ConsoleHandler.PrintBestGenome(BestGenome, i);
			while (true) // TODO fix this
			{
				lastBest.Copy(BestGenome);
				Parallel.For(0, howManyDies, ThreeTournament);	// Mortality determines how many times we should do the Tournaments
				DetermineBestFitness();
				if (!(BestGenome.Fitness < lastBest.Fitness)) continue;
				ConsoleHandler.PrintBestGenome(BestGenome, i);
			}
		}
		
		private void ThreeTournament(int index)
		{
			Random rnd = new Random(index);
			List<int> choices = new List<int>(3);
			while (true)
			{
				int randNum = rnd.Next(1, PopulationSize);
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

			Genome temp = new Genome(Instance);
			Order(order);
			temp.Copy(order[2]);

			Crossover(order[0], order[1], ref temp);
			for (int i = 0; i < temp.Size; i++)
			{
				if (Rand.NextDouble() < MutationProbability)
					Mutation(ref temp, i);
			}
			DetermineGenomeFitness(ref temp);
			order[2].Copy(temp);
		}

		protected override void UpdateResult()
		{
			_solution.SetTests(Instance.TestList);
			_solution.SetMachines(BestGenome.GetMachines());
			_solution.SetTimes(BestGenome.GetStartingTimes());
		}
	}
}