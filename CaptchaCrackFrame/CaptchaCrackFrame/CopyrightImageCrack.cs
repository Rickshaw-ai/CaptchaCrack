

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using ImageCut;
using Tesseract;
using ImageCommon;
namespace CaptchaCrackFrame
{
   public class CopyrightImageCrack:ImageCrackBase
    {
        public CopyrightImageCrack(Bitmap bmp)
            :base(bmp)
        {

        }

        public override void GetBinaryImage()
        {
            //c#_具有索引像素格式的图像不支持 SetPixel,通过Bitmap.Clone()来将索引图片的像素数据拷贝到我们新建的图片上，并且在函数中指定我们新图片的pixelFormat
            Bitmap bmp = base.grayImage.Clone(new Rectangle(0, 0, base.grayImage.Width, base.grayImage.Height), PixelFormat.Format24bppRgb);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    if (color.R <= 81)      //81是针对数字的                                     
                    {
                        bmp.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        bmp.SetPixel(i, j, Color.FromArgb(255, 255, 255));//白色
                    }
                }
            }
            base.binaryImage = bmp;
        }

        public override string ImageRecognize()
        {
            try
            {
                var height = base._cutCharImageList[0].GetLength(0);
                if (height == 10)
                {
                    var addend = Convert.ToInt32(OCRImageRecognize(ImageHandle.CreateImage(base._cutCharImageList[0])));
                    height = base._cutCharImageList[1].GetLength(0);
                    if (height == 10)
                    {
                        addend = addend * 10 + Convert.ToInt32(OCRImageRecognize(ImageHandle.CreateImage(base._cutCharImageList[1])));
                    }
                    height = base._cutCharImageList[2].GetLength(0);
                    string operater = null;
                    if (height == 1)
                    {
                        operater = "-";
                    }
                    else
                    {
                        operater = "+";
                    }
                    height = base._cutCharImageList[3].GetLength(0);
                    if (height == 1)//加号及减号被分割了,那就忽略
                    {
                        height = base._cutCharImageList[5].GetLength(0);
                        if (height == 10)//说明加数是十位数
                        {
                            var add = 10 * Convert.ToInt32(OCRImageRecognize(ImageHandle.CreateImage(base._cutCharImageList[4]))) + Convert.ToInt32(OCRImageRecognize(ImageHandle.CreateImage(base._cutCharImageList[5])));
                            return AddOrSubOperator(addend, add, operater).ToString();
                        }
                        else
                        {
                            var add = Convert.ToInt32(OCRImageRecognize(ImageHandle.CreateImage(base._cutCharImageList[4])));
                            return AddOrSubOperator(addend, add, operater).ToString();
                        }
                    }
                    else
                    {
                        height = base._cutCharImageList[4].GetLength(0);
                        if (height == 10)//说明加数是十位数
                        {
                            var add = 10 * Convert.ToInt32(OCRImageRecognize(ImageHandle.CreateImage(base._cutCharImageList[3]))) + Convert.ToInt32(OCRImageRecognize(ImageHandle.CreateImage(base._cutCharImageList[4])));
                            return AddOrSubOperator(addend, add, operater).ToString();
                        }
                        else
                        {
                            var add = Convert.ToInt32(OCRImageRecognize(ImageHandle.CreateImage(base._cutCharImageList[3])));
                            return AddOrSubOperator(addend, add, operater).ToString();
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private int AddOrSubOperator(int addorsubend, int addorsub, string operation)
        {
            int result = 0;
            if (operation == "-")
            {
                result = addorsubend - addorsub;
                return result;
            }
            else
            {
                result = addorsubend + addorsub;
                return result;
            }
        }

        /// <summary>
        /// https://github.com/charlesw/tesseract
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private string OCRImageRecognize(Bitmap bitmap)
        {
            string res = "";
            using (var engine = new TesseractEngine(System.AppDomain.CurrentDomain.BaseDirectory + "/tessdata", "eng", EngineMode.Default))//path  @"./tessdata"
            {
                engine.SetVariable("tessedit_char_whitelist", "0123456789");
                // engine.SetVariable("tessedit_unrej_any_wd", true);
                using (var page = engine.Process(bitmap, PageSegMode.SingleLine))
                    res = page.GetText();
            }
            return res;
        }

        /// <summary>
        /// 识别中文汉字
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private string OCRImageRecognizeChi(Bitmap bitmap)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"./tessdata", "chi_sim", EngineMode.Default))
            {
                engine.SetVariable("tessedit_unrej_any_wd", true);
                using (var page = engine.Process(bitmap, PageSegMode.SingleLine))
                    res = page.GetText();
            }
            return res;
        }
    }
}
