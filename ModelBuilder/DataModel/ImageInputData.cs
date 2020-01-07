using Microsoft.ML.Transforms.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ModelBuilder.DataModel
{
    public class ImageInputData
    {
        [ImageType(224, 224)]
        public Bitmap Image { get; set; }
    }
}