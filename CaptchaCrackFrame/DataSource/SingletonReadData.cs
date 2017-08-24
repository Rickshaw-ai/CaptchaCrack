

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace DataSource
{
    public sealed class SingletonReadData
    {
        private static IEnumerable<string> data;
        SingletonReadData()
        {
            LoadData();
        }
        private void LoadData()
        {
            //var path = System.AppDomain.CurrentDomain.BaseDirectory + "/FeatureData/BeiJingFeatureData.txt";
            data = File.ReadLines(System.AppDomain.CurrentDomain.BaseDirectory + "/FeatureData/BeiJingFeatureData.txt");
        }
        private static SingletonReadData srd = new SingletonReadData();

        public static SingletonReadData GetInstance
        {
            get { return srd; }
        }
        public IEnumerable<string> GetOriginalData
        {
            get { return data; }
        }
    }
}
