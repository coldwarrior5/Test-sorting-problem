using System;
using System.Collections.Generic;
using System.Diagnostics;
using TestSortingProblem.Abstract;
using TestSortingProblem.Handlers;
using TestSortingProblem.Structures;
using System.Threading;

namespace TestSortingProblem.GeneticAlgorithm
{
	public class Algorithm : Ga
	{
		private readonly GaSettings _settings;

		private bool _abort;
		private bool _abortTimer;
		private static int _currentIteration;
		private const double RefreshTime = 0.1;
		
		public Algorithm(Instance instance, ExecutionTime time, GaSettings settings) : base(instance, time)
		{
			_settings = settings;
		}

		public override Solution Solve(bool consolePrint)
		{
			_abort = false;
			_abortTimer = false;
			Console.CursorVisible = false;
            var workerThread = new Thread(() => Start(consolePrint));
			var monitorThread = new Thread(UpdateIteration);
			if (consolePrint)
				monitorThread.Start();
			workerThread.Start();
			SetAbortSignal();
			workerThread.Join();
			_abortTimer = true;
			monitorThread.Join();

			Solution solution = new Solution(Structure.TestList, BestGenome.GetStartingTimes(), BestGenome.GetMachines());
			return solution;
		}

		protected override void Start(bool consolePrint)
		{
			_currentIteration = 0;
			int fromLastChange = 0;
			int howManyDies = (int)(_settings.Mortality * _settings.PopulationSize);
			Genome lastBest = new Genome(Structure);
			RandomPopulation(_settings.PopulationSize);
			
			if (consolePrint)
				ConsoleHandler.PrintBestGenome(BestGenome, _currentIteration);
			while (Runnable(fromLastChange))
			{
				_currentIteration++;
				lastBest.Copy(BestGenome);

				// Parallel implementation
				//Parallel.For(0, howManyDies, ThreeTournament);  // Mortality determines how many times we should do the Tournaments

				for (int i = 0; i < howManyDies; i++)
					ThreeTournament(i);
				
				DetermineBestFitness();
				fromLastChange = !(BestGenome.Fitness < lastBest.Fitness) ? fromLastChange + 1 : 0;
				if(consolePrint && BestGenome.Fitness < lastBest.Fitness)
					ConsoleHandler.PrintBestGenome(BestGenome, _currentIteration);
			}
		}
		
		private void ThreeTournament(int index)
		{
			Random rnd = new Random(index + DateTime.Now.Millisecond);
			List<int> choices = new List<int>(3);
			while (true)
			{
				int randNum = rnd.Next(_settings.PopulationSize);
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
			
			Crossover(order[0], order[1], ref temp);
			
			if (Rand.NextDouble() < _settings.MutationProbability * 10) 
				Mutation(ref temp);
			
			DetermineGenomeFitness(ref temp);
			order[2].Copy(temp);
		}

		private bool Runnable(int fromLastChange)
		{
			if (Time is ExecutionTime.Unlimited)
				return fromLastChange < _settings.MaxIter;
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

		private void UpdateIteration()
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			int lastI = _currentIteration;
			while (!_abortTimer)
			{
				if (watch.Elapsed.Seconds > RefreshTime)
				{
					Console.WriteLine("Current iteration: " + _currentIteration + "\nIteration delta: " + (_currentIteration - lastI) + "    ");
					Console.SetCursorPosition(0, Console.CursorTop - 2);
					watch.Restart();
					lastI = _currentIteration;
				}
			}
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
					for (int k = 1; k < machines[i].ElementCount[j]; k++)
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
					for (int k = 1; k < resources[i].ElementCount[j]; k++)
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