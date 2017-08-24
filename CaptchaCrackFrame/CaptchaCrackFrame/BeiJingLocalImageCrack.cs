

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using ImageFeature;

namespace CaptchaCrackFrame
{
   public class BeiJingLocalImageCrack: ImageCrackBase
    {

        public BeiJingLocalImageCrack(Bitmap bmp)
            :base(bmp)
        {

        }
        /// <summary>
        /// 获取二值化图像
        /// </summary>
        /// <param name="grayImg"></param>
        /// <returns></returns>
        public override void GetBinaryImage()
        {          
            Bitmap bmp = base.grayImage.Clone(new Rectangle(0, 0, base.grayImage.Width, base.grayImage.Height), PixelFormat.Format24bppRgb);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    if (color.R > 4 && color.R < 100)
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

        //public override void ImageCutToList()
        //{
            
        //}


        public override string ImageRecognize()
        {
            Feature feature = new Feature();
            List<List<double>> trainFeatureSet = feature.FeatureSet;
            List<int> trainLabelList = feature.LableList;
            List<List<double>> testFeatureSet = new List<List<double>>();
            List<int> blackcountList = new List<int>();
            int blackcounter;
            base._cutCharImageList.ForEach(t =>
            {
                testFeatureSet.Add(MatrixToLine(t, out blackcounter));
                blackcountList.Add(blackcounter);
            });
            List<int> recognizeList = new List<int>();//识别结果集
            testFeatureSet.ForEach(t =>
            {
                recognizeList.Add(trainLabelList[GetClassifyResult(trainFeatureSet, t)]);
            });

            return Recognize(recognizeList, blackcountList);
        }

        /// <summary>
        /// 映射关系：加：10; 减：11; 乘：12; 去：13; 以：14; 上：15;(未考虑“加”被分开的情况，以及“减”切割的时候被切割成零星点的情况)
        /// </summary>
        /// <param name="recognizeList"></param>
        /// <returns></returns>
        private string Recognize(List<int> recognizeList)
        {
            var result = 0;
            try
            {
                if (recognizeList[1] == 10)
                {
                    if (recognizeList[2] >= 10)//第一个四则运算符识别对了，第二个有可能错了，但是不影响结果，不要写死为recognizeList[2]等于15
                    {
                        result = recognizeList[0] + recognizeList[3];
                    }
                    else
                    {
                        result = recognizeList[0] + recognizeList[2];
                    }
                    return result.ToString();
                }
                if (recognizeList[1] == 11)
                {
                    if (recognizeList[2] >= 10)
                    {
                        result = recognizeList[0] - recognizeList[3];
                    }
                    else
                    {
                        result = recognizeList[0] - recognizeList[2];
                    }
                    result.ToString();
                }
                if (recognizeList[1] == 12)
                {
                    if (recognizeList[2] >= 10)
                    {
                        result = recognizeList[0] * recognizeList[3];
                    }
                    else
                    {
                        result = recognizeList[0] * recognizeList[2];
                    }
                    result.ToString();
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                return "fail";
            }

        }

        /// <summary>
        /// 映射关系：加：10; 减：11; 乘：12; 去：13; 以：14; 上：15;
        /// </summary>
        /// <param name="recognizeList"></param>
        /// <param name="blackcountList">单个字符中1的个数</param>
        /// <returns></returns>
        private string Recognize(List<int> recognizeList, List<int> blackcountList)
        {
            var result = 0;
            try
            {
                #region 加未弯曲的情况
                //先判断第二个字符1的个数，判断是否是未变形“加”被切割
                if (blackcountList[1] >= 39 && blackcountList[1] <= 83)
                {
                    if (recognizeList[3] >= 10)//第一个四则运算符识别对了，第二个有可能错了，但是不影响结果，不要写死为recognizeList[3]等于15
                    {
                        result = recognizeList[0] + recognizeList[4];//5 力 口 上 5 

                    }
                    else
                    {
                        result = recognizeList[0] + recognizeList[3];// 5 力 口 5 
                    }
                    return result.ToString();
                }
                #endregion

                #region 减弯曲后，左边被切割一丁点的情况
                if (blackcountList[1] >= 6 && blackcountList[1] <= 8)
                {
                    if (recognizeList[3] >= 10)//第一个四则运算符识别对了，第二个有可能错了，但是不影响结果，不要写死为recognizeList[3]等于15
                    {
                        result = recognizeList[0] - recognizeList[4];//5 ' 减 去 5 

                    }
                    else
                    {
                        result = recognizeList[0] - recognizeList[3];// 5 ' 减 5 
                    }
                    return result.ToString();
                }
                #endregion

                return Recognize(recognizeList);
            }
            catch (Exception ex)
            {
                return "0";
            }

        }

        /// <summary>
        /// 可以写成公共方法(补0，并且统计1的个数)，目前是针对于预先定理，如果是机器学习方法，需要修改下
        /// </summary>
        /// <param name="imageSquence"></param>
        /// <param name="normalizationDim"></param>
        private List<double> MatrixToLine(int[,] imageSquence, out int blackCount, int normalizationDim = 625)
        {
            blackCount = 0;
            List<double> lineList = new List<double>();
            var height = imageSquence.GetLength(0);
            var width = imageSquence.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    lineList.Add(imageSquence[y, x]);
                    if (imageSquence[y, x] == 1)
                    {
                        blackCount++;
                    }
                }
            }
            var zerocount = normalizationDim - height * width;
            for (int i = 0; i < zerocount; i++)
            {
                //补0
                lineList.Add(0);
            }
            return lineList;
        }

        /// <summary>
        /// 获取相似度最高的向量索引
        /// </summary>
        /// <param name="_trainFeatureSet"></param>
        /// <param name="_testFeature"></param>
        /// <returns></returns>
        private static int GetClassifyResult(List<List<double>> _trainFeatureSet, List<double> _testFeature)
        {
            double[] cosArray = new double[_trainFeatureSet.Count];
            Dictionary<double, int> cosdic = new Dictionary<double, int>();
            for (int i = 0; i < _trainFeatureSet.Count; i++)
            {
                cosArray[i] = CalculateVectorCos(_trainFeatureSet[i], _testFeature);
                if (!cosdic.Keys.Contains(cosArray[i]))
                    cosdic.Add(cosArray[i], i);
            }
            double max = cosArray.Max();
            int maxIndex;
            cosdic.TryGetValue(max, out maxIndex);
            return maxIndex;
        }

        /// <summary>
        /// 计算向量间夹角
        /// </summary>
        /// <param name="_trainFeature"></param>
        /// <param name="_testFeature"></param>
        /// <returns></returns>
        private static double CalculateVectorCos(List<double> _trainFeature, List<double> _testFeature)
        {
            double cosab = 0;
            double vectora = 0;
            double vectorb = 0;
            double aInnerProductb = 0;
            vectora = GetVertorLength(_trainFeature);
            vectorb = GetVertorLength(_testFeature);
            for (int i = 0; i < _trainFeature.Count; i++)
            {
                aInnerProductb += _trainFeature[i] * _testFeature[i];
            }
            cosab = aInnerProductb / (vectora * vectorb);
            return cosab;
        }

        /// <summary>
        /// 计算向量的长度
        /// </summary>
        /// <param name="_trainFeature"></param>
        /// <returns></returns>
        private static double GetVertorLength(List<double> _trainFeature)
        {
            double vector = 0;
            for (int i = 0; i < _trainFeature.Count; i++)
            {
                vector += _trainFeature[i] * _trainFeature[i];
            }
            vector = Math.Sqrt(vector);
            return vector;
        }
    }
}
