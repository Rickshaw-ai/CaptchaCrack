

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageCut
{
   public interface IImageCut
    {
        List<int[,]> GetCodeList();
    }
}
