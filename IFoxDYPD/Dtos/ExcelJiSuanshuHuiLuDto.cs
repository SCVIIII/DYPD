using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Dtos
{
    /// <summary>
    /// 存放计算书类(由变电所计算书导入)
    /// </summary>
    public class ExcelJiSuanshuHuiLuDto
    {
        public string IdHuilu { get; set; } //编号
        public string Purpose { get; set; }  //用电设备组
        public string Pe { get; set; }  //Pe
        public string Kx { get; set; }  //Kx
        public string Cos { get; set; }  //cos
        public string Pjs { get; set; }  //Pjs
        public string Ijs { get; set; }  //Ijs
        public string Ir1 { get; set; }  //Izd
        public string CableType { get; set; }  //电缆类型
        public string CableCSA { get; set; }  //电缆截面
        public string Name { get; set; }  //备注(箱名)
        public string NorS { get; set; }  //常备用选择
        public string Height { get; set; }  //柜高
        public string Info1 { get; set; }  //消防/切非/电气火灾监控信息
        public string Info_GuiHao { get; set; }  //柜号
        public string Info_SheetName { get; set; }  //所在表名

        // end of class JiSuanshuExcel
    }


    public class ExcelDto
    {
        //EXCEL的Page名
        public List<string> SheetNames { get; set; } = new List<string>();
        //反推得出的变压器数量
        public int Num_transformer { get; set; } = 0; 
        public List<string> GuiHao_TA { get; set; } = new List<string>(); //TA变压器的馈电柜柜号
        public List<string> GuiHao_TB { get; set; } = new List<string>(); //TB变压器的馈电柜柜号
        public string First_GuiHao_TA { get; set; } //TA变压器的第一个馈电柜编号
        public string First_GuiHao_TB { get; set; } //TB变压器的第一个馈电柜编号
        public int Nth_FirstKuiChuGui_TA { get; set; } //TA变压器的第一个馈电柜所在序号号
        public int Nth_FirstKuiChuGui_TB { get; set; } //TB变压器的第一个馈电柜所在序号号
        public int Num_KuiDianGui_TA { get; set; } = 0; //TA变压器的馈电柜数量
        public int Num_KuiDianGui_TB { get; set; } = 0; //TB变压器的馈电柜数量
        public string Pre_TA { get; set; } //TA变压器编号的前缀
        public string Pre_TB { get; set; } //TB变压器编号的前缀
        public bool Fanzhuan_TA { get; set; } = false;//TA变压器是否翻转
        public bool Fanzhuan_TB { get; set; } = true;  //TB变压器是否翻转

    }
}
