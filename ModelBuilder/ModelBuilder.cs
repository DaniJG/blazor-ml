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
        private readonly MLContext _mlContext;
        private readonly ITransformer _mlModel;

        public ModelBuilder(string tensorFlowModelFilePath)
        {
            _mlContext = new MLContext();

            // Model creation and pipeline definition for images needs to run just once, so calling it from the constructor:
            _mlModel = SetupMlnetModel(tensorFlowModelFilePath);
        }

        private ITransformer SetupMlnetModel(string tensorFlowModelFilePath)
        {
            var pipeline = _mlContext.Transforms.ResizeImages(
                    outputColumnName: TensorFlowModelSettings.inputTensorName,
                    imageWidth: ImageSettings.imageWidth,
                    imageHeight: ImageSettings.imageHeight,
                    inputColumnName: nameof(ImageInputData.Image)
                )
                .Append(_mlContext.Transforms.ExtractPixels(
                    outputColumnName: TensorFlowModelSettings.inputTensorName,
                    interleavePixelColors: ImageSettings.channelsLast,
                    offsetImage: ImageSettings.mean)
                )
                .Append(_mlContext.Model.LoadTensorFlowModel(tensorFlowModelFilePath)
                    .ScoreTensorFlowModel(
                        outputColumnNames: new[] { TensorFlowModelSettings.outputTensorName },
                        inputColumnNames: new[] { TensorFlowModelSettings.inputTensorName },
                        addBatchDimensionInput: true));

            ITransformer mlModel = pipeline.Fit(CreateEmptyDataView());

            return mlModel;
        }
        private IDataView CreateEmptyDataView()
        {
            // Create empty DataView ot Images. We just need the schema to call fit()
            return _mlContext.Data.LoadFromEnumerable<ImageInputData>(new List<ImageInputData>());
        }

        public void SaveMLNetModel(string mlnetModelFilePath)
        {
            // Save/persist the model to a .ZIP file to be loaded by the PredictionEnginePool
            _mlContext.Model.Save(_mlModel, null, mlnetModelFilePath);
        }

        public struct ImageSettings
        {
            // This has to match how the pretrained model was trained
            // Reusing the same settings from where it was downloaded: https://github.com/dotnet/machinelearning-samples/tree/master/samples/csharp/getting-started/DeepLearning_ImageClassification_TensorFlow
            public const int imageHeight = 224;
            public const int imageWidth = 224;
            public const float mean = 117;         //offsetImage
            public const bool channelsLast = true; //interleavePixelColors
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