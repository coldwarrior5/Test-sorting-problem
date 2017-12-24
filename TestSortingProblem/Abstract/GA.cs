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
		protected Instance Instance;
		protected ExecutionTime Time;
		protected Genome[] Population;
		protected readonly Genome BestGenome;
		protected readonly Random Rand;

		protected Ga(Instance structure, ExecutionTime time)
		{
			Instance = structure;
			Time = time;
			Rand = new Random();
			BestGenome = new Genome(Instance);
		}
		
		public abstract Solution Solve(bool consolePrint);

		protected abstract void Start(bool consolePrint);

		protected abstract void UpdateResult();

		protected void RandomPopulation(int populationSize)
		{
			Population = new Genome[populationSize];
			for(var i = 0; i < populationSize; i++)
			{
				Population[i] = Genome.RandomGenome(Instance);
			}
			DeterminePopulationFitness();
		}

		protected void DeterminePopulationFitness()
		{
			object syncObject = new object();

			Parallel.ForEach(Population, () => new Genome(Instance), (genome, loopState, localState) =>
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
			int which = Rand.Next(3);
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
				default:
					child = null;
					break;
			}
		}

		protected void Mutation(ref Genome gene)
		{
			int which = Rand.Next(3);
			switch (which)
			{
				case 0:
					MutationMethods.RandomMutation(ref gene, Rand);
					break;
				case 1:
					MutationMethods.SwapMutation(ref gene, Rand);
					break;
				case 2:
					MutationMethods.ScrambleMutation(ref gene, Rand);
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
		public static void UniformLikeCrossover(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Fitness = int.MaxValue;
			/*
			for (int i = 0; i < firstChild.Genes.Length; i++)
			{
				firstChild.Genes[i] = rand.NextDouble() > 0.5 ? firstParent.Genes[i] : secondParent.Genes[i];
			}
			*/
		}

		public static void PositionBasedCrossover(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			/*
			int location = rand.Next(0, firstChild.Genes.Length);
			firstChild.Copy(firstParent);
			firstChild.Fitness = Single.MaxValue;
			for (int i = location; i < firstParent.Genes.Length; i++)
			{
				firstChild.Genes[i] = Average(firstParent.Genes[i], secondParent.Genes[i]);
			}
			*/
		}

		public static void CycleCrossover(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			/*
			int location = rand.Next(0, firstChild.Genes.Length);
			firstChild.Copy(firstParent);
			firstChild.Fitness = Single.MaxValue;
			firstChild.Genes[location] = Average(firstParent.Genes[location], secondParent.Genes[location]);
			*/
		}
	}

	public static class MutationMethods
	{
		public static void SwapMutation(ref Genome gene, Random rand)
		{
			int firstIndex = rand.Next(gene.Size - 1);
			int secondIndex = rand.Next(firstIndex + 1, gene.Size);

			gene.SwapPlaces(firstIndex, secondIndex);
		}

		public static void RandomMutation(ref Genome gene, Random rand)
		{
			gene.Randomize(rand);
		}

		public static void ScrambleMutation(ref Genome gene, Random rand)
		{
			int firstIndex = rand.Next(gene.Size - 1);
			int secondIndex = rand.Next(firstIndex + 1, gene.Size);
			gene.ScrambleGenes(firstIndex, secondIndex, rand);
		}
	}
}