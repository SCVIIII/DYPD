using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Dtos
{
    public class MTVSDto
    {

        //电容柜容量
        public string EB31_CAPACITY { get; set; }
        public string EB11_GUIHAO { get; set; }
        public string EB12_PURPOSE { get; set; }
        //进线柜尺寸,默认为800(W)X1000(D)X2200(H)
        public string EB13_SIZE { get; set; }
        //进线柜类型,默认为MNS柜
        public string EB14_TYPE { get; set; }
        //铜排规格
        public string EB21_BUSBAR { get; set; }
        //三相共补
        public string EB32_CAPACITY_3P { get; set; }
        //分补
        public string EB33_CAPACITY_1P { get; set; }
        //熔断器规格
        public string EB34_FUSE { get; set; }
        //互感器
        public string EB35_CT { get; set; }
        //功率因数控制器
        public string EB36_INFO1 { get; set; }
        //电抗率
        public string EB37_INFO2 { get; set; }
        //避雷器
        public string EB38_INFO3 { get; set; }
        //功能要求A
        public string EB41_INFOA { get; set; }
        //功能要求B
        public string EB42_INFOB { get; set; }
        //功能要求C
        public string EB43_INFOC { get; set; }
        //功能要求D
        public string EB44_INFOD { get; set; }

    }
}
