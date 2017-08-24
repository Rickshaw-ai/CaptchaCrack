

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageCut
{
   public class HoriAndVertiCut : IImageCut
    {
        private int[,] imageMatrix;

        private List<int> xValueListOrder;//各字符x轴投影切割长度段
        private List<int> xAxisCutLength = new List<int>();//X轴各字符切割长度
        //private List<int[,]> _cutCharImageList = new List<int[,]>();
        //public List<int[,]> CutCharImageList
        //{
        //    get { return this._cutCharImageList; }
        //}       

        public HoriAndVertiCut(int[,] _imageMatrix)
        {
            imageMatrix = _imageMatrix;
            Parser();
        }
        private void Parser()
        {
            GetXvalueList();
            GetXaxisCutStartEnd();
            //GetCutCharImage();
        }

        /// <summary>
        /// 得到X轴投影的切割段
        /// </summary>
        private void GetXvalueList()
        {
            var height = imageMatrix.GetLength(0);
            var width = imageMatrix.GetLength(1);
            List<int> xValueList = new List<int>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (imageMatrix[y, x] == 1)
                    {
                        if (!xValueList.Contains(x))
                        {
                            xValueList.Add(x);
                        }
                    }
                }
            }
            xValueListOrder = xValueList.OrderBy(u => u).ToList();
        }
        /// <summary>
        /// 得到每个字符的X轴切割长度
        /// </summary>
        /// <param name="xValueListOrder"></param>
        /// <returns></returns>
        private void GetXaxisCutStartEnd()
        {
            var startcursor = 0;
            var endcursor = 0;
            for (int index = 0; index < xValueListOrder.Count - 1; index++)
            {
                if (xValueListOrder[index] + 1 == xValueListOrder[index + 1])
                {
                    if (index == xValueListOrder.Count - 2)//统计最后一组连续数的个数
                    {
                        xAxisCutLength.Add(xValueListOrder.Count - 1 - startcursor + 1);
                        break;
                    }
                    continue;
                }
                else
                {
                    endcursor = index;
                    xAxisCutLength.Add(endcursor - startcursor + 1);
                    startcursor = index + 1;
                    if (index == xValueListOrder.Count - 2)//最后是单个元素d的情况如{1，2，3，7}
                    {
                        xAxisCutLength.Add(1);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 切割单个字符
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageMatrix"></param>
        /// <param name="xValueListOrder"></param>
        /// <param name="xaxisCutLength"></param>
        /// <returns></returns>
        public List<int[,]> GetCodeList()
        {
            var pointerstart = 0;
            var pointerend = 0;
            var xstart = 0;
            var xend = 0;
            int[] YaxisCutStartEnd;
            int[,] cutcharMatrix;
            List<int[,]> _cutCharImageList = new List<int[,]>();
            for (int index = 0; index < xAxisCutLength.Count; index++)
            {
                xstart = xValueListOrder[pointerstart];
                pointerend = pointerstart + xAxisCutLength[index] - 1;
                pointerstart += xAxisCutLength[index];
                xend = xValueListOrder[pointerend];
                YaxisCutStartEnd = GetYaxisCutStartEnd( xstart, xend);
                cutcharMatrix = GetCharCutMatrix( xstart, xend, YaxisCutStartEnd[0], YaxisCutStartEnd[1]);            
                _cutCharImageList.Add(cutcharMatrix);
            }
            return _cutCharImageList;
        }

        /// <summary>
        /// 得到每个字符Y轴切割的起始值
        /// </summary>
        /// <param name="imageMatrix"></param>
        /// <param name="xstart"></param>
        /// <param name="xend"></param>
        private int[] GetYaxisCutStartEnd(int xstart, int xend)
        {
            int[] yaxiscutstartend = new int[2];
            var height = imageMatrix.GetLength(0);
            List<int> yValueList = new List<int>();
            for (int x = xstart; x < xend + 1; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (imageMatrix[y, x] == 1)
                    {
                        if (!yValueList.Contains(y))
                        {
                            yValueList.Add(y);
                        }
                    }
                }
            }
            List<int> yaxisListOrder = yValueList.OrderBy(u => u).ToList();
            yaxiscutstartend[0] = yaxisListOrder.Min();
            yaxiscutstartend[1] = yaxisListOrder.Max();
            //var heightcut = max - min;
            return yaxiscutstartend;
        }

        /// <summary>
        /// 从原有矩阵中获取单个切割字符的特征（其实也是矩阵）
        /// </summary>
        /// <param name="imageMatrix"></param>
        /// <param name="xstart"></param>
        /// <param name="xend"></param>
        /// <param name="ystart"></param>
        /// <param name="yend"></param>
        private int[,] GetCharCutMatrix(int xstart, int xend, int ystart, int yend)
        {
            var width = xend - xstart + 1;
            var height = yend - ystart + 1;
            int[,] cutchar = new int[height, width];
            var cutcharx = 0;
            var cutchary = 0;
            for (int y = ystart; y <= yend; y++)
            {
                for (int x = xstart; x <= xend; x++)
                {
                    cutchar[cutcharx, cutchary] = imageMatrix[y, x];                    
                    cutchary++;
                }
                cutchary = 0;
                cutcharx++;
            }
            return cutchar;
        }
    }
}
