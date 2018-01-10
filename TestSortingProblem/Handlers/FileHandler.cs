using System.IO;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Handlers
{
    public class FileHandler : IFileHandler
    {
	    private const string OutputFolder = "Results/";
        private readonly string _inputFileName;
        private readonly string _outputFileName;

	    public FileHandler(string filename)
	    {
		    _inputFileName = filename;
	    }

        public FileHandler(InputData data)
        {
			if (!Directory.Exists(OutputFolder))
				Directory.CreateDirectory(OutputFolder);

			_inputFileName = data.FileName;
	        if (!File.Exists(data.FileName))
				ErrorHandler.TerminateExecution(ErrorCode.NoSuchFile, _inputFileName);


			IoHandler.FilenameFormatter(_inputFileName, out var path, out var fileName, out var extension);
            var newFileName = "res-" + StringTime.ToString(data.Time) + "-" + fileName;
	        string root = Path.GetPathRoot(path);
	        path = Path.Combine(root, OutputFolder);
			var outputFileName = IoHandler.FilenameFormatter(path, newFileName, extension);
            _outputFileName = outputFileName;
        }
        public string[] ReadFile()
        {
			if(File.Exists(_inputFileName))
				return File.ReadAllLines(_inputFileName);
	        return null;
        }

        public void SaveFile(string[] outputBuffer)
        {
            if(!(outputBuffer is null))
                File.WriteAllLines(_outputFileName, outputBuffer);
        }
    }
}