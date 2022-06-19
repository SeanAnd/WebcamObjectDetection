using Microsoft.ML.Data;

namespace WebcamObjectDetection.DataStructures
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}
