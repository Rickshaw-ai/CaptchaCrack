﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace CaptchaCrackFrame
{
    public  interface IImageCrack
    {
          void  GetGrayImage();
          void GetBinaryImage();
          void GetImageBinSquence();
          void ClearNoise();
          void ImageCutToList();
          string ImageRecognize();
    }
}
