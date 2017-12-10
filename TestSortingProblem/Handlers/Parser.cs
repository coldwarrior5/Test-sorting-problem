using System.Collections.Generic;
using System.Text.RegularExpressions;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Handlers
{
    public class Parser : IParser
    {
        private const string Comment = "%%";
        private const string InstanceInfo = "% ";
        private const string EmptyLine = "";
        private const string Test = "test";
        private const string Machine = "embedded_board";
        private const string Resource = "resource";

        private const string TestInFile = "tests";
        private const string MachinesInFile = "machines";
        private const string ResourcesInFile = "resources";
        
        private readonly FileHandler _fileHandler;
        private Test[] _tests;
        private string[] _machines;
        private string[]_resources;
        private int[] _resourcesCount;
        
        private int _numMachines;
        private int _numTests;
        private int _numResources;

        private int _iMachines;
        private int _iTests;
        private int _iResources;
        
        public Parser(InputData data)
        {
            _fileHandler = new FileHandler(data);
            _iMachines = 0;
            _iResources = 0;
            _iTests = 0;

        }
        
        public Instance ParseData()
        {
            var data = _fileHandler.ReadFile();
            for (var i = 0; i < data.Length; i++)
            {
                ParseLine(data[i], i + 1);
            }
            CheckParameters();
            return new Instance(_tests, _machines, _resources, _resourcesCount);
        }

        private void ParseLine(string line, int position)
        {
            if (line.StartsWith(Comment) || line is EmptyLine)
                return;
            if (line.StartsWith(InstanceInfo))
                ParseInstance(line);
            else if (line.StartsWith(Test))
                ParseTest(line, position);
            else if(line.StartsWith(Machine))
                ParseMachine(line);
            else if(line.StartsWith(Resource))
                ParseResources(line);
            else
                ErrorHandler.TerminateExecution(ErrorCode.ImproperLine, "Line " + position + " is not valid.");
        }

        private void ParseInstance(string line)
        {
            var splits = line.Split(" ");
            if (line.Contains(MachinesInFile))
            {
                var success = int.TryParse(splits[splits.Length - 1], out _numMachines);
                if(!success)
                    ErrorHandler.TerminateExecution(ErrorCode.UndefinedNumberOfMachines);
                _machines = new string[_numMachines];
            }
            else if (line.Contains(TestInFile))
            {
                var success = int.TryParse(splits[splits.Length - 1], out _numTests);
                if(!success)
                    ErrorHandler.TerminateExecution(ErrorCode.UndefinedNumberOfTests);
                _tests = new Test[_numTests];
            }
            else if (line.Contains(ResourcesInFile))
            {
                var success = int.TryParse(splits[splits.Length - 1], out _numResources);
                if(!success)
                    ErrorHandler.TerminateExecution(ErrorCode.UndefinedNumberOfResources);
                _resources = new string[_numResources];
                _resourcesCount = new int[_numResources];
            }
        }

        private void ParseTest(string line, int position)
        {
            string testName = "";
            int testDuration = 0;
            List<string> machines = new List<string>();
            List<string> resources = new List<string>();
            
            if(_iTests >= _numTests)
                ErrorHandler.TerminateExecution(ErrorCode.TooManyTests);
            
            var splits = line.Split("'");
            if (splits[1] != null)
                testName = splits[1];
            else
                ErrorHandler.TerminateExecution(ErrorCode.ImproperLine, "Line " + position + " does not define test name.");

            splits = line.Split(" ");
  
            if(splits[2] != null)
            {
                var matches = Regex.Matches(splits[2], "[0-9]+");
                int.TryParse(matches[0].Value, out testDuration);
            }
            else
                ErrorHandler.TerminateExecution(ErrorCode.ImproperLine, "Line " + position + " does not define test duration.");
            if (splits[3] != null)
            {
                var matches = Regex.Matches(splits[3], "[a-zA-Z0-9]+");
                var instances = matches.GetEnumerator();
                while (instances.MoveNext())
                    machines.Add(instances.Current.ToString());
            }
            else 
                ErrorHandler.TerminateExecution(ErrorCode.ImproperLine, "Line " + position + " does not define machines.");
            if (splits[4] != null)
            {
                var matches = Regex.Matches(splits[4], "[a-zA-Z0-9]+");
                var instances = matches.GetEnumerator();
                while (instances.MoveNext())
                    resources.Add(instances.Current.ToString());
            }
            else 
                ErrorHandler.TerminateExecution(ErrorCode.ImproperLine, "Line " + position + " does not define resources.");
            
            
            _tests[_iTests++] = new Test(testName, testDuration, machines, resources);
        }

        private void ParseMachine(string line)
        {
            if(_iMachines >= _numMachines)
                ErrorHandler.TerminateExecution(ErrorCode.TooManyMachines);
            var splits = line.Split("'");
            if (splits[1] != null)
            {
                _machines[_iMachines++] = splits[1];
            }
        }

        private void ParseResources(string line)
        {
            if(_iResources >= _numResources)
                ErrorHandler.TerminateExecution(ErrorCode.TooManyResources);
            var splits = line.Split("'");
            if (splits[1] != null)
                _resources[_iResources] = splits[1];
            if (splits[2] != null)
               int.TryParse(Regex.Replace(splits[2], "[^0-9]+", string.Empty), out _resourcesCount[_iResources++]);
        }

        private void CheckParameters()
        {
            if(_iMachines < _numMachines - 1)
                ErrorHandler.TerminateExecution(ErrorCode.NotEnoughMachines);
            if(_iTests < _numTests - 1)
                ErrorHandler.TerminateExecution(ErrorCode.NotEnoughTests);
            if(_iTests < _numResources - 1)
                ErrorHandler.TerminateExecution(ErrorCode.NotEnoughResources);
        }
    }
}