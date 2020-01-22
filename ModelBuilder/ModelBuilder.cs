using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModelBuilder.DataModel;

namespace ModelBuilder
{
    public class ModelBuilder
    {
        private string _tensorFlowModelFilePath;
        private string _mlnetOutputZipFilePath;

        public ModelBuilder(string tensorFlowModelFilePath, string mlnetOutputZipFilePath)
        {
            _tensorFlowModelFilePath = tensorFlowModelFilePath;
            _mlnetOutputZipFilePath = mlnetOutputZipFilePath;
        }

        public void Run()
        {
            // Create new model context
            var mlContext = new MLContext();

            // Define the model pipeline:
            //    1. loading and resizing the image
            //    2. extracting image pixels
            //    3. running pre-trained TensorFlow model
            var pipeline = mlContext.Transforms.ResizeImages(
                    outputColumnName: TensorFlowModelSettings.inputTensorName,
                    imageWidth: ImageSettings.imageWidth,
                    imageHeight: ImageSettings.imageHeight,
                    inputColumnName: nameof(ImageInputData.Image)
                )
                .Append(mlContext.Transforms.ExtractPixels(
                    outputColumnName: TensorFlowModelSettings.inputTensorName,
                    interleavePixelColors: ImageSettings.interleavePixelColors,
                    offsetImage: ImageSettings.offsetImage)
                )
                .Append(mlContext.Model.LoadTensorFlowModel(_tensorFlowModelFilePath)
                    .ScoreTensorFlowModel(
                        outputColumnNames: new[] { TensorFlowModelSettings.outputTensorName },
                        inputColumnNames: new[] { TensorFlowModelSettings.inputTensorName },
                        addBatchDimensionInput: true));

            // Train the model
            // Since we are simply using a pre-trained TensorFlow model, we can "train" it against an empty dataset
            var emptyTrainingSet = mlContext.Data.LoadFromEnumerable<ImageInputData>(new List<ImageInputData>());
            ITransformer mlModel = pipeline.Fit(emptyTrainingSet);

            // Save/persist the model to a .ZIP file
            // This will be loaded into a PredictionEnginePool by the Blazor application, so it can classify new images
            mlContext.Model.Save(mlModel, null, _mlnetOutputZipFilePath);

            // Optional code to load he model and try it making some prediction
            DataViewSchema predictionPipelineSchema;
            mlModel = mlContext.Model.Load(_mlnetOutputZipFilePath, out predictionPipelineSchema);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ImageInputData, ImageLabelPredictions>(mlModel);
            var image = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile("../SampleImages/broccoli.jpg");
            var prediction = predictionEngine.Predict(new ImageInputData{ Image = image });
            var maxProbability = prediction.PredictedLabels.Max();
            var labelIndex = prediction.PredictedLabels.AsSpan().IndexOf(maxProbability);
            var allLabels = System.IO.File.ReadAllLines("TFInceptionModel/imagenet_comp_graph_label_strings.txt");
            Console.WriteLine($"Test image broccoli.jpg predicted as '{allLabels[labelIndex]}' with probability {100 * maxProbability}%");
        }

        public struct ImageSettings
        {
            // This has to match how the pretrained model was trained
            // Reusing the same settings from where it was downloaded: https://github.com/dotnet/machinelearning-samples/tree/master/samples/csharp/getting-started/DeepLearning_ImageClassification_TensorFlow
            public const int imageHeight = 224;
            public const int imageWidth = 224;
            public const float offsetImage = 117;
            public const bool interleavePixelColors = true;
        }

        // For checking tensor names, you can open the TF model .pb file with tools like Netron: https://github.com/lutzroeder/netron
        // which is also available online https://lutzroeder.github.io/netron/
        public struct TensorFlowModelSettings
        {
            // input tensor name
            public const string inputTensorName = "input";

            // output tensor name
            public const string outputTensorName = "softmax2";
        }
    }
}