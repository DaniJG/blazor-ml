using System;
using System.IO;

namespace ModelBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var tensorFlowModelPath = "TFInceptionModel/tensorflow_inception_graph.pb";
            var mlNetModelPath = "PredictionModel.zip";
            var modelBuilder = new ModelBuilder(tensorFlowModelPath);
            modelBuilder.SaveMLNetModel(mlNetModelPath);

            Console.WriteLine($"Generated {mlNetModelPath}");
        }
    }
}
