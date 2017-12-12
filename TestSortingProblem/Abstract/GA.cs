using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestSortingProblem.GeneticAlgorithm;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Abstract
{
	public abstract class GA : IAlgorithm
	{
		protected Instance Instance;
		protected ExecutionTime Time;
		protected Genome[] Population;
		protected readonly Genome BestGenome;
		protected readonly Random Rand;

		protected GA(Instance structure, ExecutionTime time)
		{
			Instance = structure;
			Time = time;
			Rand = new Random();
			BestGenome = new Genome(Instance);
		}
		
		public abstract Solution Solve(bool consolePrint);

		protected abstract void Start(bool consolePrint);

		protected abstract void UpdateResult();

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

		protected static void DetermineGenomeFitness(ref Genome genome)
		{
			genome.Fitness = genome.FirstStart() - genome.LastEnd() + 1;
		}

		protected void Crossover(Genome first, Genome second, ref Genome child)
		{
			int which = Rand.Next(0, 4);
			switch (which)
			{
				case 0:
					CrossoverMethods.DiscreteRecombination(first, second, ref child, Rand);
					break;
				case 1:
					CrossoverMethods.SimpleArithmeticRecombination(first, second, ref child, Rand);
					break;
				case 2:
					CrossoverMethods.SingleArithmeticRecombination(first, second, ref child, Rand);
					break;
				case 3:
					CrossoverMethods.WholeArithmeticRecombination(first, second, ref child, Rand);
					break;
				default:
					child = null;
					break;
			}
		}

		protected int RouletteWheelSelection(Random rand)
		{
			double totalFitness = 0;
			
			foreach (Genome t in Population)
			{
				totalFitness += 1.0 / t.Fitness;
			}
			double value = rand.NextDouble() * totalFitness;
			for (int i = 0; i < Population.Length; i++)
			{
				value -= 1.0 / Population[i].Fitness;
				if (value <= 0)
					return i;
			}
			// When rounding errors occur, we return the last item's index 
			return Population.Length - 1;
		}

		protected double TotalFitness(double[] fitness)
		{
			double totalFitness = 0;
			foreach (double t in fitness)
				totalFitness += t;
			return totalFitness;
		}

		protected void Mutation(ref Genome gene, int index)
		{
			int which = Rand.Next(0, 2);
			switch (which)
			{
				case 0:
					MutationMethods.SimpleMutation(ref gene, index, Rand);
					break;
				case 1:
					MutationMethods.BoundaryMutation(ref gene, index, Rand);
					break;
				case 2:
					MutationMethods.SlightMutation(ref gene, index, Rand);
					break;
				case 3:
					MutationMethods.SwitchPlaces(ref gene, index, Rand);
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

		protected void RandomPopulation(int paramSize)
		{
			for(var i = 0; i < Population.Length; i++)
			{
				Population[i] = Genome.RandomGenome(Instance);
			};
			DeterminePopulationFitness();
		}

		protected static void RandomPopulation(int paramSize, Genome[] population)
		{
			/*
			Random rand = new Random();
			Parallel.For(0, population.Length, i =>
			{
				float[] field = new float[paramSize];
				for (int j = 0; j < paramSize; j++)
				{
					field[j] = Functions.NewParamValue(rand);
				}
				population[i] = new Genome(field);
			});
			*/
		}
	}

	public static class CrossoverMethods
	{
		public static void DiscreteRecombination(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Fitness = int.MaxValue;
			/*
			for (int i = 0; i < firstChild.Genes.Length; i++)
			{
				firstChild.Genes[i] = rand.NextDouble() > 0.5 ? firstParent.Genes[i] : secondParent.Genes[i];
			}
			*/
		}

		public static void SimpleArithmeticRecombination(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
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

		public static void SingleArithmeticRecombination(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			/*
			int location = rand.Next(0, firstChild.Genes.Length);
			firstChild.Copy(firstParent);
			firstChild.Fitness = Single.MaxValue;
			firstChild.Genes[location] = Average(firstParent.Genes[location], secondParent.Genes[location]);
			*/
		}

		public static void WholeArithmeticRecombination(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			/*
			firstChild.Fitness = Single.MaxValue;
			for (int i = 0; i < firstParent.Genes.Length; i++)
			{
				firstChild.Genes[i] = Average(firstParent.Genes[i], secondParent.Genes[i]);
			}
			*/
		}
	}

	public static class MutationMethods
	{
		public static void SimpleMutation(ref Genome gene, int index, Random rand)
		{
			
			//gene.Genes[index] = Functions.NewParamValue(rand);
			
		}

		public static void BoundaryMutation(ref Genome gene, int index, Random rand)
		{
			//gene.Genes[index] = rand.Next(0, 2) > 0 ? Functions.MaxValue : Functions.MinValue;
		}

		public static void SlightMutation(ref Genome gene, int index, Random rand)
		{
			/*
			float delta = Functions.NewParamValue(rand);
			float subtractDelta = (gene.Genes[index] - delta) % (Functions.MaxValue - Functions.MinValue) + Functions.MinValue;
			float addDelta = (gene.Genes[index] + delta) % (Functions.MaxValue - Functions.MinValue) + Functions.MinValue;
			gene.Genes[index] = rand.Next(0, 2) > 0 ? addDelta : subtractDelta;
			*/
		}

		public static void SwitchPlaces(ref Genome gene, int index, Random rand)
		{
			/*
			int index2 = rand.Next(0, gene.Genes.Length);
			float temp = gene.Genes[index2];
			gene.Genes[index2] = gene.Genes[index];
			gene.Genes[index] = temp;
			*/
		}
	}
}