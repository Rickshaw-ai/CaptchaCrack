

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using ImageCut;

namespace ImageCommon
{
   public class ImageHandle
    {
        /// <summary>
        /// 将二值化二维矩阵生成图像
        /// </summary>
        /// <param name="singleCharMatrix"></param>
        /// <returns></returns>
        public static Bitmap CreateImage(int[,] CharMatrix)
        {
            Bitmap bitmap = new Bitmap(CharMatrix.GetLength(1), CharMatrix.GetLength(0));
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {

                    if (CharMatrix[j, i] != 1)
                    {
                        bitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));//白色
                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                }
            }
            return bitmap;
        }

        public static Bitmap ApplyFilter(Bitmap bmp)
        {
            return new Invert().Apply(bmp);
        }

        public static Bitmap EroseImage(Bitmap bit)
        {
            Bitmap bmp = bit.Clone(new Rectangle(0, 0, bit.Width, bit.Height), PixelFormat.Format24bppRgb);

            Erosion erosion = new Erosion();

            return erosion.Apply(bmp);
        }

        public static Bitmap DilatImage(Bitmap bit)
        {
            Bitmap bmp = bit.Clone(new Rectangle(0, 0, bit.Width, bit.Height), PixelFormat.Format24bppRgb);

            Dilatation dila = new Dilatation();

            return dila.Apply(bmp);
        }
        /// <summary>
        /// 先膨胀再腐蚀，可以填充漏洞（中间为0的像素）,官方下面使用方法效果不好
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static Bitmap DilatAndEroseImage(Bitmap bit)
        {
            Bitmap bmp = bit.Clone(new Rectangle(0, 0, bit.Width, bit.Height), PixelFormat.Format24bppRgb);

            Dilatation dila = new Dilatation();
            dila.Apply(bmp);
            Erosion erosion = new Erosion();

            return erosion.Apply(bmp);
        }

        /// <summary>
        /// 先膨胀再腐蚀，可以填充漏洞（中间为0的像素）  http://blog.ostermiller.org/dilate-and-erode
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static int[,] DilatImageDIY(int[,] matrix, int dilatOrErode, int erodeOrDilat)//1周边是0的元素都被标记为2，否则会无限拓展，最后再将2全部替换为1
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (i == 0 && j == 0 && matrix[i, j] == dilatOrErode)//第一个点
                    {
                        matrix[i, j + 1] = SetLableValue(matrix[i, j + 1], erodeOrDilat);
                        matrix[i + 1, j] = SetLableValue(matrix[i + 1, j], erodeOrDilat);
                    }
                    if (i == 0 && j > 0 && j < matrix.GetLength(1) - 1 && matrix[i, j] == dilatOrErode)//第一个行除去两边点
                    {

                        matrix[i, j - 1] = SetLableValue(matrix[i, j - 1], erodeOrDilat);
                        matrix[i, j + 1] = SetLableValue(matrix[i, j + 1], erodeOrDilat);
                        matrix[i + 1, j] = SetLableValue(matrix[i + 1, j], erodeOrDilat);
                    }
                    if (i == 0 && j == matrix.GetLength(1) - 1 && matrix[i, j] == dilatOrErode)//右上角顶点
                    {

                        matrix[i, j - 1] = SetLableValue(matrix[i, j - 1], erodeOrDilat);
                        matrix[i + 1, j] = SetLableValue(matrix[i + 1, j], erodeOrDilat);
                    }

                    if (j == 0 && i > 0 && i < matrix.GetLength(0) - 1 && matrix[i, j] == dilatOrErode)//左边第一列
                    {
                        matrix[i - 1, j] = SetLableValue(matrix[i - 1, j], erodeOrDilat);
                        matrix[i + 1, j] = SetLableValue(matrix[i + 1, j], erodeOrDilat);
                        matrix[i, j + 1] = SetLableValue(matrix[i, j + 1], erodeOrDilat);
                    }
                    if (j == matrix.GetLength(1) - 1 && i > 0 && i < matrix.GetLength(0) - 1 && matrix[i, j] == dilatOrErode)//右边最后一列
                    {
                        matrix[i - 1, j] = SetLableValue(matrix[i - 1, j], erodeOrDilat);
                        matrix[i + 1, j] = SetLableValue(matrix[i + 1, j], erodeOrDilat);
                        matrix[i, j - 1] = SetLableValue(matrix[i, j - 1], erodeOrDilat);
                    }
                    if (i == matrix.GetLength(0) - 1 && j == 0 && matrix[i, j] == dilatOrErode)//左下角
                    {
                        matrix[i - 1, j] = SetLableValue(matrix[i - 1, j], erodeOrDilat);
                        matrix[i, j + 1] = SetLableValue(matrix[i, j + 1], erodeOrDilat);
                    }
                    if (i == matrix.GetLength(0) - 1 && j == matrix.GetLength(1) - 1 && matrix[i, j] == dilatOrErode)//右下角
                    {
                        matrix[i, j - 1] = SetLableValue(matrix[i, j - 1], erodeOrDilat);
                        matrix[i - 1, j] = SetLableValue(matrix[i - 1, j], erodeOrDilat);
                    }
                    if (i == matrix.GetLength(0) - 1 && j > 0 && j < matrix.GetLength(1) - 1 && matrix[i, j] == dilatOrErode)//最后一行
                    {
                        matrix[i, j - 1] = SetLableValue(matrix[i, j - 1], erodeOrDilat);
                        matrix[i, j + 1] = SetLableValue(matrix[i, j + 1], erodeOrDilat);
                        matrix[i - 1, j] = SetLableValue(matrix[i - 1, j], erodeOrDilat);
                    }

                    if (i > 0 && i < matrix.GetLength(0) - 1 && j > 0 && j < matrix.GetLength(1) - 1 && matrix[i, j] == dilatOrErode)//中间点
                    {
                        matrix[i, j - 1] = SetLableValue(matrix[i, j - 1], erodeOrDilat);
                        matrix[i, j + 1] = SetLableValue(matrix[i, j + 1], erodeOrDilat);
                        matrix[i + 1, j] = SetLableValue(matrix[i + 1, j], erodeOrDilat);
                        matrix[i - 1, j] = SetLableValue(matrix[i - 1, j], erodeOrDilat);
                    }
                }

            }
            //将标记2替换为1
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 2)
                    {
                        matrix[i, j] = dilatOrErode;
                    }
                }
            }

            return matrix;
        }

        private static int SetLableValue(int singlePointValue, int erodeOrDilat)
        {
            if (singlePointValue == erodeOrDilat)
            {
                singlePointValue = 2;
            }
            return singlePointValue;
        }

        public static Bitmap ThresholdBinarization(Bitmap bmp)
        {
            bmp = Grayscale.CommonAlgorithms.RMY.Apply(bmp);
            return new Threshold().Apply(bmp);
        }

        /// <summary>
        ///细化算法 http://www.cnblogs.com/mikewolf2002/p/3327318.html
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static int[,] Rosenfeld(int[,] matrix)
        {

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (i == 0 && j == 0 && matrix[i, j] == 1)//第一个点
                    {
                        var sum = matrix[i + 1, j] + matrix[i, j + 1] + matrix[i + 1, j + 1];
                        if (sum >= 2)//只有4个点，所以可以直接判断设置为0 是否影响连通性
                        {
                            matrix[i, j] = 0;
                        }
                    }
                    if (i == 0 && j > 0 && j < matrix.GetLength(1) - 1 && matrix[i, j] == 1)//第一行除去两边点
                    {
                        var sum = matrix[i, j - 1] + matrix[i, j + 1] + matrix[i + 1, j] + matrix[i + 1, j - 1] + matrix[i + 1, j + 1];
                        if (sum >= 2)//需要使用连通域切割判断去除是否对8连通有影响
                        {
                            if (matrix[i, j - 1] == 0 || matrix[i, j + 1] == 0 || matrix[i + 1, j] == 0)
                            {
                                int[,] matrixSix = new int[,] { { matrix[i, j - 1], matrix[i, j], matrix[i, j + 1] }, { matrix[i + 1, j - 1], matrix[i + 1, j], matrix[i + 1, j + 1] } };

                                if (!IsImpactConnect(matrixSix, 1))
                                {
                                    matrix[i, j] = 0;
                                }
                            }
                        }
                    }
                    if (i == 0 && j == matrix.GetLength(1) - 1 && matrix[i, j] == 1)//右上角顶点
                    {
                        var sum = matrix[i, j - 1] + matrix[i + 1, j] + matrix[i + 1, j - 1];
                        if (sum >= 2)
                        {
                            matrix[i, j] = 0;
                        }
                    }

                    if (j == 0 && i > 0 && i < matrix.GetLength(0) - 1 && matrix[i, j] == 1)//左边第一列
                    {
                        var sum = matrix[i - 1, j] + matrix[i + 1, j] + matrix[i, j + 1] + matrix[i - 1, j + 1] + matrix[i + 1, j + 1];
                        if (sum >= 2)
                        {
                            if (matrix[i - 1, j] == 0 || matrix[i + 1, j] == 0 || matrix[i, j + 1] == 0)
                            {
                                int[,] matrixSix = new int[,] { { matrix[i - 1, j], matrix[i - 1, j + 1] }, { matrix[i, j], matrix[i, j + 1] }, { matrix[i + 1, j], matrix[i + 1, j + 1] } };
                                if (!IsImpactConnect(matrixSix, 2))
                                {
                                    matrix[i, j] = 0;
                                }
                            }
                        }
                    }
                    if (j == matrix.GetLength(1) - 1 && i > 0 && i < matrix.GetLength(0) - 1 && matrix[i, j] == 1)//右边最后一列
                    {

                        var sum = matrix[i - 1, j] + matrix[i + 1, j] + matrix[i, j - 1] + matrix[i - 1, j - 1] + matrix[i + 1, j - 1];
                        if (sum >= 2)
                        {
                            if (matrix[i - 1, j] == 0 || matrix[i + 1, j] == 0 || matrix[i, j - 1] == 0)
                            {
                                int[,] matrixSix = new int[,] { { matrix[i - 1, j - 1], matrix[i - 1, j] }, { matrix[i, j - 1], matrix[i, j] }, { matrix[i + 1, j - 1], matrix[i + 1, j] } };
                                if (!IsImpactConnect(matrixSix, 3))
                                {
                                    matrix[i, j] = 0;
                                }
                            }
                        }

                    }
                    if (i == matrix.GetLength(0) - 1 && j == 0 && matrix[i, j] == 1)//左下角
                    {
                        var sum = matrix[i - 1, j] + matrix[i, j + 1] + matrix[i - 1, j + 1];
                        if (sum >= 2)
                        {
                            matrix[i, j] = 0;
                        }
                    }
                    if (i == matrix.GetLength(0) - 1 && j == matrix.GetLength(1) - 1 && matrix[i, j] == 1)//右下角
                    {
                        var sum = matrix[i, j - 1] + matrix[i - 1, j] + matrix[i - 1, j - 1];
                        if (sum >= 2)
                        {
                            matrix[i, j] = 0;
                        }
                    }
                    if (i == matrix.GetLength(0) - 1 && j > 0 && j < matrix.GetLength(1) - 1 && matrix[i, j] == 1)//最后一行
                    {
                        var sum = matrix[i, j - 1] + matrix[i, j + 1] + matrix[i - 1, j] + matrix[i - 1, j - 1] + matrix[i - 1, j + 1];
                        if (sum >= 2)
                        {
                            if (matrix[i, j - 1] == 0 || matrix[i, j + 1] == 0 || matrix[i - 1, j] == 0)
                            {
                                int[,] matrixSix = new int[,] { { matrix[i - 1, j + 1], matrix[i - 1, j], matrix[i - 1, j + 1] }, { matrix[i, j - 1], matrix[i, j], matrix[i, j + 1] } };

                                if (!IsImpactConnect(matrixSix, 4))
                                {
                                    matrix[i, j] = 0;
                                }
                            }
                        }

                    }

                    if (i > 0 && i < matrix.GetLength(0) - 1 && j > 0 && j < matrix.GetLength(1) - 1 && matrix[i, j] == 1)//中间点
                    {
                        var sum = matrix[i, j - 1] + matrix[i, j + 1] + matrix[i + 1, j] + matrix[i - 1, j] + matrix[i - 1, j - 1] + matrix[i - 1, j + 1] + matrix[i + 1, j - 1] + matrix[i + 1, j + 1];
                        if (sum >= 2)
                        {
                            if (matrix[i, j - 1] == 0 || matrix[i, j + 1] == 0 || matrix[i + 1, j] == 0 || matrix[i - 1, j] == 0)
                            {
                                int[,] matrixSix = new int[,] { { matrix[i - 1, j - 1], matrix[i - 1, j], matrix[i - 1, j + 1] }, { matrix[i, j - 1], matrix[i, j], matrix[i, j + 1] }, { matrix[i + 1, j - 1], matrix[i + 1, j], matrix[i + 1, j + 1] } };

                                if (!IsImpactConnect(matrixSix, 5))
                                {
                                    matrix[i, j] = 0;
                                }
                            }
                        }
                    }
                }

            }
            return matrix;
        }

        private static bool IsImpactConnect(int[,] matrix, int type)
        {
            ImageConnectCut ic = new ImageConnectCut(matrix);
            var conNum = ic.ConnectedLayerAreaNumList.Count;
            int[,] matrixChange = null;
            if (type == 1)
            {
                matrixChange = new int[,] { { matrix[0, 0], 0, matrix[0, 2] }, { matrix[1, 0], matrix[1, 1], matrix[1, 2] } };
            }
            if (type == 2)
            {
                matrixChange = new int[,] { { matrix[0, 0], matrix[0, 1] }, { 0, matrix[1, 1] }, { matrix[2, 0], matrix[2, 1] } };
            }
            if (type == 3)
            {
                matrixChange = new int[,] { { matrix[0, 0], matrix[0, 1] }, { matrix[1, 0], 0 }, { matrix[2, 0], matrix[2, 1] } };
            }
            if (type == 4)
            {
                matrixChange = new int[,] { { matrix[0, 0], matrix[0, 1], matrix[0, 2] }, { matrix[1, 0], 0, matrix[1, 2] } };
            }
            if (type == 5)
            {
                matrixChange = new int[,] { { matrix[0, 0], matrix[0, 1], matrix[0, 2] }, { matrix[1, 0], 0, matrix[1, 2] }, { matrix[2, 0], matrix[2, 1], matrix[2, 2] } };
            }
            ImageConnectCut icc = new ImageConnectCut(matrixChange);
            var conNumC = icc.ConnectedLayerAreaNumList.Count;
            if (conNum == conNumC)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public static Bitmap SobelEdgesFilters(Bitmap bmp)
        {
            bmp = Grayscale.CommonAlgorithms.RMY.Apply(bmp);
            return new SobelEdgeDetector().Apply(bmp);
        }


        /// <summary>
        /// 将连通域图片重新调整为二值化图片
        /// </summary>
        /// <param name="binaryImage"></param>
        public static int[,] GetPictureSquence(int[,] connectedLayerAreaMatrix)
        {
            var width = connectedLayerAreaMatrix.GetLength(1);
            var height = connectedLayerAreaMatrix.GetLength(0);
            int[,] singleChar = new int[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (connectedLayerAreaMatrix[y, x] == 0)
                    {
                        singleChar[y, x] = 0;
                    }
                    else
                    {
                        singleChar[y, x] = 1;
                    }

                }
            }
            return singleChar;
        }

        /// <summary>
        /// 重置连通域矩阵为二值化矩阵
        /// </summary>
        /// <param name="binaryImage"></param>
        public static int[,] ResetMatrix(int[,] matrix)
        {

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    if (matrix[y, x] == 1)
                    {
                        matrix[y, x] = 0;
                    }
                    else
                    {
                        if (matrix[y, x] != 0)
                        {
                            matrix[y, x] = 1;
                        }
                    }
                }
            }
            return matrix;
        }
    }
}
