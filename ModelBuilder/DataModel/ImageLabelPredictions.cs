using Microsoft.ML.Data;

namespace ModelBuilder.DataModel
{
    public class ImageLabelPredictions
    {
        // TODO: should match TensorFlowModelSettings.outputTensorName
        [ColumnName("softmax2")]
        public float[] PredictedLabels { get; set; }
    }
}