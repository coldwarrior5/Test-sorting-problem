using System.IO;
using TestSortingProblem.Interfaces;
using TestSortingProblem.Structures;

namespace TestSortingProblem.Handlers
{
    public class FileHandler : IFileHandler
    {
        private readonly string _inputFileName;
        private readonly string _outputFileName;
        
        public FileHandler(InputData data)
        {
            _inputFileName = data.FileName;
            IoHandler.FilenameFormatter(_inputFileName, out var path, out var fileName, out var extension);
            var newFileName = "res-" + StringTime.ToString(data.Time) + "-" + fileName;
            var outputFileName = IoHandler.FilenameFormatter(path, newFileName, extension);
            _outputFileName = outputFileName;
        }
        public string[] ReadFile()
        {
            return File.ReadAllLines(_inputFileName);
        }

        public void SaveFile(string[] outputBuffer)
        {
            File.WriteAllLines(_outputFileName, outputBuffer);
        }
    }
}