using System;

namespace TestSortingProblem.GeneticAlgorithm
{
	public class Genome
	{
		public float[] Genes;
		public float Fitness;

		public Genome(float[] genes)
		{
			Genes = genes;
			Fitness = Single.MaxValue;
		}

		public Genome(float[] genes, float fitness)
		{
			Genes = genes;
			Fitness = fitness;
		}

		public void Copy(Genome original)
		{
			Genes = FloatCopy(original.Genes);
			Fitness = original.Fitness;
		}

		private static float[] FloatCopy(float[] original)
		{
			float[] newGenes = new float[original.Length];
			for (int i = 0; i < original.Length; i++)
			{
				newGenes[i] = original[i];
			}
			return newGenes;
		}
	}
}
