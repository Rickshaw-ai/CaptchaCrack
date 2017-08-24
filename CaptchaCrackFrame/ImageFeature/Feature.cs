

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSource;

namespace ImageFeature
{
   public class Feature
    {
        private int _featureNumber;
        /// <summary>
        /// 特征个数
        /// </summary>
        public int FeatureNumber
        {
            get { return this._featureNumber; }
        }

        private int _inputDimension;
        /// <summary>
        /// 输入维度
        /// </summary>
        public int InputDimension
        {
            get { return this._inputDimension; }
        }

        private List<List<double>> _featureSet;
        /// <summary>
        /// 训练特征集
        /// </summary>
        public List<List<double>> FeatureSet
        {
            get { return this._featureSet; }
        }

        private List<int> _labelList;
        /// <summary>
        /// 标签集
        /// </summary>
        public List<int> LableList
        {
            get { return this._labelList; }
        }
        public Feature()
        {
            Features();
        }

        /// <summary>
        /// 该方法适用于余弦定理
        /// </summary>
        /// <param name="_featureSet"></param>
        /// <param name="_labelList"></param>
        private void Features()
        {
            IEnumerable<string> lis = SingletonReadData.GetInstance.GetOriginalData;
            List<List<double>> featureSet = new List<List<double>>();
            List<int> labelList = new List<int>();
            foreach (string str in lis)
            {
                List<double> featureList = new List<double>();
                var featureLine = str.Split(':')[1];
                for (int i = 0; i < featureLine.Length; i++)
                {
                    featureList.Add(Convert.ToDouble(featureLine[i].ToString()));
                }
                featureSet.Add(featureList);
                labelList.Add(Convert.ToInt32(str.Split(':')[0]));

            }
            this._featureSet = featureSet;
            this._labelList = labelList;
            this._featureNumber = labelList.Distinct().ToList().Count;
            this._inputDimension = featureSet[0].Count;

        }
    }
}
