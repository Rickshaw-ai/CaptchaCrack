

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageCut
{
   public class ImageConnectCut:IImageCut
    {
        private int[,] matrix;
        private int _connectedLayerAreaNum;

        public int ConnectedLayerAreaNum
        {
            get { return _connectedLayerAreaNum; }
        }
        private List<int> _connectedLayerAreaNumList;
        public List<int> ConnectedLayerAreaNumList
        {
            get { return _connectedLayerAreaNumList; }
        }

        private int[,] _firstScanMatrix;
        public int[,] FirstScanMatrix
        {
            get { return _firstScanMatrix; }
        }

        private int[,] _connectedLayerAreaMatrix;
        public int[,] ConnectedLayerAreaMatrix
        {
            get { return _connectedLayerAreaMatrix; }
        }

        private Dictionary<int, int> parentDic;

        /// <summary>
        /// 一定要是二值化矩阵
        /// </summary>
        /// <param name="_matrix"></param>
        public ImageConnectCut(int[,] _matrix)
        {
            Init(_matrix);
        }

        private void Init(int[,] _matrix)
        {
            //matrix = _matrix;//初始化后，因为传递的是引用，所以_matrix的值会被改变，所以需要克隆
            matrix = (int[,])_matrix.Clone();
            _connectedLayerAreaNumList = new List<int>();
            parentDic = new Dictionary<int, int>();
            FirstScan();
            SecondScan();
            // StatisticConnectedNumber();
        }

        /// <summary>
        /// 八连通域,第一次扫描
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private void FirstScan()
        {
            var label = 0;
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 && j == 0)//第一个点
                    {
                        if (matrix[i, j] == 1)
                        {
                            label++;
                            parentDic.Add(label, 0);
                            matrix[i, j] = label;
                        }
                    }
                    else if (i == 0 && j > 0)//第一行
                    {
                        if (matrix[i, j] == 1)
                        {
                            if (matrix[i, j - 1] == 0)
                            {
                                label++;
                                parentDic.Add(label, 0);
                                matrix[i, j] = label;
                            }
                            else
                            {
                                matrix[i, j] = matrix[i, j - 1];
                            }

                        }
                    }
                    else if (i > 0 && j == 0)//第一列
                    {
                        if (matrix[i, j] == 1)
                        {
                            List<int> labelList = new List<int>();
                            labelList.Add(matrix[i - 1, j]);
                            labelList.Add(matrix[i - 1, j + 1]);
                            var pixValue = UnionLabel(labelList);
                            if (pixValue == 0)
                            {
                                label++;
                                parentDic.Add(label, 0);
                                matrix[i, j] = label;
                            }
                            else
                            {
                                matrix[i, j] = pixValue;
                            }

                        }

                    }
                    else if (i > 0 && j == width - 1)//最后一列
                    {
                        if (matrix[i, j] == 1)
                        {
                            List<int> labelList = new List<int>();
                            labelList.Add(matrix[i, j - 1]);
                            labelList.Add(matrix[i - 1, j - 1]);
                            labelList.Add(matrix[i - 1, j]);
                            var pixValue = UnionLabel(labelList);
                            if (pixValue == 0)
                            {
                                label++;
                                parentDic.Add(label, 0);
                                matrix[i, j] = label;
                            }
                            else
                            {
                                matrix[i, j] = pixValue;
                            }
                        }
                    }
                    else //中间部分
                    {
                        if (matrix[i, j] == 1)
                        {
                            List<int> labelList = new List<int>();
                            labelList.Add(matrix[i, j - 1]);
                            labelList.Add(matrix[i - 1, j - 1]);
                            labelList.Add(matrix[i - 1, j]);
                            labelList.Add(matrix[i - 1, j + 1]);
                            var pixValue = UnionLabel(labelList);
                            if (pixValue == 0)
                            {
                                label++;
                                parentDic.Add(label, 0);
                                matrix[i, j] = label;
                            }
                            else
                            {
                                matrix[i, j] = pixValue;
                            }
                        }
                    }

                }
            }
           // SetFirstScanMatrix(matrix);
        }

        /// <summary>
        /// 设置第一次扫描后二维矩阵(用于测试，真实环境可以注释掉)
        /// </summary>
        /// <param name="matrix"></param>
        private void SetFirstScanMatrix(int[,] matrix)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            _firstScanMatrix = new int[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        _firstScanMatrix[i, j] = matrix[i, j];
                    }
                }
            }
        }

        /// <summary>
        /// 第二次扫描
        /// </summary>
        private void SecondScan()
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        matrix[i, j] = FindRoot(matrix[i, j]);
                        if (!_connectedLayerAreaNumList.Contains(matrix[i, j]))
                        {
                            _connectedLayerAreaNumList.Add(matrix[i, j]);
                        }
                    }
                }
            }
            _connectedLayerAreaMatrix = matrix;
        }


        /// <summary>
        /// 找到根节点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private int FindRoot(int x)
        {
            var i = x;
            while (0 != parentDic[i])
                i = parentDic[i];
            return i;
        }


        /// <summary>
        /// 合并连通区域
        /// </summary>
        /// <param name="unZeroList"></param>
        /// <param name="parent"></param>
        private int UnionLabel(List<int> labelList)
        {
            var unZeroList = labelList.Where(a => a != 0).ToList();
            if (unZeroList.Count == 0)
            {
                return 0;
            }
            else
            {
                var min = unZeroList.Min();
                List<int> unZeroParentList = new List<int>();
                unZeroList.ForEach(a => {
                    if (parentDic[a] != 0)
                    {
                        unZeroParentList.Add(parentDic[a]);
                    }
                });
                var unZeroParentMin = 0;
                if (unZeroParentList.Count != 0)
                {
                    unZeroParentMin = unZeroParentList.Min();
                }
                int minlable = 0;
                if (unZeroParentMin != 0)
                {
                    if (min > unZeroParentMin)
                    {
                        minlable = unZeroParentMin;
                    }
                    else
                    {
                        minlable = min;
                    }
                }
                else
                {
                    minlable = min;
                }
                var parentZeroList = unZeroList.Where(a => parentDic[a] == 0 && a != minlable).ToList();
                parentZeroList.ForEach(a => parentDic[a] = minlable);
                return minlable;
            }


        }

        /// <summary>
        /// 统计连通区域的个数
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public int StatisticConnectedNumber()
        {
           return  parentDic.Values.ToList().Where(a => a == 0).ToList().Count;
            //_connectedLayerAreaNumList = parentDic.Keys.ToList();
            //parentDic.Keys.ToList().ForEach(a => { _connectedLayerAreaNumList.Add(FindRoot(a)); });
        }

        /// <summary>
        /// 统计单个字符的个数
        /// </summary>
        /// <param name="charNum"></param>
        /// <returns></returns>
        private int TheNumberOfSingleChar(int charNum)
        {
            var count = 0;
            for (int i = 0; i < _connectedLayerAreaMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _connectedLayerAreaMatrix.GetLength(1); j++)
                {
                    if (_connectedLayerAreaMatrix[i, j] == charNum)
                    {
                        count++;
                    }
                }
            }
            return count;
        }


        /// <summary>
        /// 通过连接字符1的个数清除干扰线
        /// </summary>
        /// <param name="countConnectedLow"></param>
        /// <param name="countConnectedHigh"></param>
        /// <returns></returns>
        public int[,] GetClearedImageMarix(int countConnectedLow, int countConnectedHigh)
        {
            List<int> setZeroList = new List<int>();
            _connectedLayerAreaNumList.ForEach(a => { if (TheNumberOfSingleChar(a) < countConnectedHigh && TheNumberOfSingleChar(a) > countConnectedLow) { setZeroList.Add(a); } });
            for (int i = 0; i < _connectedLayerAreaMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _connectedLayerAreaMatrix.GetLength(1); j++)
                {
                    if (setZeroList.Contains(_connectedLayerAreaMatrix[i, j]))
                    {
                        _connectedLayerAreaMatrix[i, j] = 0;
                    }
                }
            }
            setZeroList.ForEach(a => { _connectedLayerAreaNumList.Remove(a); });//和_connectedLayerAreaMatrix中的字符数保持一致
            return _connectedLayerAreaMatrix;
        }

        /// <summary>
        /// 通过连接字符的长度和宽度来清除干扰
        /// </summary>
        /// <param name="countConnectedLow"></param>
        /// <param name="countConnectedHigh"></param>
        /// <returns></returns>
        public int[,] ClearMarixByWidthAndHeight(int heightMin, int heightMax, int widthMin, int widthMax)
        {
            List<int> saveList = new List<int>();
            _connectedLayerAreaNumList.ForEach(a => { int[] heightAndWidth = GetWidthAndHeightFromMatrix(a); if (heightAndWidth[0] >= heightMin && heightAndWidth[0] <= heightMax && heightAndWidth[1] >= widthMin && heightAndWidth[1] <= widthMax) { saveList.Add(a); } });
            // _connectedLayerAreaNumList.ForEach(a => { var testNum = TheNumberOfSingleChar(a); });
            for (int i = 0; i < _connectedLayerAreaMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _connectedLayerAreaMatrix.GetLength(1); j++)
                {
                    if (!saveList.Contains(_connectedLayerAreaMatrix[i, j]))
                    {
                        if (_connectedLayerAreaNumList.Contains(_connectedLayerAreaMatrix[i, j]))
                        {
                            _connectedLayerAreaNumList.Remove(_connectedLayerAreaMatrix[i, j]);//和_connectedLayerAreaMatrix中的字符数保持一致
                        }
                        _connectedLayerAreaMatrix[i, j] = 0;
                    }
                }
            }
            return _connectedLayerAreaMatrix;
        }

        private int[] GetWidthAndHeightFromMatrix(int singleCode)
        {
            List<int> heightList = new List<int>();
            List<int> widthList = new List<int>();
            for (int i = 0; i < _connectedLayerAreaMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _connectedLayerAreaMatrix.GetLength(1); j++)
                {
                    if (_connectedLayerAreaMatrix[i, j] == singleCode)
                    {
                        heightList.Add(i);
                        widthList.Add(j);
                    }
                }
            }
            var heightMax = heightList.Max();
            var heightMin = heightList.Min();
            var widthMax = widthList.Max();
            var widthMin = widthList.Min();
            int[] heightAndWidth = new int[] { heightMax - heightMin + 1, widthMax - widthMin + 1, heightMin, widthMin, singleCode };//数组既包括长度和宽度，也包括截取数据的起始点,以及字符本身
            return heightAndWidth;
        }

        public List<int[,]> GetCodeList()
        {
            List<int[,]> codeList = new List<int[,]>();
            List<int[]> widthAndHeightList = new List<int[]>();
            _connectedLayerAreaNumList.ForEach(a => { widthAndHeightList.Add(GetWidthAndHeightFromMatrix(a)); });
            widthAndHeightList = widthAndHeightList.OrderBy(a => a[3]).ToList();
            widthAndHeightList.ForEach(a => { codeList.Add(GetSingleCodeMatrix(a)); });
            return codeList;
        }
        private int[,] GetSingleCodeMatrix(int[] heightAndWidth)
        {
            //var codeNum= TheNumberOfSingleChar(singleCode);
            // int[] heightAndWidth = GetWidthAndHeightFromMatrix(singleCode);
            int[,] singleCodeMatrix = new int[heightAndWidth[0], heightAndWidth[1]];

            for (int i = 0; i < heightAndWidth[0]; i++)
            {
                for (int j = 0; j < heightAndWidth[1]; j++)
                {
                    if (_connectedLayerAreaMatrix[heightAndWidth[2] + i, heightAndWidth[3] + j] == heightAndWidth[4])
                    {
                        singleCodeMatrix[i, j] = 1;
                    }
                    else
                    {
                        singleCodeMatrix[i, j] = 0;
                    }
                }
            }
            return singleCodeMatrix;
        }
    }
}
