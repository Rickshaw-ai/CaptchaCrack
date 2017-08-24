using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using CaptchaCrackFrame;
using System.Diagnostics;
namespace TestCaptchaCrack
{
    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            //实例化一个打开文件对话框
            OpenFileDialog op = new OpenFileDialog();
            //设置文件的类型
            // op.Filter = "Image files (*.jpg;*.bmp;*.gif;*.png)|*.jpg*.bmp;*.gif;*.png|AllFiles (*.*)|*.*"; 
            op.Filter = "所有文件(*.*)|*.*";
            //如果用户点击了打开按钮、选择了正确的图片路径则进行如下操作：
            if (op.ShowDialog() == DialogResult.OK)
            {
                //实例化一个文件流
                FileStream fs = new FileStream(op.FileName, FileMode.Open);
                //把文件读取到字节数组
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                //实例化一个内存流--->把从文件流中读取的内容[字节数组]放到内存流中去
                MemoryStream ms = new MemoryStream(data);
                Bitmap bmp = new Bitmap(ms);
                Stopwatch sw = new Stopwatch();//http://www.cnblogs.com/gpcuster/archive/2008/06/24/1229140.html   http://jingyan.baidu.com/season/38030
                sw.Start();
                ImageCrackBase icb = new BeiJingLocalImageCrack(bmp);
                icb.ImageProcess();
                var result = icb.ImageRecognize();
                sw.Stop();
                Console.WriteLine(result);
                Console.WriteLine(sw.ElapsedMilliseconds.ToString() + "ms");
                Console.ReadLine();
            }
        }
    }
}
