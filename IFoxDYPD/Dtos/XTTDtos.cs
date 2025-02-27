using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Dtos
{
    public class XTTDtos
    {

    }


    /// <summary>
    /// 存放XTT配电箱回路的功能
    /// </summary>
    public enum HuiluOptions
    {
        Error = 0,
        子配电箱N = 1,
        风机水泵控制箱d = 2,
        普通照明m = 3,
        普通插座c = 4,
        空调插座ck = 5,
        智能照明mk = 6,
        风机盘管d = 7,
    }
    public class XTTHuiluDto : ObservableObject
    {
        private string _name;
        private string _circuitNumber;
        private double _pe = 0;
        private string _cableType;
        private string _cable;
        private double _cableCSA = 2.5;
        private double _kx = 1.0;
        private double _cos = 0.9;
        private int    _in;
        private string _purpose;
        private string _blkjName;
        private string _circuitBreaker_In;
        private string _circuitBreaker_Shell;
        private int    _circuitBreaker_Mode = 0;
        private string _l123;
        private bool   _isInsert = false;
        private string _VE;
        private string _phase;
        private string _info5;
        private string _info6;
        private HuiluOptions _huiluStatus;

        /// <summary>
        /// 回路功能
        /// </summary>
        public HuiluOptions HuiluStatus
        {
            get => _huiluStatus;
            set => SetProperty(ref _huiluStatus, value);
        }

        /// <summary>
        /// 所属配电箱的名称
        /// </summary>
        public string Name 
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        /// <summary>
        /// 回路编号
        /// </summary>
        public string CircuitNumber
        {
            get => _circuitNumber;
            set => SetProperty(ref _circuitNumber, value);
        }
        /// <summary>
        /// 功率
        /// </summary>
        public double Pe
        {
            get => _pe;
            set => SetProperty(ref _pe, Math.Round(value, 1)); // 限定为一位小数
        }
        /// <summary>
        /// 电缆类型
        /// </summary>
        public string CableType
        {
            get => _cableType;
            set => SetProperty(ref _cableType, value);
        }
        /// <summary>
        /// 电缆截面
        /// </summary>
        public double CableCSA
        {
            get => _cableCSA;
            set => SetProperty(ref _cableCSA, Math.Round(value, 1));
        }
        /// <summary>
        /// 电缆（类型+界面+穿管+敷设）
        /// </summary>
        public string Cable
        {
            get => _cable;
            set => SetProperty(ref _cable, value);
        }
        /// <summary>
        /// 需要系数，默认为1
        /// </summary>
        public double Kx
        {
            get => _kx;
            set => SetProperty(ref _kx, Math.Round(value, 1)); // 限定为一位小数
        }
        /// <summary>
        /// 功率因数，默认0.9
        /// </summary>
        public double Cos
        {
            get => _cos;
            set => SetProperty(ref _cos, Math.Round(value, 1)); // 限定为一位小数
        }
        /// <summary>
        /// 计算电流（只读属性）
        /// </summary>
        public double Ijs => Math.Round(Pe * Kx / Cos / 0.38 / 1.732, 1); // Ijs 计算为一位小数
        /// <summary>
        /// 整定电流
        /// </summary>
        public int In
        {
            get => _in;
            set => SetProperty(ref _in, value);
        }

        /// <summary>
        /// 回路用途 照明、插座等
        /// </summary>
        public string Purpose
        {
            get => _purpose;
            set => SetProperty(ref _purpose, value);
        }
        /// <summary>
        /// 插入的图块名称
        /// </summary>
        public string BlkName
        {
            get => _blkjName;
            set => SetProperty(ref _blkjName, value);
        }
        /// <summary>
        /// 断路器额定电流/微断编号
        /// </summary>
        public string CircuitBreaker_In
        {
            get => _circuitBreaker_In;
            set => SetProperty(ref _circuitBreaker_In, value);
        }
        /// <summary>
        /// 断路器的壳架电流（微端无此项）
        /// </summary>
        public string CircuitBreaker_Shell
        {
            get => _circuitBreaker_Shell;
            set => SetProperty(ref _circuitBreaker_Shell, value);
        }
        /// <summary>
        /// 断路器形式 0-微断 1-塑壳 
        /// </summary>
        public int CircuitBreaker_Mode
        {
            get => _circuitBreaker_Mode;
            set => SetProperty(ref _circuitBreaker_Mode, value);
        }
        public string L123
        {
            get => _l123;
            set => SetProperty(ref _l123, value);
        }
        /// <summary>
        /// 标志位:当前回路是否插入
        /// </summary>
        public bool IsInsert
        {
            get => _isInsert;
            set => SetProperty(ref _isInsert, value);
        }
        public string VE
        {
            get => _VE;
            set => SetProperty(ref _VE, value);
        }
        /// <summary>
        ///  保护开关级数
        /// </summary>
        public string Phase
        {
            get => _phase;
            set => SetProperty(ref _phase, value);
        }
        public string Info5
        {
            get => _info5;
            set => SetProperty(ref _info5, value);
        }
        public string Info6
        {
            get => _info6;
            set => SetProperty(ref _info6, value);
        }


    }


    /// <summary>
    /// 定义类:存放筛选,排序,且设置好信息后的配电箱回路信息
    /// 供后续插入函数使用
    /// </summary>
    public class XTTHuiluList_Dto : ObservableObject
    {
        private string _name;
        private List<XTTHuiluDto> _listHuilus;

        /// <summary>
        /// 所属配电箱的名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public List<XTTHuiluDto> ListHuilus
        {
            get => _listHuilus;
            set => SetProperty(ref _listHuilus, value);
        }
    }

    /// <summary>
    /// 判断照明平面的是否正确编号程序的返回类，若正确返回一个class
    /// </summary>
    public class XTTHuiLuIsValidDto : ObservableObject
    {
        //string name, out string huilu, out double pe)

        private string _name;
        /// <summary>
        /// 所属配电箱的名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _pre;
        /// <summary>
        /// 所属配电箱的名称
        /// </summary>
        public string Pre
        {
            get => _pre;
            set => SetProperty(ref _pre, value);
        }
        private string _circuitNumber;
        /// <summary>
        /// 回路编号
        /// </summary>
        public string CircuitNumber
        {
            get => _circuitNumber;
            set => SetProperty(ref _circuitNumber, value);
        }

        private double _pe = 0;
        /// <summary>
        /// 回路功率
        /// </summary>
        public double Pe
        {
            get => _pe;
            set => SetProperty(ref _pe, value);
        }

        private string _PDXname;
        /// <summary>
        /// 子配电箱名称
        /// </summary>
        public string PDXName
        {
            get => _PDXname;
            set => SetProperty(ref _PDXname, value);
        }
        private bool _isValid = false;
        /// <summary>
        /// 此回路格式是否正确
        /// </summary>
        public bool IsValid
        {
            get => _isValid;
            set => SetProperty(ref _isValid, value);
        }

        //编号的形式
        //形式0为无效
        //形式1为 箱名+回路+功率
        //形式2为 箱名+回路
        //形式3为 箱名+多回路+功率
        //形式4为 箱名+回路+子配电箱+功率
        private int _mode = 0;
        /// <summary>
        /// 此回路格式是否正确
        /// </summary>
        public int Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }
    }

    /// <summary>
    /// 由风机水泵属性块=>提取的class
    /// </summary>
    public class XTTACDto
    {
        private double _pe;
        /// <summary>
        /// 功率
        /// </summary>
        public double Pe
        {
            get { return _pe; }
            set { _pe = value; }
        }
        private string _purpose;
        /// <summary>
        /// 风机功能及名称
        /// </summary>
        public string Purpose
        {
            get { return _purpose; }
            set { _purpose = value; }
        }

        private string _changBeiYong;
        /// <summary>
        /// 用、备
        /// </summary>
        public string ChangBeiYong
        {
            get { return _changBeiYong; }
            set { _changBeiYong = value; }
        }
        private bool _xiaofang;
        /// <summary>
        /// 是否消防
        /// </summary>
        public bool XiaoFang
        {
            get { return _xiaofang; }
            set { _xiaofang = value; }
        }
        private string _name1;
        ///所属配电箱1（常用回路/单回路）编名称
        public string Name1
        {
            get { return _name1; }
            set { _name1 = value; }
        }
        private string _name2;
        /// <summary>
        /// 所属配电箱2（备用回路）名称
        /// </summary>
        public string Name2
        {
            get { return _name2; }
            set { _name2 = value; }
        }
        private string _circuitNumber1;
        /// <summary>
        /// 常用回路/单回路的回路编号
        /// </summary>
        public string CircuitNumber1
        {
            get { return _circuitNumber1; }
            set { _circuitNumber1 = value; }
        }
        private string _circuitNumber2;
        /// <summary>
        /// 备用回路的回路编号
        /// </summary>
        public string CircuitNumber2
        {
            get { return _circuitNumber2; }
            set { _circuitNumber2 = value; }
        }
        private string _strNum1;
        ///回路编号1的内容
        public string StrNum1
        {
            get { return _strNum1; }
            set { _strNum1 = value; }
        }
        private string _strNum2;
        ///回路编号2的内容
        public string StrNum2
        {
            get { return _strNum2; }
            set { _strNum2 = value; }
        }

        private string _acPDXName;
        ///风机控制箱的名称
        public string ACPDXName
        {
            get { return _acPDXName; }
            set { _acPDXName = value; }
        }

        private string _mode;
        /// <summary>
        /// 风机类型
        /// </summary>
        public string Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        private int _baMode=0;
        /// <summary>
        /// BA的控制方式
        /// 0接入、1机电一体化、2接BA
        /// </summary>
        public int BAMode
        {
            get { return _baMode; }
            set { _baMode = value; }
        }







    }

}
