

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using ImageCut;
namespace CaptchaCrackFrame
{
   public class ImageCrackBase:IImageCrack
    {
        protected Bitmap originalImage;

        protected Bitmap grayImage;

        public Bitmap GrayImage
        {
            get { return this.grayImage; }
        }

        protected Bitmap binaryImage;

        public Bitmap BinaryImage
        {
            get { return this.binaryImage; }
        }

        protected int[,] imageMatrix;

        protected List<int[,]> _cutCharImageList;

        public ImageCrackBase( Bitmap _originalImage)
        {
            originalImage = _originalImage;
            _cutCharImageList = new List<int[,]>();
        }
        public void ImageProcess()
        {
            GetGrayImage();
            GetBinaryImage();
            GetImageBinSquence();
            ClearNoise();
            ImageCutToList();
           // ImageRecognize();
        }

        public virtual void GetGrayImage()
        {            
            Bitmap bmp =this.originalImage.Clone(new Rectangle(0, 0, this.originalImage.Width, this.originalImage.Height), PixelFormat.Format24bppRgb);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)      
                {
                    Color color = bmp.GetPixel(i, j);        
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);                   
                    bmp.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            this.grayImage= bmp;
        }

        public virtual void GetBinaryImage()
        {

        }

        public virtual void GetImageBinSquence()
        {
            var width = this.binaryImage.Width;
            var height = this.binaryImage.Height;
            this.imageMatrix = new int[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = this.binaryImage.GetPixel(x, y);
                    if (color.R == 255)
                    {
                        this.imageMatrix[y, x] = 0;
                    }
                    else
                    {
                        this.imageMatrix[y, x] = 1;
                    }
                }
            }
        }

        public virtual void ClearNoise()
        {
            int nearDots = 0;
            var height = this.imageMatrix.GetLength(0);
            var width = this.imageMatrix.GetLength(1);
            var MaxNearPoints = 8;//可以为6，如果为6，则需要修改去掉点的判断
            //获得字节数组
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //判断周围8个点是否全为空
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)  //边框全去掉
                    {
                        this.imageMatrix[y, x] = 0;
                    }
                    else
                    {
                        if (this.imageMatrix[y - 1, x - 1] == 0) nearDots++;
                        if (this.imageMatrix[y, x - 1] == 0) nearDots++;
                        if (this.imageMatrix[y + 1, x - 1] == 0) nearDots++;
                        if (this.imageMatrix[y - 1, x] == 0) nearDots++;
                        if (this.imageMatrix[y + 1, x] == 0) nearDots++;
                        if (this.imageMatrix[y - 1, x + 1] == 0) nearDots++;
                        if (this.imageMatrix[y, x + 1] == 0) nearDots++;
                        if (this.imageMatrix[y + 1, x + 1] == 0) nearDots++;
                    }

                    if (nearDots == MaxNearPoints)
                    {
                        this.imageMatrix[y, x] = 0;  //去掉单点 
                    }
                    nearDots = 0;//清零   
                }

            }
        }

        public virtual void  ImageCutToList()
        {
            IImageCut iic = new HoriAndVertiCut(this.imageMatrix);
            this._cutCharImageList = iic.GetCodeList();
        }

       public virtual string ImageRecognize()
        {
            return null;
        }



        /// <summary>
        /// 统计二值化矩阵中1的个数
        /// </summary>
        /// <param name="cutcharMatrix"></param>
        /// <returns></returns>
        protected int GetBlackCount(int[,] cutcharMatrix)
        {
            var count = 0;
            for (int i = 0; i < cutcharMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < cutcharMatrix.GetLength(1); j++)
                {

                    if (cutcharMatrix[i, j] == 1)
                    {
                        count++;
                    }

                }
            }
            return count;
        }
    }
}
