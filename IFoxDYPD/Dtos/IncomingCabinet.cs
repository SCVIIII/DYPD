using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Dtos
{
    internal class IncomingCabinet
    {
    }
    /// <summary>
    /// 定义类:进线柜
    /// </summary>
    public class DYPD_INCOMING_CABINET
    {
        public string EB31_TRANSFORMER { get; set; }
        //变压器电压等级
        public string EB11_GUIHAO { get; set; }
        public string EB12_PURPOSE { get; set; }
        //进线柜尺寸,默认为800(W)X1000(D)X2200(H)
        public string EB13_SIZE { get; set; }
        //进线柜类型,默认为MNS柜
        public string EB14_TYPE { get; set; }
        //变压器ACB的编号
        public string EB16_NUM_SWTICH { get; set; }
        //短延时整定电流
        public string EB17_IR2 { get; set; }
        //断路器规格
        public string EB18_BREAKER { get; set; }
        //断路器额定电流
        public string EB19_In { get; set; }
        //断路器长延时整定电流
        public string EB20_Ir1 { get; set; }
        //断路器短路瞬动整定电流
        public string EB21_Ir3 { get; set; }
        //断路器CT变比
        public string EB22_CT { get; set; }
        //断路器备注
        public string EB23_INFO1 { get; set; }
        //变压器规格

        public string EB32_VOLTAGE { get; set; }
        //变压器阻抗电压
        public string EB33_Ud { get; set; }
        //变压器IP防护等级
        public string EB34_IP { get; set; }
        //变压器尺寸
        public string EB35_SIZE { get; set; }
        //变压器备注
        public string EB36_INFO2 { get; set; }
        //进线电缆
        public string EB41_IMCOMING_CABLE { get; set; }
        //母线槽规格
        public string EB42_BUSBAR { get; set; }
        //母线槽要求
        public string EB43_BUSBAR_INFO { get; set; }
        //接地铜排
        public string EB44_BUSBAR_PE { get; set; }

    }
}
