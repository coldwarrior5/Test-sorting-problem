using System;
using System.Collections.Generic;

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
    }
}