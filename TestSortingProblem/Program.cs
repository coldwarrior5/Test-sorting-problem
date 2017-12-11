using System;
using TestSortingProblem.GeneticAlgorithm;
using TestSortingProblem.Handlers;
using TestSortingProblem.Interfaces;

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
		    IAlgorithm algorithm = new Algorithm(parser.ParseData(), data.Time);
		 	algorithm.Solve();
			
		}
    }
}
