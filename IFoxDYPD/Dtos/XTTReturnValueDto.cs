using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Dtos
{
    public class XTTReturnValueDto
    {
        public class IsValidFormatDto()
        {
            //当前Dto的格式是否正确
            public bool IsValid { get; set; }=false;
            //所在配电箱的名称
            public string PDXName { get; set; }
            //编号的形式
            //形式0为无效
            //形式1为 箱名+回路+功率
            //形式2为 箱名+回路
            //形式3为 箱名+多回路+功率
            //形式4为 箱名+回路+子配电箱+功率
            public int Mode { get; set; } = 0;
            //回路编号的前缀c,m,mk,N,ck
            public string Prefix { get; set; }
            //回路功率
            public double Pe { get; set; }
        }

    }
}
