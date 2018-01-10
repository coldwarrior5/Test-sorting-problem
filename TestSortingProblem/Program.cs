using System;
using TestSortingProblem.GeneticAlgorithm;
using TestSortingProblem.Handlers;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Test sorting problem");
	        Solve(args);
			Console.WriteLine("Application finished execution");
        }

	    private static void Solve(string[] args)
	    {
			IIoHandler inputManager = new IoHandler();
		    var data = inputManager.GetParameters(args);
		    IParser parser = new Parser(data);
		    Instance instance = parser.ParseData();
		    if (data.ExecuteAlgorithm)
		    {
			    GaSettings settings = parser.ParseSettings();
				IAlgorithm algorithm = new Algorithm(instance, data.Time, settings);
			    Solution solution = algorithm.Solve(true);
			    parser.FormatAndSaveResult(solution);
			}
		    else
				ConsoleHandler.CheckResources(instance);
		}
    }
}
