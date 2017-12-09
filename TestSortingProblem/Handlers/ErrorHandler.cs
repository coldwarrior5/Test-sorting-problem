using System;

namespace TestSortingProblem.Handlers
{
    public static class ErrorHandler
    {
        public enum ErrorCode
        {
            InvalidInputParameters = 1,
            UserTermination = 2
        }

        public static void Notify(ErrorCode code, string explanation = "")
        {
            switch (code)
            {
                case ErrorCode.InvalidInputParameters:
                    Console.WriteLine("Invalid number of input parameters.");
                    break;
                case ErrorCode.UserTermination:
                    Console.WriteLine("User has terminated the application.");
                    break;
                default:
                    throw new ArgumentException("Such error is non existant");
            }
        }
    }
}