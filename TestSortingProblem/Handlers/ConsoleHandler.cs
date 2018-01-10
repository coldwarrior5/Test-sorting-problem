using System;
using System.Collections.Generic;
using TestSortingProblem.GeneticAlgorithm;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Handlers
{
    public static class ConsoleHandler
    {
        private static readonly List<string> TerminationExpressions = new List<string> {"quit", "stop", "exit", "terminate", "q"};

        public static T AskForInput<T>() where T : IConvertible
        {
            bool correctInput;
            T result;
            do
            {
                var input = Console.ReadLine();
                CheckIfTerminating(input);
                correctInput = TryParse(input, out result);
            } while (!correctInput);
            return result;
        }

        public static T AskForInput<T>(T defaultValue) where T : IConvertible
        {
            var input = Console.ReadLine();
            CheckIfTerminating(input);
            var correctInput = TryParse(input, out T result);
            if (!correctInput)
                result = defaultValue;

            return result;
        }
		
        private static bool TryParse<T>(string input, out T thisType) where T: IConvertible
        {
            bool success;
            thisType = typeof(T) == typeof(string) ? (T)(object)string.Empty : default(T);
            if (thisType == null)
                return false;

            var typeCode = thisType.GetTypeCode();

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    success = bool.TryParse(input, out var b);
                    thisType = (T)Convert.ChangeType(b, typeCode);
                    break;
                case TypeCode.Double:
                    success = double.TryParse(input, out var d);
                    thisType = (T)Convert.ChangeType(d, typeCode);
                    break;
                case TypeCode.Single:
                    success = float.TryParse(input, out var f);
                    thisType = (T)Convert.ChangeType(f, typeCode);
                    break;
                case TypeCode.Int32:
                    success = int.TryParse(input, out var i);
                    thisType = (T)Convert.ChangeType(i, typeCode);
                    break;
                case TypeCode.String:
                    success = true;
                    thisType = (T)Convert.ChangeType(input, typeCode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return success;
        }
        
        public static void NotifyUserOfTermination()
        {
            Console.Write("If you wish to terminate program, use one of the following expressions: ");
            foreach (var expression in TerminationExpressions)
            {
                if(expression != null && expression != TerminationExpressions[TerminationExpressions.Count - 1])
                    Console.Write(expression + ", ");
                else
                    Console.WriteLine(expression + ".");
            }
        }
        
        // ReSharper disable once UnusedMember.Local
        private static void CheckIfTerminating(string input)
        {
            if(TerminationExpressions.Contains(input))
                ErrorHandler.TerminateExecution(ErrorCode.UserTermination);
        }

	    public static void PrintBestGenome(Genome genome, int iter = -1)
	    {
			if(iter == -1)
				Console.Write(iter + " iteration. Current best: ");
			else
				Console.Write("Best result: ");
		    
		    Console.WriteLine(Genome.ParseTimes(genome) + ". With fitness: " + genome.Fitness);
		}

	    public static void CheckResources(Instance instance)
	    {
		    Console.WriteLine("\nMinimum length of resource allocation\n");

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
				    for (var k = 0; k < instance.Resources.Length; k++)
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

		    int totalTime = 0;
		    for (var i = 0; i < counter.Length; i++)
		    {
				if(totalTime < length[i])
					totalTime = length[i];
				Console.WriteLine("Resource " + (i + 1) + " has " + counter[i] + " tests, totaling length of " + length[i]);
			}

		    Console.WriteLine("\nMinimal total time is " + totalTime);
		}
    }
}