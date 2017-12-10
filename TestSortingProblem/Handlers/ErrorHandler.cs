using System;

namespace TestSortingProblem.Handlers
{
    public enum ErrorCode
    {
        InvalidNumInputParameters,
        InvalidInputParameter,
        UserTermination,
        NoSuchFile
    }
    
    public static class ErrorHandler
    {
        public static void TerminateExecution(ErrorCode code, string explanation = "")
        {
            Console.WriteLine("Application stopped.\nReason: " + ErrorMessage(code) + " " + explanation);
            Environment.Exit((int) code);
        }

        private static string ErrorMessage(ErrorCode code)
        {
            var explanation = "";
            
            switch (code)
            {
                case ErrorCode.InvalidNumInputParameters:
                    explanation = "Invalid number of input parameters.";
                    break;
                case ErrorCode.UserTermination:
                    explanation = "User has terminated the application.";
                    break;
                case ErrorCode.NoSuchFile:
                    explanation = "There is no such file.";
                    break;
                case ErrorCode.InvalidInputParameter:
                    explanation = "Invalid input parameter.";
                    break;
                default:
                    throw new ArgumentException("Such error is non existant.");
            }
            return explanation;
        }
    }
}