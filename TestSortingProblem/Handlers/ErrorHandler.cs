﻿using System;

namespace TestSortingProblem.Handlers
{
    public enum ErrorCode
    {
		EarlyExit = -1,
		UserTermination = 0,
        InvalidNumInputParameters,
        InvalidInputParameter,
        NoSuchFile,
		NoFileGiven,
        UndefinedNumberOfTests,
        UndefinedNumberOfMachines,
        UndefinedNumberOfResources,
        ImproperLine,
        NotEnoughTests,
        NotEnoughMachines,
        NotEnoughResources,
        TooManyTests,
        TooManyMachines,
        TooManyResources
    }
    
    public static class ErrorHandler
    {
        public static void TerminateExecution(ErrorCode code, string explanation = "")
        {
	        if (code < 0)
	        {
				Console.WriteLine(explanation);
		        code = 0;
	        }
			else
				Console.WriteLine("\nApplication stopped.\n  Reason: " + ErrorMessage(code) + " " + explanation);
            Environment.Exit((int) code);
        }

        private static string ErrorMessage(ErrorCode code)
        {
            string explanation;
            
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
                case ErrorCode.UndefinedNumberOfTests:
                    explanation = "Undefined number of Tests.";
                    break;
                case ErrorCode.UndefinedNumberOfMachines:
                    explanation = "Undefined number of Machines.";
                    break;
                case ErrorCode.UndefinedNumberOfResources:
                    explanation = "Undefined number of Resources.";
                    break;
                case ErrorCode.ImproperLine:
                    explanation = "File contains irregular line.";
                    break;
                case ErrorCode.NotEnoughTests:
                    explanation = "Not enough tests according to specification.";
                    break;
                case ErrorCode.NotEnoughMachines:
                    explanation = "Not enough machines according to specification.";
                    break;
                case ErrorCode.NotEnoughResources:
                    explanation = "Not enough resources according to specification.";
                    break;
                case ErrorCode.TooManyTests:
                    explanation = "Too many tests according to specification.";
                    break;
                case ErrorCode.TooManyMachines:
                    explanation = "Too many machines according to specification.";
                    break;
                case ErrorCode.TooManyResources:
                    explanation = "Too many resources according to specification.";
                    break;
	            case ErrorCode.EarlyExit:
		            explanation = "";
		            break;
	            case ErrorCode.NoFileGiven:
					explanation = "Filename was not defined.";
		            break;
	            default:
                    throw new ArgumentException("Such error is non existant.");
            }
            return explanation;
        }
    }
}