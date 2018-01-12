using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestSortingProblem.GeneticAlgorithm;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Abstract
{
	public abstract class Ga : IAlgorithm
	{
		protected readonly Instance Structure;
		protected readonly ExecutionTime Time;
		protected Genome[] Population;
		protected readonly Genome BestGenome;
		protected readonly Random Rand;

		protected Ga(Instance structure, ExecutionTime time)
		{
			Structure = structure;
			Time = time;
			Rand = new Random();
			BestGenome = new Genome(Structure);
		}
		
		public abstract Solution Solve(bool consolePrint);

		protected abstract void Start(bool consolePrint);

		protected void RandomPopulation(int populationSize)
		{
			Population = new Genome[populationSize];
			
			
			Parallel.For(0, Population.Length, i =>
			{
				Population[i] = Genome.RandomGenome(Structure);
			});
			
			DeterminePopulationFitness();
		}

		private void DeterminePopulationFitness()
		{
			object syncObject = new object();

			Parallel.ForEach(Population, () => new Genome(Structure), (genome, loopState, localState) =>
			{
				DetermineGenomeFitness(ref genome);
				return genome.Fitness < localState.Fitness ? genome : localState;
			},
			localState =>
			{
				lock (syncObject)
				{
					if (localState.Fitness < BestGenome.Fitness)
						BestGenome.Copy(localState);
				}
			});
		}

		protected static void DetermineGenomeFitness(ref Genome genome)
		{
			genome.Fitness = genome.LastEnd() - genome.FirstStart();
		}

		protected void DetermineBestFitness()
		{
			object syncObject = new object();
			foreach (Genome t in Population)
			{
				lock (syncObject)
				{
					if (t.Fitness < BestGenome.Fitness)
						BestGenome.Copy(t);
				}
			}
		}

		protected void Crossover(Genome first, Genome second, ref Genome child)
		{
			int which = Rand.Next(4);
			switch (which)
			{
				case 0:
					CrossoverMethods.UniformLikeCrossover(first, second, ref child, Rand);
					break;
				case 1:
					CrossoverMethods.PositionBasedCrossover(first, second, ref child, Rand);
					break;
				case 2:
					CrossoverMethods.CycleCrossover(first, second, ref child, Rand);
					break;
				case 3:
					CrossoverMethods.UniformCrossover(first, second, ref child, Rand);
					break;
				default:
					child = null;
					break;
			}
		}

		protected void Mutation(ref Genome gene)
		{
			int which = Rand.Next(4);
			switch (which)
			{
				case 0:
					MutationMethods.SwapMutation(ref gene, Rand);
					break;
				case 1:
					MutationMethods.ScrambleMutation(ref gene, Rand);
					break;
				case 2:
					MutationMethods.RandomMutation(ref gene, Rand);
					break;
				case 3:
					MutationMethods.CompletelyRandomMutation(ref gene, Rand);
					break;
				default:
					gene = null;
					break;
			}
		}

		protected static void Order(List<Genome> order)
		{
			Genome temp;
			Genome temp2;
			double worstFitness = float.MinValue;
			int worstIndex = 2;
			double bestFitness = float.MaxValue;
			int bestIndex = 0;

			for (int i = 0; i < 3; i++)
			{
				if (order[i].Fitness > worstFitness)
				{
					worstFitness = order[i].Fitness;
					worstIndex = i;
				}

				if (order[i].Fitness < bestFitness)
				{
					bestFitness = order[i].Fitness;
					bestIndex = i;
				}
			}

			switch (bestIndex)
			{
				case 0 when worstIndex == 2:
					return;
				case 0:
					temp = order[worstIndex];
					order[worstIndex] = order[2];
					order[2] = temp;
					break;
				case 1 when worstIndex == 2:
					temp = order[bestIndex];
					order[bestIndex] = order[0];
					order[0] = temp;
					break;
				case 1:
					temp = order[worstIndex];
					order[worstIndex] = order[2];
					order[2] = temp;
					temp2 = order[bestIndex];
					order[bestIndex] = order[0];
					order[0] = temp2;
					break;
				case 2 when worstIndex == 0:
					temp = order[bestIndex];
					order[bestIndex] = order[0];
					order[0] = temp;
					break;
				case 2:
					temp = order[bestIndex];
					order[bestIndex] = order[0];
					order[0] = temp;
					temp2 = order[worstIndex];
					order[worstIndex] = order[2];
					order[2] = temp2;
					break;
			}
		}
	}

	public static class CrossoverMethods
	{
		public static void UniformCrossover(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Fitness = int.MaxValue;
			string[] firstParentMachines = firstParent.GetMachines();
			string[] secondParentMachines = secondParent.GetMachines();
			string[] newMachineOrder = Uniform(new List<string[]> { firstParentMachines, secondParentMachines }, rand);

			firstChild.SetMachines(newMachineOrder);
		}

		public static void UniformLikeCrossover(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Fitness = int.MaxValue;
			string[] firstParentMachines = firstParent.GetMachines();
			string[] secondParentMachines = secondParent.GetMachines();
			string[] newMachineOrder = UniformLike(new List<string[]> { firstParentMachines, secondParentMachines }, rand);

			firstChild.SetMachines(newMachineOrder);
		}

		public static void PositionBasedCrossover(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Fitness = int.MaxValue;
			string[] firstParentMachines = firstParent.GetMachines();
			string[] secondParentMachines = secondParent.GetMachines();
			string[] newMachineOrder = PositionAscertaining(new List<string[]> {firstParentMachines, secondParentMachines});

			firstChild.SetMachines(newMachineOrder);
		}

		public static void CycleCrossover(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Fitness = int.MaxValue;
			string[] firstParentMachines = firstParent.GetMachines();
			string[] secondParentMachines = secondParent.GetMachines();
			string[] newMachineOrder = Cycle(new List<string[]> {firstParentMachines, secondParentMachines});

			firstChild.SetMachines(newMachineOrder);
		}

		private static string[] Cycle(List<string[]> machines)
		{
			int parentNum = machines.Count;
			int length = machines[0].Length;
			string[] indices = new String[length];
			bool[] available = new bool[length];
			for (int i = 0; i < length; i++)
			{
				available[i] = true;
			}
			int index = 0;
			int parent = parentNum - 1;
			while (index != length)
			{
				var noChange = false;
				int tempIndex = index;
				do
				{
					var found = false;
					if (!(indices[tempIndex] is null))
					{
						noChange = true;
						continue;
					}

					indices[tempIndex] = machines[parent][tempIndex];
					for (var i = 0; i < length; i++)
					{
						if (!machines[parent][i].Equals(machines[parentNum - 1 - parent][tempIndex]) || !available[i]) continue;
						if (i != index)
							available[i] = false;
						tempIndex = i;
						found = true;
						break;
					}
					// Not found
					if (!found)
						tempIndex = index;
				} while (tempIndex != index);
				if (!noChange)
				{
					available[tempIndex] = false;
					parent = parentNum - 1 - parent;
				}
				index++;
			}
			return indices;
		}

		private static string[] PositionAscertaining(List<string[]> machines)
		{
			int parentNum = machines.Count;
			int length = machines[0].Length;
			string[] indices = new String[length];
			List<string> combination = new List<string>();
			List<int> occurences = new List<int>();
			int parent = 0;
			for (int i = 0; i < length; i++)
			{
				int index = combination.IndexOf(machines[parent][i]);
				if (index == -1)
				{
					combination.Add(machines[parent][i]);
					occurences.Add(1);
				}
				else
					occurences[index] = occurences[index] + 1;

				index = combination.IndexOf(machines[parentNum - 1 - parent][i]);
				if (index == -1)
				{
					combination.Add(machines[parentNum - 1 - parent][i]);
					occurences.Add(1);
				}
				else
					occurences[index] = occurences[index] + 1;

			}
			for (int i = 0; i < length; i++)
			{
				int firstindex = combination.IndexOf(machines[parent][i]);
				int secondindex = combination.IndexOf(machines[parentNum - 1 - parent][i]);
				if (occurences[firstindex] < occurences[secondindex])
					indices[i] = machines[parent][i];
				else
					indices[i] = machines[parentNum - 1 - parent][i];
			}
			return indices;
		}

		private static string[] Uniform(List<string[]> machines, Random rand)
		{
			int parentNum = machines.Count;
			int length = machines[0].Length;
			string[] indices = new String[length];
			int parent = 0;
			for (int i = 0; i < length; i++)
			{
				if (rand.NextDouble() < 0.5)
					indices[i] = machines[parent][i];
				else
					indices[i] = machines[parentNum - 1 - parent][i];
			}
			return indices;
		}

		private static string[] UniformLike(List<string[]> machines, Random rand)
		{
			int parentNum = machines.Count;
			int length = machines[0].Length;
			string[] indices = new String[length];
			int parent = 0;
			for (int i = 0; i < length; i++)
			{
				if (machines[parent][i].Equals(machines[parentNum - 1 - parent][i]))
					indices[i] = machines[parent][i];
			}
			for (int i = 0; i < length; i++)
			{
				if (indices[i] is null)
				{
					if (rand.NextDouble() < 0.5)
						indices[i] = machines[parent][i];
					else
						indices[i] = machines[parentNum - 1 - parent][i];
				}
			}
			return indices;
		}
	}

	public static class MutationMethods
	{
		public static void SwapMutation(ref Genome gene, Random rand)
		{
			gene.SwapPlaces(rand);
		}

		public static void ScrambleMutation(ref Genome gene, Random rand)
		{
			int firstIndex = rand.Next(gene.Size - 1);
			int secondIndex = rand.Next(firstIndex + 1, gene.Size);
			gene.ScrambleGenes(firstIndex, secondIndex, rand);
		}

		public static void RandomMutation(ref Genome gene, Random rand)
		{
			gene.Randomize(rand);
		}

		public static void CompletelyRandomMutation(ref Genome gene, Random rand)
		{
			gene.Randomize(rand, 0);
		}
	}
}