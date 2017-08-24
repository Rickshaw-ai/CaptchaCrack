

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ImageCommon;
using ImageCut;
using Tesseract;
namespace CaptchaCrackFrame
{
   public  class DomainRecordImageCrack:ImageCrackBase
    {

        private  int[,] resetMatrix;
        private int[,] clearMatrixByWidthAndHeight;
        public DomainRecordImageCrack(Bitmap bmp)
            :base(bmp)
        {
            resetMatrix = null;
            clearMatrixByWidthAndHeight = null;
        }
        public override void GetGrayImage()
        {
            base.grayImage = ImageHandle.SobelEdgesFilters(base.originalImage);
        }
        public override void GetBinaryImage()
        {          
            base.binaryImage = base.grayImage;
        }

        public override void ClearNoise()
        {

        }
        public override void ImageCutToList()
        {
            ImageConnectCut ic = new ImageConnectCut(base.imageMatrix);
            int[,] singleMatrix = ImageHandle.GetPictureSquence(ic.ConnectedLayerAreaMatrix);
            ic = new ImageConnectCut(singleMatrix);
            resetMatrix = ImageHandle.ResetMatrix(ic.ConnectedLayerAreaMatrix);
            ic = new ImageConnectCut(resetMatrix);
            clearMatrixByWidthAndHeight = ic.ClearMarixByWidthAndHeight(22, 38, 4, 32);
            base._cutCharImageList = ic.GetCodeList();
        }

        public override string ImageRecognize()
        {
            if (base._cutCharImageList.Count == 4)
            {               
                ImageConnectCut ica = new ImageConnectCut(resetMatrix);
                clearMatrixByWidthAndHeight = ica.ClearMarixByWidthAndHeight(22, 38, 4, 64);//宽粘连
                base._cutCharImageList = ica.GetCodeList();
                if (base._cutCharImageList.Count == 4)
                {
                    ica = new ImageConnectCut(resetMatrix);
                    clearMatrixByWidthAndHeight = ica.ClearMarixByWidthAndHeight(22, 70, 4, 38);//高粘连
                    base._cutCharImageList = ica.GetCodeList();
                }
            }

            if (base._cutCharImageList.Count == 7)
            {
                return  OCRImageRecognize(ImageHandle.CreateImage(clearMatrixByWidthAndHeight)).Replace(" ", "").Trim();
            }
            else
            {
               return ImageRecognize(base._cutCharImageList);
            }
           
        }
        public string ImageRecognize(List<int[,]> matrixList)
        {
            string code = string.Empty;
            for (int i = 0; i < matrixList.Count; i++)
            {   //判断是否返回I，返回I的话直接返回去，用整体破解
                var singleCode = OCRImageRecognize(ImageHandle.CreateImage(matrixList[i])).Replace(" ", "").Replace("  ", "").Trim();
                if (singleCode == "X" || singleCode == "H")
                {
                    if (matrixList[i].GetLength(1) <= 10)
                    {
                        singleCode = "I";
                    }
                }
                if (singleCode == "C")
                {
                    if (GetBlackCount(matrixList[i]) <= 166)
                    {
                        singleCode = "C";
                    }
                    else
                    {
                        singleCode = "G";
                    }
                }
                code += singleCode;
            }
            return code;
        }


        /// <summary>
        /// https://github.com/charlesw/tesseract
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private string OCRImageRecognize(Bitmap bitmap)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                // engine.SetVariable("tessedit_unrej_any_wd", true);
                using (var page = engine.Process(bitmap, PageSegMode.SingleLine))
                    res = page.GetText();
            }
            return res;
        }
    }
}
