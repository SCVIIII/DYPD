using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IFoxCAD.Cad;
using IFoxDYPD.Common;
using IFoxDYPD.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace IFoxDYPD.WPF
{
    public class XTTViewModel : ObservableObject
    {
        public XTTViewModel(List<XTTHuiluDto> XTTHuilus, List<XTTACDto> XTTACHuilus)
        {
            //主程序移至过来的数据

            

            //框选得到的回路信息
            var orderHuilus = XTTHuilus.OrderBy(s => s.Name).ThenBy(s => s.CircuitNumber.Length).ThenBy(s => s.CircuitNumber);
            //框选得到的配电箱名称
            List<string> PeiDdxNames = new List<string>();
            PeiDdxNames = (from d in orderHuilus
                           orderby d.Name, d.Name.Length
                           select d.Name).Distinct().ToList();
            List_xTTHuilus = new ObservableCollection<XTTHuiluList_Dto>();

            //命令初始化
            DoubleClickCommand = new RelayCommand<string>(Test);  // 初始化命令
            // 初始化功能柜的内容 ComboBoxItems
            ListBoxItems = new ObservableCollection<string>(PeiDdxNames);
            Teststring = "测试开始";

            //回路数据处理，无需在trans中执行
            #region 回路数据处理及添加
            //初始化开关列表XTTCableSwitchDto
            List<XTTCableCircuitBreakerDto> ListXTTCableSwitchDto = new();
            ListXTTCableSwitchDto = XTTCableCircuitBreakerDto.CreateXTTCableSwitchDto();

            //开关品牌
            string brand = "Schneider";
            //微断的前缀(不带剩余电流保护)
            string CircuitBreaker_SinglePhase = "";
            //微断的前缀(带剩余电流保护)
            string CircuitBreaker_SinglePhase_VE = "";
            if (brand == "Schneider")
            {
                //单相回路的微断
                CircuitBreaker_SinglePhase = "iC65N-";  //普通照明回路的微断
                CircuitBreaker_SinglePhase_VE = "iC65N-";  //插座回路的微断
            }


            // 引入for循环，处理照明配电箱的回路数据
            for (int i = 0; i < PeiDdxNames.Count; i++)
            {
                //获取当前的配电箱名称
                string PDXNamei = PeiDdxNames[i];


                //获取当前配电箱的所有馈出回路
                var listPDXiHuilus_PDXNamei = (from d in orderHuilus
                                               where d.Name == PDXNamei
                                               select d).ToList();

                //房间配电箱回路
                var listPDXiHuilus_N = (from d in listPDXiHuilus_PDXNamei
                                        where d.CircuitNumber.StartsWith("N")
                                        orderby d.Name, d.Name.Length
                                        select d).ToList();

                //风机、水泵、风管回路
                var listPDXiHuilus_d = (from d in listPDXiHuilus_PDXNamei
                                        where d.CircuitNumber.StartsWith("d")
                                        orderby d.Name, d.Name.Length
                                        select d).ToList();

                //普通照明回路
                var listPDXiHuilus_m = (from d in listPDXiHuilus_PDXNamei
                                        where !d.CircuitNumber.Contains("mk") && d.CircuitNumber.StartsWith("m")
                                        orderby d.Name, d.Name.Length
                                        select d).ToList();
                //普通插座回路
                var listPDXiHuilus_c = (from d in listPDXiHuilus_PDXNamei
                                        where !d.CircuitNumber.Contains("ck") && d.CircuitNumber.StartsWith("c")
                                        orderby d.Name, d.Name.Length
                                        select d).ToList();
                //空调插座回路
                var listPDXiHuilus_ck = (from d in listPDXiHuilus_PDXNamei
                                         where d.CircuitNumber.StartsWith("ck")
                                         orderby d.Name, d.Name.Length
                                         select d).ToList();

                //智能照明回路
                List<XTTHuiluDto> listPDXiHuilus_mk = (from d in orderHuilus
                                                       where d.CircuitNumber.Contains("mk")
                                                       orderby d.Name, d.Name.Length
                                                       select d).ToList();

                //排序后按普通照明m,插座c,空调插座ck 添加到list中
                //当前缺少mk回路
                var listPDXiHuilus_Filter = new List<XTTHuiluDto>();
                listPDXiHuilus_Filter.AddRange(listPDXiHuilus_N);
                listPDXiHuilus_Filter.AddRange(listPDXiHuilus_d);
                listPDXiHuilus_Filter.AddRange(listPDXiHuilus_m);
                listPDXiHuilus_Filter.AddRange(listPDXiHuilus_c);
                listPDXiHuilus_Filter.AddRange(listPDXiHuilus_ck);
                //存放名为获取当前的配电箱名称的所有回路的变量
                XTTHuiluList_Dto XTTHuilus_PDXNamei = new XTTHuiluList_Dto()
                {
                    Name = PDXNamei,
                    ListHuilus = listPDXiHuilus_Filter,
                };

                //智能照明回路待加入
                //循环插入相序
                int L123 = 0;

                //第i个配电箱的普通出线回路
                for (int j = 0; j < listPDXiHuilus_Filter.Count; j++)
                {
                    #region 插入回路

                    //第一步 由回路编号分析功能
                    XTTHuiluDto huilu = listPDXiHuilus_Filter[j];
                    string circuitNumber = huilu.CircuitNumber;
                    var pe = huilu.Pe;
                    //防止回路为空
                    if (string.IsNullOrWhiteSpace(circuitNumber)) return;


                    //房间配电箱回路
                    if (circuitNumber.StartsWith("N"))
                    {
                        if(string.IsNullOrWhiteSpace(huilu.Purpose))
                        {
                            huilu.Purpose = "配电箱";
                        }
                        huilu.BlkName = "系统图开关-N";
                        huilu.Phase = "2P";
                        huilu.IsInsert = true;
                        //如果Pe为空,由程序赋设置默认值
                        if (pe == 0) { huilu.Pe = Methos_CalDefalutPe(huilu.Purpose); pe = huilu.Pe; }

                        //求照明回路整定电流--断路器10A/16A 1.75kW
                        if (pe > 0 && pe <= 1.8)
                        {
                            huilu.In = 10;
                        }
                        else if (pe > 1.8 && pe <= 2.8)
                        {
                            huilu.In = 16;
                        }
                        else if (pe > 2.8 && pe <= 3.5)
                        {
                            huilu.In = 20;
                        }
                        else if (pe > 3.5 && pe <= 4.4)
                        {
                            huilu.In = 25;
                        }
                        else if (pe > 4.4 && pe <= 5.7)
                        {
                            huilu.In = 32;
                        }
                        else if (pe > 5.7 && pe <= 10.5)
                        {
                            huilu.In = 25;
                            huilu.Phase = "3P";
                        }
                        //CircuitBreaker_SinglePrase

                        if(huilu.Phase == "2P")
                        {
                            huilu.CircuitBreaker_In = CircuitBreaker_SinglePhase + $"C{huilu.In}/{huilu.Phase}";

                        }
                        else if (huilu.Phase == "3P")
                        {
                            huilu.CircuitBreaker_In = CircuitBreaker_SinglePhase + $"C{huilu.In}/{huilu.Phase}";

                        }
                    }

                    else if(circuitNumber.StartsWith("d"))
                    {
                        //初始化开关列表XTTCableSwitchDto
                        //d为风机回路与水泵回路共用，因此插入前需先区分是控制箱or风机盘管
                        //如果用途为空，认定为风机盘管回路
                        if (string.IsNullOrWhiteSpace(huilu.Purpose )) { huilu.Purpose = "风机盘管"; }
                        //风机回路与水泵回路区分
                        if (string.IsNullOrWhiteSpace(huilu.BlkName)) { huilu.BlkName = "系统图开关-eJd1"; }

                        huilu.Phase = "1P";
                        huilu.IsInsert = true;
                        //如果Pe为空,由程序赋设置默认值
                        //如果Pe为空,由程序赋设置默认值
                        if (pe == 0) { huilu.Pe = Methos_CalDefalutPe(huilu.Purpose); pe = huilu.Pe; }

                        //求照明回路整定电流--断路器10A/16A 1.75kW
                        if (pe > 0 && pe <= 1.8)
                        {
                            huilu.In = 10;
                        }
                        else if (pe > 1.8 && pe <= 2.8)
                        {
                            huilu.In = 16;
                        }
                        else if (pe > 2.8 && pe <= 3.5)
                        {
                            huilu.In = 20;
                        }
                        else if (pe > 3.5 && pe <= 4.4)
                        {
                            huilu.In = 25;
                        }
                        else if (pe > 4.4 && pe <= 5.7)
                        {
                            huilu.In = 32;
                        }
                        //CircuitBreaker_SinglePrase
                        huilu.CircuitBreaker_In = CircuitBreaker_SinglePhase + $"D{huilu.In}/{huilu.Phase}";
                    }

                    //普通照明回路
                    else if (circuitNumber.StartsWith("m") && !circuitNumber.Contains("mk"))
                    {
                        //初始化开关列表XTTCableSwitchDto
                        //List<XTTCableCircuitBreakerDto> ListXTTCableSwitchDto = XTTCableCircuitBreakerDto.CreateXTTCableSwitchDto();
                        huilu.Purpose = "照明";
                        huilu.BlkName = "系统图开关-eJd1";
                        huilu.Phase = "1P";
                        huilu.IsInsert = true;
                        //如果Pe为空,由程序赋设置默认值
                        if (pe == 0) { huilu.Pe = Methos_CalDefalutPe(huilu.Purpose); pe = huilu.Pe; }

                        //求照明回路整定电流--断路器10A/16A 1.75kW
                        if (pe > 0 && pe <= 1.8)
                        {
                            huilu.In = 10;
                        }
                        else if (pe > 1.8 && pe <= 2.8)
                        {
                            huilu.In = 16;
                        }
                        else if (pe > 2.8 && pe <= 3.5)
                        {
                            huilu.In = 20;
                        }
                        else if (pe > 3.5 && pe <= 4.4)
                        {
                            huilu.In = 25;
                        }
                        else if (pe > 4.4 && pe <= 5.7)
                        {
                            huilu.In = 32;
                        }
                        //CircuitBreaker_SinglePrase
                        huilu.CircuitBreaker_In = CircuitBreaker_SinglePhase + $"C{huilu.In}/{huilu.Phase}";

                    } //end of if 普通照明

                    //普通插座
                    else if (circuitNumber.StartsWith("c") && !circuitNumber.Contains("ck"))
                    {
                        //初始化开关列表XTTCableSwitchDto
                        //List<XTTCableCircuitBreakerDto> ListXTTCableSwitchDto = XTTCableCircuitBreakerDto.CreateXTTCableSwitchDto();
                        huilu.IsInsert = true;
                        huilu.Purpose = "插座";
                        huilu.BlkName = "系统图开关";
                        huilu.Phase = "2P";
                        huilu.VE = "30mA";
                        //如果Pe为空,由程序赋设置默认值
                        if (pe == 0) { huilu.Pe = Methos_CalDefalutPe(huilu.Purpose); pe = huilu.Pe; }
                        //求照明回路整定电流--断路器10A/16A 1.75kW
                        if (pe > 0 && pe <= 2.8)
                        {
                            huilu.In = 16;
                        }
                        else if (pe > 2.8 && pe <= 3.5)
                        {
                            huilu.In = 20;
                        }
                        else if (pe > 3.5 && pe <= 4.4)
                        {
                            huilu.In = 25;
                        }
                        else if (pe > 4.4 && pe <= 5.7)
                        {
                            huilu.In = 32;
                        }
                        else if (pe > 5.7 && pe <= 7.1)
                        {
                            huilu.In = 40;
                        }
                        else if (pe > 7.1 && pe <= 8.9)
                        {
                            huilu.In = 50;
                        }
                        huilu.CircuitBreaker_In = CircuitBreaker_SinglePhase + $"C{huilu.In}+Vigi/{huilu.Phase}";

                    } //end of if 普通插座

                    //空调插座
                    else if (circuitNumber.StartsWith("ck"))
                    {
                        //初始化开关列表XTTCableSwitchDto
                        //List<XTTCableCircuitBreakerDto> ListXTTCableSwitchDto = XTTCableCircuitBreakerDto.CreateXTTCableSwitchDto();
                        huilu.IsInsert = true;
                        huilu.Purpose = "空调插座";
                        huilu.BlkName = "系统图开关";
                        huilu.Phase = "2P";
                        //如果Pe为空,由程序赋设置默认值
                        if (pe == 0) { huilu.Pe = Methos_CalDefalutPe(huilu.Purpose); pe = huilu.Pe; }
                        //求照明回路整定电流--断路器10A/16A 1.75kW
                        if (pe > 0 && pe <= 2)
                        {
                            huilu.In = 16;
                        }
                        else if (pe > 2 && pe <= 3.5)
                        {
                            huilu.In = 20;
                        }
                        else if (pe > 3.5 && pe <= 4.4)
                        {
                            huilu.In = 25;
                        }
                        else if (pe > 4.4 && pe <= 5.7)
                        {
                            huilu.In = 32;
                        }
                        else if (pe > 5.7 && pe <= 7.1)
                        {
                            huilu.In = 40;
                        }
                        else if (pe > 7.1 && pe <= 8.9)
                        {
                            huilu.In = 50;
                        }
                        huilu.CircuitBreaker_In = CircuitBreaker_SinglePhase + $"D{huilu.In}+Vigi/{huilu.Phase}";
                    } //end of if 空调插座

                    //当满足插入条件时，才添加通用属性
                    //单相回路的导线信息
                    if (huilu.IsInsert && (huilu.Phase == "1P" || huilu.Phase == "2P"))
                    {
                        //电缆截面
                        //此项待修正2024年10月22日
                        bool CableResult = Method_GetXTTCable(ListXTTCableSwitchDto, huilu.In, out string Cable, out int SC);
                        if (CableResult) { huilu.Cable = $"WDZ-BYJ-2x{Cable}-JDG{SC}-MR/WC/CC"; }
                        else return;
                        //相序
                        huilu.L123 = $"L{(L123 % 3) + 1}";
                        L123++;
                    }
                    else if (huilu.IsInsert && (huilu.Phase == "3P" || huilu.Phase == "4P"))
                    {
                        //三相电缆截面的功能函数待校验
                        //此项待修正2024年11月25日
                        bool CableResult = Method_GetXTTCable(ListXTTCableSwitchDto, huilu.In, out string Cable, out int SC);
                        if (CableResult) { huilu.Cable = $"WDZ-YJY-4x{Cable}-JDG{SC}-MR/WC/CC"; }
                        else return;
                        //相序
                        huilu.L123 = "L123";
                    }

                    #endregion
                } //end of for j
                //将处理好的当前配电箱加入List_xTTHuilus中
                List_xTTHuilus.Add(XTTHuilus_PDXNamei);
            } //end of for i

            #endregion

            //导入CAD图块
            using (DBTrans tr = new DBTrans())
            {

                //导入图块
                //getblockfrom函数的第一个参数是文件路径，第二个参数是是否强制覆盖本图的块定义
                var id3 = tr.BlockTable.GetBlockFrom("D:\\2020项目\\测试代码\\系统图生成v1.1(2023.10.11)\\Lib\\系统图开关-N_t3.dwg", false);
                var id1 = tr.BlockTable.GetBlockFrom(@"D:\2020项目\测试代码\系统图生成v1.1(2023.10.11)\Lib\系统图开关.dwg", false);
                var id2 = tr.BlockTable.GetBlockFrom(@"D:\2020项目\测试代码\系统图生成v1.1(2023.10.11)\Lib\系统图开关-eJd.dwg", false);
            }

        }

        public void Test(string obj)
        {
            Teststring = obj;

            #region 插入块的实际指令区
            using (DBTrans tr = new DBTrans())
            {
                #region 主程序中移植过来的程序代码
                Editor ed = tr.Editor;


                // 显示中间量的调试代码，待删除
                // 构造要显示的字符串
                //string message = string.Join("\n", orderHuilus.Select(dto => $"Name: {dto.Name}, Pe: {dto.Pe}, HuiLu: {dto.CircuitNumber}"));


                // 检查图层是否存在
                // 检测是否存在所需的图层
                string LayerInsert = "LayerInsert";
                Common.LayerTools.IfHasLayer(tr, LayerInsert, 3);
                Common.LayerTools.IfHasLayer(tr, "LayerChouti", 7);

                //.由用户选择起始插入点
                // 获取用户插入点
                var ppo = new PromptPointOptions(Environment.NewLine + "\n指定标注点:<空格退出>")
                {
                    AllowArbitraryInput = true,// 任意输入
                    AllowNone = true // 允许回车
                };
                var ppr = ed.GetPoint(ppo);// 用户点选
                Point3d ptInsert = ppr.Value.PolarPoint(Math.PI / 2, -1500);
                if (ptInsert == null) return;

                //传入的参量是配电箱名称
                string PDXNamei = obj;
                //获取当前配电箱对应的list<Name,list>
                var list_PDX_NameAndHuilus_Insert = List_xTTHuilus.Where(d => d.Name == PDXNamei).Select(d => d).ToList().FirstOrDefault();

                if (list_PDX_NameAndHuilus_Insert.ListHuilus.Count > 0)
                {
                    var listPDXiHuilus_Insert = list_PDX_NameAndHuilus_Insert.ListHuilus;
                    //第i个配电箱的普通出线回路
                    for (int j = 0; j < listPDXiHuilus_Insert.Count; j++)
                    {
                        #region 插入回路

                        //第一步 由回路编号分析功能
                        XTTHuiluDto huilu = listPDXiHuilus_Insert[j];

                        //当满足插入条件时，才添加通用属性
                        if (huilu.IsInsert)
                        {
                            //开始插入文字、图块
                            //相序
                            DBText dBTextLN = new DBText()
                            {
                                TextString = huilu.L123,
                                Layer = LayerInsert,
                                Position = ptInsert.PolarPoint(0, 100).PolarPoint(Math.PI / 2, 200),
                                Height = 350,
                                WidthFactor = 0.7,
                            };
                            tr.CurrentSpace.AddEntity(dBTextLN);
                            //断路器
                            DBText dBTextSwitch = new DBText()
                            {
                                TextString = huilu.CircuitBreaker_In,
                                Layer = LayerInsert,
                                Position = ptInsert.PolarPoint(0, 2100).PolarPoint(Math.PI / 2, 200),
                                Height = 350,
                                WidthFactor = 0.7,
                            };
                            tr.CurrentSpace.AddEntity(dBTextSwitch);
                            //电缆
                            DBText dBTextCable = new DBText()
                            {
                                TextString = huilu.Cable,
                                Layer = LayerInsert,
                                Position = ptInsert.PolarPoint(0, 7400).PolarPoint(Math.PI / 2, 200),
                                Height = 300,
                                WidthFactor = 0.6,
                            };
                            tr.CurrentSpace.AddEntity(dBTextCable);
                            //回路
                            DBText dBTextHuilu = new DBText()
                            {
                                TextString = huilu.CircuitNumber,
                                Layer = LayerInsert,
                                Position = ptInsert.PolarPoint(0, 6400).PolarPoint(Math.PI / 2, 200),
                                Height = 350,
                                WidthFactor = 0.7,
                            };
                            tr.CurrentSpace.AddEntity(dBTextHuilu);
                            //用途
                            DBText dBTextPurpose = new DBText()
                            {
                                TextString = huilu.Purpose,
                                Layer = LayerInsert,
                                Position = ptInsert.PolarPoint(0, 14550).PolarPoint(Math.PI / 2, 200),
                                Height = 350,
                                WidthFactor = 0.7,
                            };
                            tr.CurrentSpace.AddEntity(dBTextPurpose);
                            //Pe
                            DBText dBTextPe = new DBText()
                            {
                                TextString = huilu.Pe.ToString() + "kW",
                                Layer = LayerInsert,
                                Position = ptInsert.PolarPoint(0, 16000).PolarPoint(Math.PI / 2, 200),
                                Height = 350,
                                WidthFactor = 0.7,
                            };
                            tr.CurrentSpace.AddEntity(dBTextPe);
                            //插入块
                            var objectId = tr.CurrentSpace.InsertBlock(ptInsert, huilu.BlkName);
                            ptInsert = ptInsert.PolarPoint(Math.PI / 2, -1500);
                        }

                        #endregion
                    } //end of for j
                }
                else
                {
                    System.Windows.MessageBox.Show($"未找到配电箱:{PDXNamei}", "XTTHuiluDto Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                #endregion
            }

            #endregion


        }

        //ListBox 存放配电箱名
        private ObservableCollection<string> _listboBoxItems;

        public ObservableCollection<string> ListBoxItems
        {
            get => _listboBoxItems;
            set => SetProperty(ref _listboBoxItems, value);
        }
        //存放筛选处理后的配电箱及对应回路信息
        private ObservableCollection<XTTHuiluList_Dto> _list_xTTHuilus;
        public ObservableCollection<XTTHuiluList_Dto> List_xTTHuilus
        {
            get => _list_xTTHuilus;
            set => SetProperty(ref _list_xTTHuilus, value);
        }

        //测试用显示文字
        private string _teststring;
        public string Teststring
        {
            get => _teststring;
            set => SetProperty(ref _teststring, value);
        }


        public RelayCommand<string> DoubleClickCommand { get; }


        private static bool Method_GetXTTCable(List<XTTCableCircuitBreakerDto> ListXTTCableSwitchDto, int In, out string Cable, out int SC)
        {
            var dto = ListXTTCableSwitchDto.Where(d => d.In == In).Select(d => d).ToList().FirstOrDefault();
            Cable = dto.Cable;
            SC = dto.SC;
            if (dto == null) return false;
            else return true;
        }

        //根据功能,确定Pe的默认值
        private double Methos_CalDefalutPe(string purpose)
        {
            double pe = 0;
            switch(purpose)
            {
                case "照明": 
                    pe = 0.2; 
                    break;
                case "插座":
                    pe = 0.5;
                    break;
                case "空调插座":
                    pe = 2.5;
                    break;
            }
            return pe;

        }
    }

}
