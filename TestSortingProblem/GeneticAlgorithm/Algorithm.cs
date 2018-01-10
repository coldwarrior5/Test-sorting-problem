using System;
using System.Collections.Generic;
using TestSortingProblem.Abstract;
using TestSortingProblem.Handlers;
using TestSortingProblem.Structures;
using System.Threading;
using System.Threading.Tasks;

namespace TestSortingProblem.GeneticAlgorithm
{
	public class Algorithm : Ga
	{
		private const double Mortality = 0.5;
		private const int PopulationSize = 100;
		private const double MutationProbability = 0.01;
		private const double MaxNoChange = 1000;

		private bool _abort;
		
		public Algorithm(Instance instance, ExecutionTime time) : base(instance, time)
		{
			_abort = false;
		}

		public override Solution Solve(bool consolePrint)
		{
            var workerThread = new Thread(() => Start(consolePrint));
			workerThread.Start();

			SetAbortSignal();
			workerThread.Join();

			Solution solution = new Solution(Structure.TestList, BestGenome.GetStartingTimes(), BestGenome.GetMachines());
			return solution;
		}

		protected override void Start(bool consolePrint)
		{
			int i = 0;
			int fromLastChange = 0;
			int howManyDies = (int)(Mortality * PopulationSize);
			Genome lastBest = new Genome(Structure);
			RandomPopulation(PopulationSize);
			if(consolePrint)
				ConsoleHandler.PrintBestGenome(BestGenome, i);
			while (Runnable(fromLastChange))
			{
				i++;
				lastBest.Copy(BestGenome);

				// Serial test
				//for(int j = 0; j < howManyDies; j++)
					//ThreeTournament(j);
				// Parallel implementation
				Parallel.For(0, howManyDies, ThreeTournament);	// Mortality determines how many times we should do the Tournaments
				
				DetermineBestFitness();
				if (!(BestGenome.Fitness < lastBest.Fitness))
				{
					fromLastChange++;
					continue;
				}
				fromLastChange = 0;
				if (consolePrint)
					ConsoleHandler.PrintBestGenome(BestGenome, i);
			}
		}
		
		private void ThreeTournament(int index)
		{
			Random rnd = new Random(index);
			List<int> choices = new List<int>(3);
			while (true)
			{
				int randNum = rnd.Next(PopulationSize);
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

			Genome temp = new Genome(Structure);
			Order(order);
			temp.Copy(order[2]);

			Crossover(order[0], order[1], ref temp);

			if (Rand.NextDouble() < MutationProbability * 10)
				Mutation(ref temp);
			
			DetermineGenomeFitness(ref temp);
			order[2].Copy(temp);
		}

		private bool Runnable(int fromLastChange)
		{
			if (Time is ExecutionTime.Unlimited)
				return fromLastChange < MaxNoChange;
			return !_abort;
		}

		private void SetAbortSignal()
		{
			int timeInMiliseconds = StringTime.Miliseconds(Time);
			if(timeInMiliseconds == 0)
				return;
			Thread.Sleep(timeInMiliseconds);
			_abort = true;
		}

		// ReSharper disable once UnusedMember.Local
		private static void CheckSchedulers(Genome bestGenome, Instance instance)
		{
			int[] length = new int[instance.ResourcesCount.Length];
			int[] counter = new int[instance.ResourcesCount.Length];
			for (int i = 0; i < counter.Length; i++)
			{
				length[i] = 0;
				counter[i] = 0;
			}

			for (int i = 0; i < instance.Tests.Length; i++)
			{
				for (int j = 0; j < instance.Tests[i].Resources.Count; j++)
				{
					int resource = -1;
					for (int k = 0; k < instance.Resources.Length; k++)
					{
						if (instance.Resources[k].Equals(instance.Tests[i].Resources[j]))
						{
							resource = k;
							break;
						}
					}
					length[resource] += instance.Tests[i].Length;
					counter[resource]++;
				}
			}

			for (int i = 0; i < counter.Length; i++)
				Console.WriteLine("Resource " + (i + 1) + " has " + counter[i] + " tests totaling length of " + length[i]);

			Scheduler[] machines = bestGenome.GetMachineScheduler();
			for (int i = 0; i < machines.Length; i++)
			{
				for (int j = 0; j < machines[i].ResourceCount; j++)
				{
					for (int k = 1; k < machines[i].ElementCount; k++)
					{
						Schedule first = machines[i].GetSchedule(j, k - 1);
						Schedule second = machines[i].GetSchedule(j, k);
						if (second.ResourceIndex - first.Place < 0)
							Console.WriteLine("Collision between " + first.StartTime + " and " + second.StartTime + " at " + (i + 1) + "machine " + machines[i].Name);
					}
				}
			}

			Scheduler[] resources = bestGenome.GetResourceScheduler();
			for (int i = 0; i < resources.Length; i++)
			{
				for (int j = 0; j < resources[i].ResourceCount; j++)
				{
					for (int k = 1; k < resources[i].ElementCount; k++)
					{
						Schedule first = resources[i].GetSchedule(j, k - 1);
						Schedule second = resources[i].GetSchedule(j, k);
						if (second.ResourceIndex - first.Place < 0)
							Console.WriteLine("Collision between " + first.StartTime + " and " + second.StartTime + " at " + (i + 1) + "resource " + resources[i].Name);
					}
				}
			}
		}
	}
}