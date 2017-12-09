using System;
using TestSortingProblem.Handlers;
using TestSortingProblem.Interfaces;

namespace TestSortingProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test sorting problem");
	        Solve(args);
        }

	    public static void Solve(string[] args)
	    {
			IIoHandler inputManager = new IoHandler();
			inputManager.GetParameters(args);
		}
    }
}
