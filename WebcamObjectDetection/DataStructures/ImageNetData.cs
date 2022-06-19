using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;

namespace WebcamObjectDetection.DataStructures
{
    public class ImageNetData
    {
        [LoadColumn(0)]
        [ColumnName("image")]
        [ImageType(416, 416)]
        public Bitmap ImagePath;
    }
}