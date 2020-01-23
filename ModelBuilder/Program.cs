using System;
using System.IO;

namespace ModelBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var tensorFlowModelPath = "TFInceptionModel/tensorflow_inception_graph.pb";
            var mlnetOutputZipFilePath = "PredictionModel.zip";
            var modelBuilder = new ModelBuilder(tensorFlowModelPath, mlnetOutputZipFilePath);
            modelBuilder.Run();

            Console.WriteLine($"Generated {Path.GetFullPath(mlnetOutputZipFilePath)}");
        }
    }
}
