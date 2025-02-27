using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IFoxDYPD.Common;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using IFoxDYPD.Dtos;
using System.Windows.Controls;
using System.Windows.Media;
using Autodesk.AutoCAD.Geometry;
using static MaterialDesignThemes.Wpf.Theme;
using IFoxCAD.Cad;
using IFoxDYPD.SQL;
using MySql.Data.MySqlClient;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;
using Color = Autodesk.AutoCAD.Colors.Color;
using K4os.Compression.LZ4.Internal;
using System.Windows.Forms;
using static IFoxDYPD.Dtos.HuiLuDtos;
using System.ComponentModel;
using Autodesk.AutoCAD.GraphicsSystem;

namespace IFoxDYPD.WPF
{
    public class DYPDViewModel : ObservableObject
    {
        public List<ExcelJiSuanshuHuiLuDto> rows_Form = new();
        public List<string> list_purpose_pub = new List<string>()
        {
            "未设置",
            "进线柜",
            "补偿柜",
            "联络柜",
            "馈电柜"
        };

        #region 关闭窗口测试
        public ICommand CloseCommand { get; }
        private void CloseWindow()
        {
            // 触发关闭事件或其他逻辑
            RequestClose?.Invoke();
        }
        public event Action RequestClose; // 关闭请求事件

        #endregion
        public DYPDViewModel()
        {

            //环境初始化
            Tsetstring = "MVVM测试";
            Title = "Window3 - MVVM Example";  // 初始化窗口的标题
            IsImportDwg = "导入DWG文件成功";    //初始化DWG导入状态
            DoSomethingCommand = new RelayCommand(DoSomething);  // 初始化命令
            InsertBlockCommand = new RelayCommand(InsertBlocksMethod);  // 插入图块的命令
            IsWaiting = "Hidden";

            //初始化Binding命令
            TestCommand2 = new RelayCommand(Test2);  // 初始化命令
            TestCommand3 = new RelayCommand(Test3);  // 初始化命令
            CloseCommand = new RelayCommand(CloseWindow);

            // 初始化功能柜的内容 ComboBoxItems
            ComboBoxItems = new ObservableCollection<string>
            {
                "未设置",
                "进线柜-315kVA",
                "进线柜-400kVA",
                "进线柜-500kVA",
                "进线柜-630kVA",
                "进线柜-800kVA",
                "进线柜-1000kVA",
                "进线柜-1250kVA",
                "进线柜-1600kVA",
                "进线柜-2000kVA",
                "补偿柜-100kvar",
                "补偿柜-120kvar",
                "补偿柜-150kvar",
                "补偿柜-200kvar",
                "补偿柜-250kvar",
                "补偿柜-300kvar",
                "联络柜",
                "馈电柜"
            };


            //从EXCEL获取EXCEL的info信息（sheetname数量、各变压器对应的馈电柜数量）
            //程序以list<string>形式返回结果
            Excelinfos = new ExcelDto();
            var db = Env.Database;
            rows_Form = Common.USRTools.ImportDataFromExcel(db, Excelinfos);

            // 初始化 MemoDtos 集合
            // 初始化TA、TB磁贴存放的所需变量
            MemoDtos_TA = new ObservableCollection<MemoDto>();
            MemoDtos_TB = new ObservableCollection<MemoDto>();
            GetMemosMethod();
            // 添加事件监听
            // 1.TA与TB的第一个低压柜关联为相同内容

            if (MemoDtos_TA.Count > 0) MemoDtos_TA[0].PropertyChanged += MemoDto_PropertyChanged;
            if (MemoDtos_TB.Count > 0) MemoDtos_TB[0].PropertyChanged += MemoDto_PropertyChanged;

            // 2.TA与TB的布置方式关联:togglebutton互斥、文字显示自动变更
            // 关联的执行命令
            IsToggleButton1Checked = false;
            IsToggleButton2Checked = true;

            //导入图块
            using (DBTrans tr = new DBTrans())
            {
                try
                {
                    //getblockfrom函数的第一个参数是文件路径，第二个参数是是否强制覆盖本图的块定义
                    var id = tr.BlockTable.GetBlockFrom(@"D:\Mycode\BlkDYPD\BlkDYPD_t3.dwg", false);
                    //增加显示DWG文件的导入状态
                    if (id == null) IsImportDwg = "导入DWG文件失败";
                }
                catch (Exception ex)
                {
                    return;
                    throw ex;
                }

            }

            //调试用断点
            if (true)
            {

            }
        }

        /// <summary>
        /// 提取低压柜信息,用于显示TA,TB的磁贴
        /// </summary>
        private void GetMemosMethod()
        {

            #region 提取低压柜信息,用于显示TA,TB的磁贴
            //为TA变压器添加进线及补偿柜
            for (int i = 0; i < Excelinfos.Nth_FirstKuiChuGui_TA - 1; i++)
            {
                MemoDto memoDto = new MemoDto();
                memoDto.ID = Excelinfos.Pre_TA + (i + 1);
                memoDto.Property1 = $"前缀{Excelinfos.Pre_TA}";
                memoDto.Property2 = list_purpose_pub[0];
                memoDto.SelectedComboBoxItem = ComboBoxItems[0];
                MemoDtos_TA.Add(memoDto);
            }
            //为TA变压器添加馈电柜
            for (int i = 0; i < Excelinfos.Num_KuiDianGui_TA; i++)
            {
                MemoDto memoDto = new MemoDto();
                memoDto.ID = Excelinfos.GuiHao_TA[i];
                memoDto.Property1 = $"前缀{Excelinfos.Pre_TA}";
                memoDto.Property2 = list_purpose_pub[4];
                memoDto.SelectedComboBoxItem = ComboBoxItems[17];
                MemoDtos_TA.Add(memoDto);
            }


            //为TB变压器添加进线及补偿柜
            for (int i = 0; i < Excelinfos.Nth_FirstKuiChuGui_TB - 1; i++)
            {
                MemoDto memoDto = new MemoDto();
                memoDto.ID = Excelinfos.Pre_TB + (i + 1);
                memoDto.Property1 = $"前缀{Excelinfos.Pre_TB}";
                memoDto.Property2 = list_purpose_pub[0];
                memoDto.SelectedComboBoxItem = ComboBoxItems[0];

                MemoDtos_TB.Add(memoDto);
            }
            //为TA变压器添加馈电柜
            for (int i = 0; i < Excelinfos.Num_KuiDianGui_TB; i++)
            {
                MemoDto memoDto = new MemoDto();
                memoDto.ID = Excelinfos.GuiHao_TB[i];
                memoDto.Property1 = $"前缀{Excelinfos.Pre_TB}";
                memoDto.Property2 = list_purpose_pub[4];
                memoDto.SelectedComboBoxItem = ComboBoxItems[17];
                MemoDtos_TB.Add(memoDto);
            }
            #endregion
        }

        private void Test2()
        {
           

        }

        //测试断点
        private void Test3()
        {
            if(true)
            {

            }
           
        }



        #region 变量与ICommand

        // ViewModel的属性
        private bool _isToggleButton1Checked;
        private bool _isToggleButton2Checked;

        public bool IsToggleButton1Checked
        {
            get => _isToggleButton1Checked;
            set
            {
                SetProperty(ref _isToggleButton1Checked, value);
                if (value)
                {
                    IsToggleButton2Checked = false;
                    Excelinfos.Fanzhuan_TA = IsToggleButton1Checked;
                    Excelinfos.Fanzhuan_TB = IsToggleButton2Checked;

                }
                else if (!_isToggleButton2Checked)
                {
                    // If ToggleButton1 is unchecked and ToggleButton2 is also unchecked,
                    // ensure one of them gets checked.
                    IsToggleButton2Checked = true;
                    Excelinfos.Fanzhuan_TA = IsToggleButton1Checked;
                    Excelinfos.Fanzhuan_TB = IsToggleButton2Checked;
                }
            }
        }

        public bool IsToggleButton2Checked
        {
            get => _isToggleButton2Checked;
            set
            {
                SetProperty(ref _isToggleButton2Checked, value);
                if (value)
                {
                    IsToggleButton1Checked = false;
                    Excelinfos.Fanzhuan_TA = IsToggleButton1Checked;
                    Excelinfos.Fanzhuan_TB = IsToggleButton2Checked;
                }
                else if (!_isToggleButton1Checked)
                {
                    // If ToggleButton2 is unchecked and ToggleButton1 is also unchecked,
                    // ensure one of them gets checked.
                    IsToggleButton1Checked = true;
                    Excelinfos.Fanzhuan_TA = IsToggleButton1Checked;
                    Excelinfos.Fanzhuan_TB = IsToggleButton2Checked;
                }
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _isImportDwg;
        public string IsImportDwg
        {
            get => _isImportDwg;
            set => SetProperty(ref _isImportDwg, value);
        }

        private ObservableCollection<string> _comboBoxItems;

        public ObservableCollection<string> ComboBoxItems
        {
            get => _comboBoxItems;
            set => SetProperty(ref _comboBoxItems, value);
        }

        // 读取并筛选后的EXCEL中具有的表名、变压器数量等信息
        private ExcelDto _excelinfos;
        public ExcelDto Excelinfos
        {
            get => _excelinfos;
            set => SetProperty(ref _excelinfos, value);
        }

        //测试用显示文字
        private string _teststring;
        public string Tsetstring
        {
            get => _teststring;
            set => SetProperty(ref _teststring, value);
        }

        

        private List<string> _listpuroposes = new List<string>()
        {
            "未设置",
            "进线柜",
            "补偿柜",
            "联络柜",
            "馈电柜"
        };

        public List<string> ListPuroposes
        {
            get { return _listpuroposes; }
            set { _listpuroposes = value; }
        }


        // 一个示例命令
        public RelayCommand DoSomethingCommand { get; }
        // 测试使用命令
        public RelayCommand InsertBlockCommand { get; }
        public RelayCommand TestCommand2 { get; }
        public RelayCommand TestCommand3 { get; }
        public ObservableCollection<MemoDto> MemoDtos_TA { get; set; }
        public ObservableCollection<MemoDto> MemoDtos_TB { get; set; }

        public class MemoDto : ObservableObject
        {
            private string _id;
            public string ID
            {
                get => _id;
                set => SetProperty(ref _id, value);
            }

            private string _property1;
            public string Property1
            {
                get => _property1;
                set => SetProperty(ref _property1, value);
            }

            private string _property2;
            public string Property2
            {
                get => _property2;
                set => SetProperty(ref _property2, value);
            }

            // 新增属性用于绑定ComboBox选中项
            private string _selectedComboBoxItem;
            public readonly List<string> list_purpose = new List<string>()
            {
                "未设置",
                "进线柜",
                "补偿柜",
                "联络柜",
                "馈电柜"
            };

            public string SelectedComboBoxItem
            {
                get => _selectedComboBoxItem;
                set
                {
                    if (SetProperty(ref _selectedComboBoxItem, value))
                    {
                        // 调用子函数根据选中的值更新 Property2
                        UpdateProperty2(value);
                    }
                    //SetProperty(ref _selectedComboBoxItem, value);
                    //// 更新Property2为选中的ComboBox项
                    //if (value.Contains(list_purpose[0])) { Property2 = list_purpose[0]; }
                    //if (value.Contains(list_purpose[1])) { Property2 = list_purpose[1]; }
                    //if (value.Contains(list_purpose[2])) { Property2 = list_purpose[2]; }
                    //if (value.Contains(list_purpose[3])) { Property2 = list_purpose[3]; }
                    //if (value.Contains(list_purpose[4])) { Property2 = list_purpose[4]; }
                }


            }
            private  void UpdateProperty2(string selectedItem)
            {
                // 根据选中的 ComboBox 项目更新 Property2
                if (selectedItem.Contains(list_purpose[0]))
                {
                    Property2 = list_purpose[0];
                }
                else if (selectedItem.Contains(list_purpose[1]))
                {
                    Property2 = list_purpose[1];
                }
                else if (selectedItem.Contains(list_purpose[2]))
                {
                    Property2 = list_purpose[2];
                }
                else if (selectedItem.Contains(list_purpose[3]))
                {
                    Property2 = list_purpose[3];
                }
                else if (selectedItem.Contains(list_purpose[4]))
                {
                    Property2 = list_purpose[4];
                }
            }
        }


        #endregion

        #region Loading时间（异步待确认）
        private string _isWating;
        public string IsWaiting
        {
            get => _isWating;
            set => SetProperty(ref _isWating, value);
        }

        

        #endregion

        // 监听 MemoDto 的 PropertyChanged 事件
        private void MemoDto_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MemoDto.SelectedComboBoxItem))
            {
                var changedMemo = sender as MemoDto;

                if (changedMemo != null)
                {
                    // 如果 MemoDtos_TA[0] 改变，更新 MemoDtos_TB[0]
                    if (MemoDtos_TA[0] == changedMemo)
                    {
                        if (MemoDtos_TB.Count > 0) MemoDtos_TB[0].SelectedComboBoxItem = MemoDtos_TA[0].SelectedComboBoxItem;
                    }
                    // 如果 MemoDtos_TB[0] 改变，更新 MemoDtos_TA[0]
                    else if (MemoDtos_TB[0] == changedMemo)
                    {
                        if (MemoDtos_TA.Count > 0) MemoDtos_TA[0].SelectedComboBoxItem = MemoDtos_TB[0].SelectedComboBoxItem;
                    }
                }
            }
        } //end of private void MemoDto_PropertyChanged(object sender, PropertyChangedEventArgs e)

        private void InsertBlocksMethod()
        {


            //测试入口2024年9月27日14:22:50
            #region 测试入口----逐个低压柜插入块

            //2.打开Using dBtrans
            using (DBTrans tr = new DBTrans())
            {



                //3.初始化插入环境
                //3-1名称、变量初始化
                //图层名称
                string LayerInsert = "E-SYST-HLVO-EQPM";  //低压柜的插入图层
                string LayerChouti = "E-SYST-HLVO-CRIC";  //出现回路模块的插入图层
                Scale3d InsertScale = new Scale3d(1);     //插入比例
                



                //4.由用户选择起始插入点
                //获取用户插入点
                var ed = Env.Editor;
                var ppo = new PromptPointOptions(Environment.NewLine + "\n指定标注点:<空格退出>")
                {
                    AllowArbitraryInput = true,// 任意输入
                    AllowNone = true // 允许回车
                };
                var ppr = ed.GetPoint(ppo);// 用户点选

                //调出等待页面
                IsWaiting = "Visible";


                Point3d ptStart = ppr.Value;
                if (ptStart == null) return;
                //新建liST存放馈线柜的插入点，若不插入功能柜，则使用默认值
                List<Point3d> PT_LIST_END = new List<Point3d>()
                {
                            ptStart,  //第一行的插入点
                            ptStart.PolarPoint(Math.PI/2,-50000) //第二行的插入点
                };

                //待修正的变量
                //待添加或修改的变量
                //此项应添加属性、移入for循环

                //初始插入点
                var ptDiYagui = ptStart;
                

                //3-2图层初始化
                if (!tr.LayerTable.Has(LayerInsert))
                {
                    //要执行的操作
                    tr.LayerTable.Add(LayerInsert, it =>
                    {
                        it.Color = Color.FromColorIndex(ColorMethod.ByColor, 3);
                        //it.LineWeight = LineWeight.ByLineWeightDefault;
                        it.IsPlottable = true;
                    });
                }
                //检测是否存在所需的图层
                Common.LayerTools.IfHasLayer(tr, LayerInsert, 3);
                Common.LayerTools.IfHasLayer(tr, LayerChouti, 7);


                //5.打开if校验WPF中TA变压器对应的Dtos是否有数据
                if (MemoDtos_TA.Count() > 0)
                {
                    int diastance_polar = 8000;               //功能柜从第一个开始插入,从左向右插入,因此偏移为正
                                                              //块名
                    string BLK_INCOMING = "EB-DYDP-进线柜-右出线1";
                    string BLK_LDC = "EB-DYPD-低压柜-联络柜1";
                    string BLK_MTVS = "EB-DYPD-低压柜-补偿柜1";
                    //是否需翻转为从右到左布置
                    if (Excelinfos.Fanzhuan_TA)
                    {
                        diastance_polar = -8000;
                        //进线柜对应的块名
                        BLK_INCOMING = "EB-DYDP-进线柜-左出线1";
                        BLK_LDC = "EB-DYPD-低压柜-联络柜2";
                        //待确认，是否保留的函数
                        ptDiYagui = ptDiYagui.PolarPoint(0, diastance_polar);
                    }
                    //插入TA对应的所有低压柜
                    MethodKuiDianGui(MemoDtos_TA, tr, LayerInsert, LayerChouti, InsertScale, diastance_polar, BLK_INCOMING, BLK_LDC, BLK_MTVS, ref ptDiYagui);

                }

                
                if (MemoDtos_TB.Count() > 0)
                {
                    int diastance_polar = 8000;               //功能柜从第一个开始插入,从左向右插入,因此偏移为正
                                                              //块名
                    string BLK_INCOMING = "EB-DYDP-进线柜-右出线1";
                    string BLK_LDC = "EB-DYPD-低压柜-联络柜1";
                    string BLK_MTVS = "EB-DYPD-低压柜-补偿柜1";
                    var pt2 = PT_LIST_END[1];
                    //是否需翻转为从右到左布置
                    if (Excelinfos.Fanzhuan_TB)
                    {
                        diastance_polar = -8000;
                        //进线柜对应的块名
                        BLK_INCOMING = "EB-DYDP-进线柜-左出线1";
                        BLK_LDC = "EB-DYPD-低压柜-联络柜2";
                        //待确认，是否保留的函数
                        ptDiYagui = ptDiYagui.PolarPoint(0, diastance_polar);
                    }
                    //插入TB对应的所有低压柜
                    MethodKuiDianGui(MemoDtos_TB, tr, LayerInsert, LayerChouti, InsertScale, diastance_polar, BLK_INCOMING, BLK_LDC, BLK_MTVS, ref pt2);

                }

            } //end of using


            //测试点
            if (true)
            {
                
            }

            #endregion
            //插入完成后关闭窗口
            CloseWindow();
        }  //end of private void Test()

        /// <summary>
        /// 插入馈电柜及对应附件、回路
        /// </summary>
        /// <param name="MemoDtos_Now"></param>
        /// <param name="tr"></param>
        /// <param name="LayerInsert"></param>
        /// <param name="LayerChouti"></param>
        /// <param name="InsertScale"></param>
        /// <param name="diastance_polar"></param>
        /// <param name="BLK_INCOMING"></param>
        /// <param name="BLK_LDC"></param>
        /// <param name="BLK_MTVS"></param>
        /// <param name="ptDiYagui"></param>
        private void MethodKuiDianGui(ObservableCollection<MemoDto> MemoDtos_Now, DBTrans tr, string LayerInsert, string LayerChouti, Scale3d InsertScale, int diastance_polar, string BLK_INCOMING, string BLK_LDC, string BLK_MTVS, ref Point3d ptDiYagui)
        {

            if (MemoDtos_Now.Count() > 0)
            {
                // 1.打开数据库连接
                var connector = new IFoxDYPD.SQL.MySQLConnector();
                using (var conn = connector.ConnMySQL())
                {
                    int capacity = 0;  //变压器容量初始化
                    string COOPER_BAR = "";  //铜牌规格

                    //Linq筛选出变压器信息
                    var infocapacity = (from d in MemoDtos_Now
                                        where d.Property2 == "进线柜"
                                        select d).ToList();
                    //获取变压器信息后，尝试计算变压器容量、从MYSQL查询铜牌规格
                    if (infocapacity.Count() == 1)
                    {
                        //根据进线柜的string截取出进线柜容量
                        string str_capacity = infocapacity[0].SelectedComboBoxItem.Substring(4, infocapacity[0].SelectedComboBoxItem.Length - 7);
                        //变压器容量（整数）
                        capacity = int.Parse(str_capacity);
                        //铜排规格（Cu-630A-4(50x5)/35kA/1s（相线和PEN线同截面））
                        var queryCoperFromSQL = new IFoxDYPD.SQL.QueryFromSQL();
                        COOPER_BAR = queryCoperFromSQL.QueryCooperBAR(conn, capacity);
                    }
                    else
                    {
                        //待补充：提示信息 未找到变压器信息
                        return;
                    }


                    //6.打开for循环逐个分析TA变压器带的低压柜
                    for (int i = 0; i < MemoDtos_Now.Count; i++)
                    {
                        //7.打开if-else判断,对不同功能执行不同函数
                        //读取WPF显示的低压柜功能
                        //0--"未设置"  1--"进线柜"  2--"补偿柜"  3--"联络柜"  4--"馈电柜"
                        string purpose = MemoDtos_Now[i].Property2;
                        //此为中间变量，调试使用
                        var memo = MemoDtos_Now[i];
                        //7-1未设置
                        if (purpose == list_purpose_pub[0])
                        {
                            //待补充
                        }
                        //7-2进线柜
                        else if (purpose == list_purpose_pub[1])
                        {
                            //从数据库中查询相关信息并转换为对应类
                            var queryFromSQL = new IFoxDYPD.SQL.QueryFromSQL();
                            DYPD_INCOMING_CABINET myobj = queryFromSQL.QueryIncomingCabinet(conn, capacity)[0];
                            //新建字典,用于存放属性
                            Dictionary<string, string> attMulti = new Dictionary<string, string> { };
                            attMulti.Add("EB16-进线及联络开关", "K1");
                            attMulti.Add("EB11-低压柜编号", memo.ID);
                            attMulti.Add("EB12-低压柜功能", myobj.EB12_PURPOSE);
                            attMulti.Add("EB13-低压柜尺寸", myobj.EB13_SIZE);
                            attMulti.Add("EB14-低压柜类型", myobj.EB14_TYPE);
                            attMulti.Add("EB17-短延时整定电流", myobj.EB17_IR2);
                            attMulti.Add("EB18-断路器规格", myobj.EB18_BREAKER);
                            attMulti.Add("EB19-额定电流", myobj.EB19_In);
                            attMulti.Add("EB20-长延时整定电流", myobj.EB20_Ir1);
                            attMulti.Add("EB21-短路瞬动", myobj.EB21_Ir3);
                            attMulti.Add("EB22-互感器规格", myobj.EB22_CT);
                            attMulti.Add("EB23-通讯接口", myobj.EB23_INFO1);
                            attMulti.Add("EB31-变压器规格", myobj.EB31_TRANSFORMER);
                            attMulti.Add("EB32-变压器电压等级", myobj.EB32_VOLTAGE);
                            attMulti.Add("EB33-变压器阻抗电压", myobj.EB33_Ud);
                            attMulti.Add("EB34-变压器IP防护", myobj.EB34_IP);
                            attMulti.Add("EB35-变压器尺寸", myobj.EB35_SIZE);
                            attMulti.Add("EB36-变压器备注", myobj.EB36_INFO2);
                            attMulti.Add("EB41-进线电缆", myobj.EB41_IMCOMING_CABLE);
                            attMulti.Add("EB42-母线槽规格", myobj.EB42_BUSBAR);
                            attMulti.Add("EB43-母线槽要求", myobj.EB43_BUSBAR_INFO);
                            attMulti.Add("EB44-接地铜排", myobj.EB44_BUSBAR_PE);

                            //插入块
                            var objectId = tr.CurrentSpace.InsertBlock(ptDiYagui, BLK_INCOMING, InsertScale, atts: attMulti);
                            //IFOXCad插入块时仅支持设置插入点、块名、比例、旋转、块属性
                            //获取ent，用于设置其他属性：图层
                            var ent = objectId.GetObject<Entity>();
                            ent.Layer = LayerInsert;

                        }
                        //7-3补偿柜
                        else if (purpose == list_purpose_pub[2])
                        {
                            //判断变压器容量是否已设置
                            if (capacity == 0)
                            {
                                //MessageBox.Show("变压器容量未找到");
                            } //end of if(capacity == 0)
                              //变压器容量已设置时，继续函数
                            else
                            {
                                //根据电容柜的string截取出电容柜容量
                                string str_MTVS_capacity = memo.SelectedComboBoxItem.Substring(4, memo.SelectedComboBoxItem.Length - 8);
                                int MTVS_capacity = int.Parse(str_MTVS_capacity);

                                //从数据库中查询相关信息并转换为对应类
                                var queryFromSQL = new IFoxDYPD.SQL.QueryFromSQL();
                                MTVSDto myobj = queryFromSQL.QueryMTVS(conn, MTVS_capacity)[0];
                                //新建字典,用于存放属性
                                Dictionary<string, string> atts2 = new Dictionary<string, string> { };
                                atts2.Add("EB11-低压柜编号", memo.ID);
                                atts2.Add("EB12-低压柜功能", myobj.EB12_PURPOSE);
                                atts2.Add("EB13-低压柜尺寸", myobj.EB13_SIZE);
                                atts2.Add("EB14-低压柜类型", myobj.EB14_TYPE);
                                //铜排规格由数据库提供
                                if (!string.IsNullOrEmpty(COOPER_BAR)) { atts2.Add("EB21-低压母线规格", COOPER_BAR); }
                                atts2.Add("EB31-总无功补偿有效输出", myobj.EB31_CAPACITY);
                                atts2.Add("EB32-三相无功补偿有效输出", myobj.EB32_CAPACITY_3P);
                                atts2.Add("EB33-单相无功补偿有效输出", myobj.EB33_CAPACITY_1P);
                                atts2.Add("EB34-熔断器规格", myobj.EB34_FUSE);
                                atts2.Add("EB35-电流互感器规格", myobj.EB35_CT);
                                atts2.Add("EB36-功率因数控制器", myobj.EB36_INFO1);
                                atts2.Add("EB37-电抗率", myobj.EB37_INFO2);
                                atts2.Add("EB38-避雷器", myobj.EB38_INFO3);
                                atts2.Add("EB41-功能要求A", myobj.EB41_INFOA);
                                atts2.Add("EB42-功能要求B", myobj.EB42_INFOB);
                                atts2.Add("EB43-功能要求C", myobj.EB43_INFOC);
                                atts2.Add("EB44-功能要求D", myobj.EB44_INFOD);

                                //插入块
                                var objectId = tr.CurrentSpace.InsertBlock(ptDiYagui, BLK_MTVS, InsertScale, atts: atts2);
                                //IFOXCad插入块时仅支持设置插入点、块名、比例、旋转、块属性
                                //获取ent，用于设置其他属性：图层
                                var ent = objectId.GetObject<Entity>();
                                ent.Layer = LayerInsert;

                            } // end of else
                        }
                        //7-4联络柜
                        else if (purpose == list_purpose_pub[3])
                        {


                            //从数据库中查询相关信息并转换为对应类
                            var queryFromSQL = new IFoxDYPD.SQL.QueryFromSQL();
                            DYPD_INCOMING_CABINET myobj = queryFromSQL.QueryIncomingCabinet(conn, capacity)[0];
                            //新建字典,用于存放属性
                            Dictionary<string, string> atts2 = new Dictionary<string, string> { };
                            atts2.Add("EB16-进线及联络开关", "K3");
                            atts2.Add("EB11-低压柜编号", memo.ID);
                            atts2.Add("EB12-低压柜功能", myobj.EB12_PURPOSE);
                            atts2.Add("EB13-低压柜尺寸", myobj.EB13_SIZE);
                            atts2.Add("EB14-低压柜类型", "联络柜");
                            atts2.Add("EB17-短延时整定电流", myobj.EB17_IR2);
                            atts2.Add("EB18-断路器规格", myobj.EB18_BREAKER);
                            atts2.Add("EB19-额定电流", myobj.EB19_In);
                            atts2.Add("EB20-长延时整定电流", myobj.EB20_Ir1);
                            atts2.Add("EB21-短路瞬动", myobj.EB21_Ir3);
                            atts2.Add("EB22-互感器规格", myobj.EB22_CT);
                            atts2.Add("EB23-通讯接口", myobj.EB21_Ir3);
                            atts2.Add("EB42-母线槽规格", myobj.EB42_BUSBAR);

                            //待补充:联络柜对象
                            //插入块
                            var objectId = tr.CurrentSpace.InsertBlock(ptDiYagui, BLK_LDC, InsertScale, atts: atts2);
                            //IFOXCad插入块时仅支持设置插入点、块名、比例、旋转、块属性
                            //获取ent，用于设置其他属性：图层
                            var ent = objectId.GetObject<Entity>();
                            ent.Layer = LayerInsert;


                        }
                        //7-5馈电柜
                        else if (purpose == list_purpose_pub[4])
                        {
                            #region 插入馈电柜


                            //查询第i个馈电柜的回路
                            var listHuiLu = (from d in rows_Form
                                             where d.IdHuilu.StartsWith(memo.ID)
                                             select d).ToList();

                            //数据源rows_Form
                            //如果馈电柜回路数量为0，不进行生成操作
                            if (rows_Form.Count() == 0 || listHuiLu.Count == 0) return;

                            //获取低压柜的抽屉数量
                            int num_DiYagui = listHuiLu.Count();

                            int color = 0; //0为随层
                            Point3d[] ptHuiLu = new Point3d[3];
                            string name_BlkMokuai1 = "模块-FAS"; //切非模块名
                            string name_BlkMokuai2 = "模块-电气火灾监控";  //电气火灾监控模块名

                            //插入馈出柜
                            Point3d ptChouti = ptDiYagui.PolarPoint(Math.PI / 2, -4000); //根据低压柜位置确定抽屉的插入点
                            Dictionary<string, string> atts2 = new Dictionary<string, string>
                            {
                                {"低压柜功能", "馈出柜" },
                                {"低压柜尺寸","800(W)X1000(D)X2200(H)" },
                                {"低压柜类型","MNS" },
                                {"低压柜编号",memo.ID},
                                {"剩余电流监控仪表","HS-L820P"}

                            };
                            //插入块
                            var objectId = tr.CurrentSpace.InsertBlock(ptDiYagui, "DYPD-低压馈出柜-带剩余电流", InsertScale, atts: atts2);
                            //IFOXCad插入块时仅支持设置插入点、块名、比例、旋转、块属性
                            //获取ent，用于设置其他属性：图层
                            var ent = objectId.GetObject<Entity>();
                            ent.Layer = LayerInsert;

                            //待补充：根据该柜内是否剩余电流监控,决定柜内是否按照对应仪表
                            //ARXTools.UpdateDynInBlock(trans, objectId, "剩余电流监控", "设置剩余电流监控");

                            //循环插入抽屉
                            for (int j = 0; j < listHuiLu.Count; j++)
                            {
                                //根据Ir1查询断路器相关参数
                                //从数据库中查询相关信息并转换为对应类
                                var queryFromSQL = new IFoxDYPD.SQL.QueryFromSQL();
                                List<CB_CLASS> CB_LIST = queryFromSQL.query_CBClass(conn, Convert.ToInt32(listHuiLu[j].Ir1));

                                //仅当有返回值时执行
                                if (CB_LIST.Count == 1)
                                {
                                    CB_CLASS CB_INFOS = CB_LIST[0];

                                    string blkName = "";
                                    int distanceblkPt = 2500;
                                    if (int.Parse(listHuiLu[j].Ir1) >= 16)
                                    {

                                        //根据抽屉高度确定插入的块名
                                        switch (CB_INFOS.HEIGHT)
                                        {
                                            case (200):
                                                blkName = "模块200";  //抽屉的块名
                                                ptHuiLu[2] = ptChouti.PolarPoint(0, 4350).PolarPoint(Math.PI / 2, -1625);  //漏电检测的块名
                                                ptHuiLu[1] = ptChouti.PolarPoint(0, 1580).PolarPoint(Math.PI / 2, -1850);  //切非模块的块名
                                                break;
                                            case (400):
                                                blkName = "模块400";
                                                distanceblkPt = 5000;
                                                ptHuiLu[2] = ptChouti.PolarPoint(0, 4350).PolarPoint(Math.PI / 2, -2500);
                                                ptHuiLu[1] = ptChouti.PolarPoint(0, 1580).PolarPoint(Math.PI / 2, -2725);
                                                break;
                                            case (600):
                                                blkName = "模块600";
                                                distanceblkPt = 7500;
                                                ptHuiLu[2] = ptChouti.PolarPoint(0, 4350).PolarPoint(Math.PI / 2, -3750);
                                                ptHuiLu[1] = ptChouti.PolarPoint(0, 1580).PolarPoint(Math.PI / 2, -3975);
                                                break;
                                            case (800):
                                                blkName = "模块800";
                                                distanceblkPt = 10000;
                                                ptHuiLu[2] = ptChouti.PolarPoint(0, 4350).PolarPoint(Math.PI / 2, -5000);
                                                ptHuiLu[1] = ptChouti.PolarPoint(0, 1580).PolarPoint(Math.PI / 2, -5225);

                                                break;
                                            case (1800):
                                                blkName = "模块1800";
                                                distanceblkPt = 22500;
                                                ptHuiLu[2] = ptChouti.PolarPoint(0, 4350).PolarPoint(Math.PI / 2, -11250);
                                                ptHuiLu[1] = ptChouti.PolarPoint(0, 1580).PolarPoint(Math.PI / 2, -11475);
                                                //name_BlkMokuai1 = "模块-FAS2";
                                                //name_BlkMokuai2 = "模块-电气火灾监控2";
                                                break;
                                        } //end of switch (CB_INFOS.Height)

                                        //插入图块
                                        Dictionary<string, string> atts = new Dictionary<string, string>();
                                        //回路编号
                                        if (!string.IsNullOrWhiteSpace(listHuiLu[j].IdHuilu)) atts.Add("回路编号", listHuiLu[j].IdHuilu);
                                        //额定功率
                                        if ((!string.IsNullOrWhiteSpace(listHuiLu[j].Pe)) && (listHuiLu[j].Pe != "0"))
                                        { atts.Add("额定功率", listHuiLu[j].Pe + "kW"); }
                                        else
                                        //231226新增默认空
                                        {
                                            atts.Add("额定功率", "");
                                        }
                                        //计算电流
                                        if ((!string.IsNullOrWhiteSpace(listHuiLu[j].Ijs)) && (listHuiLu[j].Ijs != "0"))
                                        { atts.Add("计算电流", "Ijs=" + decimal.Round(decimal.Parse(listHuiLu[j].Ijs), 2) + "A"); }
                                        else
                                        { atts.Add("计算电流", ""); }
                                        //常备用 0-备用 1-常用 2-单回路(省略)
                                        if (!string.IsNullOrWhiteSpace(listHuiLu[j].NorS))
                                        {
                                            switch (listHuiLu[j].NorS)
                                            {
                                                case "0": atts.Add("常备用", "(备用)"); break;
                                                case "1": atts.Add("常备用", "(常用)"); break;
                                                case "2": atts.Add("常备用", ""); break;
                                            }
                                        }
                                        //用电设备描述
                                        if (!string.IsNullOrWhiteSpace(listHuiLu[j].Purpose)) atts.Add("用电设备", listHuiLu[j].Purpose);
                                        else { atts.Add("用电设备", ""); }
                                        //箱名
                                        if (!string.IsNullOrWhiteSpace(listHuiLu[j].Name)) atts.Add("配电箱名称", listHuiLu[j].Name);
                                        else { atts.Add("配电箱名称", ""); }
                                        //插入消防、切非、电气火灾监控模块
                                        #region "模块"
                                        int[] info1 = new int[3];
                                        if (string.IsNullOrWhiteSpace(listHuiLu[j].Info1) || listHuiLu[j].Info1.Length != 3)  //当单元格为空时,设置默认值
                                        {

                                            //关键字:消防、应急,置100
                                            if ((listHuiLu[j].Purpose.Contains("消防")
                                                || listHuiLu[j].Purpose.Contains("应急")
                                                || listHuiLu[j].Purpose.Contains("变电所"))
                                                &&
                                                !listHuiLu[j].Purpose.Contains("非消防")

                                                )
                                            {
                                                info1[0] = 1;  //消防(断路器是否加MA)
                                                info1[1] = 0;  //切非(断路器是否加切非模块)
                                                info1[2] = 0;  //漏电(是否设置电气火灾监控模块)
                                            }
                                            //关键字:备用且常备用数字不合法,置000
                                            else if (listHuiLu[j].Purpose.Contains("备用")
                                                && String.IsNullOrEmpty(listHuiLu[j].Pe)
                                                &&
                                                !((info1[0] == 0 || info1[0] == 1)
                                                && (info1[1] == 0 || info1[1] == 1)
                                                && (info1[2] == 0 || info1[2] == 1)
                                                )
                                                )
                                            {
                                                info1[0] = 0;  //消防(断路器是否加MA)
                                                info1[1] = 0;  //切非(断路器是否加切非模块)
                                                info1[2] = 0;  //漏电(是否设置电气火灾监控模块)
                                            }
                                            //关键字:电力、动力、空调、电梯,置011
                                            else if (listHuiLu[j].Purpose.Contains("电力") ||
                                                        listHuiLu[j].Purpose.Contains("风机") ||
                                                        listHuiLu[j].Purpose.Contains("动力") ||
                                                        listHuiLu[j].Purpose.Contains("空调") ||
                                                        listHuiLu[j].Purpose.Contains("电梯") ||
                                                        listHuiLu[j].Purpose.Contains("景观照明") ||
                                                        listHuiLu[j].Purpose.Contains("泛光照明"))
                                            {
                                                info1[0] = 0;  //消防(断路器是否加MA)
                                                info1[1] = 1;  //切非(断路器是否加切非模块)
                                                info1[2] = 1;  //漏电(是否设置电气火灾监控模块)
                                            }
                                            //关键字:照明,置001
                                            else if (listHuiLu[j].Purpose.Contains("照明"))
                                            {
                                                info1[0] = 0;  //消防(断路器是否加MA)
                                                info1[1] = 0;  //切非(断路器是否加切非模块)
                                                info1[2] = 1;  //漏电(是否设置电气火灾监控模块)
                                            }

                                            //其他情况,置000
                                            else
                                            {
                                                info1[0] = 0;  //消防(断路器是否加MA)
                                                info1[1] = 0;  //切非(断路器是否加切非模块)
                                                info1[2] = 0;  //漏电(是否设置电气火灾监控模块)
                                            }
                                        }
                                        else //当输入有效时
                                        {

                                            info1[0] = int.Parse(listHuiLu[j].Info1.Substring(0, 1));  //消防(断路器是否加MA)
                                            info1[1] = int.Parse(listHuiLu[j].Info1.Substring(1, 1));  //切非(断路器是否加切非模块)
                                            info1[2] = int.Parse(listHuiLu[j].Info1.Substring(2, 1));  //漏电(是否设置电气火灾监控模块)
                                        }

                                        //插入对应计量、电气火灾监控模块
                                        //info1[0]跳过
                                        if (info1[1] == 1)
                                        {
                                            //插入块
                                            var objectIdFujian = tr.CurrentSpace.InsertBlock(ptHuiLu[1], name_BlkMokuai1, InsertScale);
                                            //IFOXCad插入块时仅支持设置插入点、块名、比例、旋转、块属性
                                            //获取ent，用于设置其他属性：图层
                                            var entChoutiFujian = objectId.GetObject<Entity>();
                                            entChoutiFujian.Layer = LayerChouti;
                                        }
                                        if (info1[2] == 1)
                                        {
                                            //插入块
                                            var objectIdFujian = tr.CurrentSpace.InsertBlock(ptHuiLu[2], name_BlkMokuai2, InsertScale);
                                            //IFOXCad插入块时仅支持设置插入点、块名、比例、旋转、块属性
                                            //获取ent，用于设置其他属性：图层
                                            var entChoutiFujian = objectId.GetObject<Entity>();
                                            entChoutiFujian.Layer = LayerChouti;
                                        }

                                        //断路器规格
                                        //函数功能待补充:判断是否为消防
                                        //当前仅按非消防处理,未加入MA
                                        //if (!string.IsNullOrWhiteSpace(listHuiLu[j].Ir1)) 
                                        //{ 
                                        //    atts.Add("断路器规格",  CB_INFOS.Switch1 +  CB_INFOS.Switch2 + listHuiLu[j].Ir1); 
                                        //}
                                        //额定电流
                                        if ((!string.IsNullOrWhiteSpace(listHuiLu[j].Ir1)) && (CB_INFOS.IR1 != 0))
                                        {
                                            if (info1[0] == 1)
                                            {
                                                color = 1;
                                                atts.Add("断路器规格", CB_INFOS.CB_INFO1 + CB_INFOS.CB_INFO3);
                                            }
                                            else if (info1[1] == 0)
                                            {
                                                atts.Add("断路器规格", CB_INFOS.CB_INFO1 + CB_INFOS.CB_INFO2);
                                            }
                                            else if (info1[1] == 1)
                                            {
                                                atts.Add("断路器规格", CB_INFOS.CB_INFO1 + CB_INFOS.CB_INFO2 + "+MX+OF");
                                            }

                                            if (CB_INFOS.IN_40 != 0) { atts.Add("额定电流", "In=" + CB_INFOS.IN_40 + "A"); }
                                            //电流互感器
                                            if (!string.IsNullOrWhiteSpace(CB_INFOS.CT)) { atts.Add("互感器规格", CB_INFOS.CT); }

                                            //区分框架断路器与塑壳
                                            //塑壳默认不设置短延时Ir2
                                            if (CB_INFOS.HEIGHT < 800)
                                            {

                                                //消防负荷无Ir1
                                                if (info1[0] == 1)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(CB_INFOS.IR3)) { atts.Add("长延时/短路瞬动", "Ir3=" + CB_INFOS.IR3); }
                                                }

                                                //一般负荷添加Ir1
                                                if (info1[0] == 0)
                                                {
                                                    atts.Add("长延时/短路瞬动", "Ir1=" + CB_INFOS.IR1 + "A");
                                                    if (!string.IsNullOrWhiteSpace(CB_INFOS.IR3)) { atts.Add("短路瞬动", "Ir3=" + CB_INFOS.IR3); }
                                                }


                                            }
                                            else if (CB_INFOS.HEIGHT >= 800) //框架断路器加设Ir2
                                            {
                                                if (CB_INFOS.IR1 != 0) { atts.Add("长延时整定电流", "Ir1=" + CB_INFOS.IR1 + "A"); }  //Ir1
                                                if (!string.IsNullOrWhiteSpace(CB_INFOS.IR2)) { atts.Add("短延时整定电流", "Ir2=" + CB_INFOS.IR2 + "A"); }  //Ir2
                                                if (!string.IsNullOrWhiteSpace(CB_INFOS.IR3)) { atts.Add("短路瞬动", "Ir3=" + CB_INFOS.IR3); }       //Ir3
                                            }
                                        } //end of if 额定电流

                                        //电缆
                                        if (!string.IsNullOrWhiteSpace(listHuiLu[j].CableType + listHuiLu[j].CableCSA)) atts.Add("电缆", (listHuiLu[j].CableType + listHuiLu[j].CableCSA));
                                        //231226新增:当电缆内容为空时，插入值也为空
                                        else
                                        {
                                            atts.Add("电缆", "");
                                        } //end of 231226新增
                                        if (!string.IsNullOrWhiteSpace(blkName))
                                        {
                                            //插入块
                                            var objectIdChouti = tr.CurrentSpace.InsertBlock(ptChouti, blkName, InsertScale, atts: atts);
                                            //IFOXCad插入块时仅支持设置插入点、块名、比例、旋转、块属性
                                            //获取ent，用于设置其他属性：图层
                                            var entChouti = objectId.GetObject<Entity>();
                                            entChouti.Layer = LayerChouti;
                                        }
                                        ptChouti = ptChouti.PolarPoint(Math.PI / 2, -distanceblkPt); //指向下一行
                                        #endregion

                                    } //end of if (int.Parse(listHuiLu[j].Ir1) >= 16)
                                } // end of for (int j = 0; j < listHuiLu.Count; j++)
                            } // end of for (int j = 0; j < listHuiLu.Count; j++)


                            #endregion
                        }

                        //当前低压柜插入完成后，为下一个柜子设置插入点
                        ptDiYagui = ptDiYagui.PolarPoint(0, diastance_polar);
                    } //end of for(int i=0;i<MemoDtos_Now.Count;i++)

                }






            } //end of if (Excelinfos.GuiHao_TA.Count > 0)
            IsWaiting = "Hidden";

        }


        // 命令逻辑的实现
        private void DoSomething()
        {

            // 在按钮点击时输出当前 MemoDtos 的排序，或者进行其他逻辑
            var sortedItems = string.Join(", ", MemoDtos_TA.Select(m => m.ID));
            Tsetstring = $"当前排序: {sortedItems}";  // 更新UI显示排序
        }



    }
}
