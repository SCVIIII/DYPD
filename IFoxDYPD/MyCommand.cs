using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using IFoxCAD.Cad;
using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using System.Reflection;

using Exception = Autodesk.AutoCAD.Runtime.Exception;
using System.Windows.Interop;
using IFoxDYPD.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IFoxDYPD.Dtos;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Mysqlx.Crud;


namespace IFoxDYPD
{
    public class MyCommand : IExtensionApplication
    {
        private void AssemblyDLL()
        {
            // 加载 HandyControl.dll
            var assembly2 = Assembly.Load("MaterialDesignColors");
            var assembly3 = Assembly.Load("MaterialDesignThemes.Wpf");
            var assembly5 = Assembly.Load("Microsoft.Xaml.Behaviors");
        }

        private string Method_queryCircuitBreaker(List<XTTCircuitBreakerDto> xTTSwitchDtos, int In)
        {
            //结合断路器的In和品牌选择断路器型号
            var CircuitBreaker = xTTSwitchDtos.Where(d => d.In == In)
                .Select(d => d.CircuitBreaker)
                .ToList().FirstOrDefault();
            return CircuitBreaker;
        }

        [CommandMethod(nameof(DYPD))]
        public void DYPD()
        {
            // 文字提示
            // Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("WPF窗口将要显示。\n");
            try
            {
                // 加载必要的DLL
                AssemblyDLL();
                // 创建并显示 WPF 窗口
                WPF.DYPDView mainWindow = new();
                // 绑定 ViewModel
                mainWindow.DataContext = new WPF.DYPDViewModel();

                // 获取当前 AutoCAD 窗口的句柄
                Document doc = Application.DocumentManager.MdiActiveDocument;
                IntPtr autocadHandle = doc.Window.Handle;

                // 使用 WindowInteropHelper 设置 WPF 窗体的所有者为 AutoCAD 窗口
                WindowInteropHelper helper = new WindowInteropHelper(mainWindow);
                helper.Owner = autocadHandle;

                // 显示 WPF 窗口为模态窗口
                mainWindow.ShowDialog();

            }
            catch (Exception ex)
            {
                // 捕获加载 DLL 的异常
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"加载程序时出错: {ex.Message}\n");
            }
        }

        //public ObservableCollection<XTTHuiluDto> XTTHuilus;
        [CommandMethod(nameof(XTT))]
        public void XTT()
        {
            //存放常规回路
            List<XTTHuiluDto> XTTHuilus = new List<XTTHuiluDto>();
            //存放风机水泵的属性块回路
            List<XTTACDto> XTTACHuilus = new List<XTTACDto>();

            //待初始化的全局/绑定变量

            //try
            //{
            //从平面选取回路编号
            using (DBTrans tr = new DBTrans())
            {
                Editor ed = tr.Editor;
                //文字+风机/水泵块的筛选条件
                var actypedValues = new TypedValue[]
                {
                new TypedValue((int)DxfCode.Operator, "<or"),
                    new TypedValue((int)DxfCode.Operator, "<and"),
                         new TypedValue((int)DxfCode.Start, "TEXT"),
                         new TypedValue((int)DxfCode.LayerName, "0-回路文字"),
                    new TypedValue((int)DxfCode.Operator, "and>"),

                    new TypedValue((int)DxfCode.Operator, "<and"),
                         new TypedValue((int)DxfCode.Start, "INSERT"),
                         new TypedValue((int)DxfCode.BlockName, "平面标注"),
                         new TypedValue((int)DxfCode.LayerName, "0-动力平面标注"),
                    new TypedValue((int)DxfCode.Operator, "and>"),
                new TypedValue((int)DxfCode.Operator, "or>"),
                };
                SelectionFilter acSelFtr_HuiLu = new SelectionFilter(actypedValues);

                // Request for objects to be selected in the drawing area
                PromptSelectionResult acSSPrompt_pl = ed.GetSelection(acSelFtr_HuiLu);

                //框选平面
                if (acSSPrompt_pl.Status != PromptStatus.OK) return; //当选中实体无效时，退出程序

                int num_acSSPrompt = acSSPrompt_pl.Value.Count;         //防火分区数量
                ObjectId[] id_ss = acSSPrompt_pl.Value.GetObjectIds();  //将防火分区对应的pline线ObjectId存入数组id_ss'

                for (int i = 0; i < id_ss.Length; i++)
                {
                    //2024-11-25新增:先判断图元是文字or块
                    var ent = id_ss[i].GetObject<Entity>();

                    #region 单行文字的筛选与处理
                    if(ent is DBText)
                    {
                        DBText dBText = id_ss[i].GetObject(OpenMode.ForRead) as DBText;  //当前pline信息
                        string textContent = dBText.TextString; // 获取文本内容

                        #region CAD中读到的回路编号进行数据分拆处理
                        var xttPMHuilus = Common.HuiIsValidTools.IsVaildHuiluInput(textContent);
                        if (xttPMHuilus.Count > 0)
                        //if (HuiIsValidTools.IsValidFormat(textContent, out XTTHuiLuIsValidDto xttHuiLuIsValid))
                        {
                            for (int j = 0; j < xttPMHuilus.Count; j++)
                            {
                                var xttHuiLuIsValid = xttPMHuilus[j];
                                var huiluDto = new XTTHuiluDto();
                                huiluDto.Name = xttHuiLuIsValid.Name;
                                huiluDto.Pe = xttHuiLuIsValid.Pe;
                                huiluDto.CircuitNumber = xttHuiLuIsValid.CircuitNumber;
                                //校验是否有子配电箱
                                if (!string.IsNullOrWhiteSpace(xttHuiLuIsValid.PDXName))
                                {
                                    huiluDto.Purpose = xttHuiLuIsValid.PDXName;
                                }
                                XTTHuilus.Add(huiluDto);
                            }

                        }
                        else
                        {
                            ed.WriteMessage($"\n字符串不符合规则:{textContent}");
                        }
                        #endregion
                    }



                    #endregion 单行文字的筛选与处理

                    else if(ent is BlockReference)
                    {
                        //获取风机块的块属性
                        var id = id_ss[i];
                        var acHuilu = new XTTACDto();
                        var attributes = new Dictionary<string, Action<string>>
                        {
                            { "设备类型", value => acHuilu.Purpose = value },
                            { "常备用信息", value => acHuilu.ChangBeiYong = value },
                            { "控制箱名称", value => acHuilu.ACPDXName = value },
                            { "回路编号1", value => acHuilu.StrNum1 = value },
                            { "回路编号2", value => acHuilu.StrNum2 = value },
                            { "功率", value =>
                                {
                                    if (double.TryParse(value, out double pe))
                                    {
                                        acHuilu.Pe = pe;
                                    }
                                }
                            }
                        };

                        foreach (var attribute in attributes)
                        {
                            var value = id.IFoxGetAttributeInBlockReference(tr, attribute.Key);
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                attribute.Value(value);  // 根据属性名执行对应的赋值操作
                            }
                        }

                        //对已获取的内容进行分析处理
                        //[1]回路编号1,2 => 所属箱名+回路号
                        //[2]Pe>0且落在预设的功率区间内
                        //[3]常备用信息为空时 => Mode置为"一用"

                        var strreturn = GetXTTFengjiHuilu(acHuilu.StrNum1);
                        if (strreturn != null)
                        {
                            if (!string.IsNullOrWhiteSpace(strreturn[0])) { acHuilu.Name1 = strreturn[0]; }
                            if (!string.IsNullOrWhiteSpace(strreturn[1])) { acHuilu.CircuitNumber1 = strreturn[1]; }
                        }
                        strreturn = GetXTTFengjiHuilu(acHuilu.StrNum2);
                        if (strreturn != null)
                        {
                            if (!string.IsNullOrWhiteSpace(strreturn[0])) { acHuilu.Name2 = strreturn[0]; }
                            if (!string.IsNullOrWhiteSpace(strreturn[1])) { acHuilu.CircuitNumber2 = strreturn[1]; }
                        }

                        //当主回路的所属配电箱、回路编号均不为空时
                        //将风机块信息=>添加至风机水泵list +
                        //             添加至所属的配电箱配出回路
                        if(!string.IsNullOrWhiteSpace(acHuilu.Name1) && !string.IsNullOrWhiteSpace(acHuilu.CircuitNumber1))
                        {
                            //风机水泵列表中添加当前属性块内容
                            XTTACHuilus.Add(acHuilu);
                            //尝试将此属性块插入到所属配电箱中
                            var xttHuilui = new XTTHuiluDto()
                            {
                                Name = acHuilu.Name1,
                                Purpose= acHuilu.Purpose,
                                Pe= acHuilu.Pe,
                                L123="L123",
                                Cos=0.5,
                                BlkName= "系统图开关-动力",
                                CircuitBreaker_Mode=1,
                                Phase="3P",
                                HuiluStatus=HuiluOptions.风机水泵控制箱d,

                            };
                            XTTHuilus.Add(xttHuilui);

                        }

                    }
                }


                //2024年11月26日新增
                //将平面框选得到的风机回路,存入XTTHuilus中
                for (int i = 0; i < XTTACHuilus.Count; i++)
                {
                    var xttACHuilui = XTTACHuilus[i];
                    var xttHuilui = new XTTHuiluDto();

                }


                //调试断点
                ed.WriteMessage("调试断点");

            } // end of using

            #region XTT WPF测试
            //测试插入点
            //显示WPF 并将参数传递给WPF


            // 加载必要的DLL
            AssemblyDLL();
            // 创建并显示 WPF 窗口
            WPF.XTTView mainWindow = new WPF.XTTView(XTTHuilus);
            // 绑定 ViewModel
            mainWindow.DataContext = new WPF.XTTViewModel(XTTHuilus,XTTACHuilus);

            // 获取当前 AutoCAD 窗口的句柄
            //Document doc = tr.Document;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            IntPtr autocadHandle = doc.Window.Handle;

            // 使用 WindowInteropHelper 设置 WPF 窗体的所有者为 AutoCAD 窗口
            WindowInteropHelper helper = new WindowInteropHelper(mainWindow);
            helper.Owner = autocadHandle;

            // 显示 WPF 窗口为模态窗口
            mainWindow.ShowDialog();


            //测试结束的
            #endregion
            //} //END OF TRY
            //catch (Exception ex)
            //{
            //    // 捕获加载 DLL 的异常
            //    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"平面框选时出错: {ex.Message}\n");
            //}

        } //end of XTT

        private static string[] GetXTTFengjiHuilu(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) 
            { 
                return null;
            }
            string[] output = new string[2];
            //正则表达式判断是否有"/"
            //匹配正反斜线
            string pattern1 = @"^(.+)/(.+)$";
            string pattern2 = @"^(.+)\\(.+)$";
            Match match1 = Regex.Match(input, pattern1);
            Match match2 = Regex.Match(input, pattern2);
            //只含有一个冒号时
            if (match1.Success)
            {
                //尝试分割字符串
                string[] parts = input.Split('/');
                output[0] = parts[0];  // ':'前的部分(所属配电箱名称)
                output[1] = parts[1];  // ':'后的部分(回路编号)
            } //end of if (match1.Success || match2.Success)
            else if (match2.Success)
            {
                //尝试分割字符串
                string[] parts = input.Split('\\');
                output[0] = parts[0];  // ':'前的部分(所属配电箱名称)
                output[1] = parts[1];  // ':'后的部分(回路编号)
            } //end of if (match1.Success || match2.Success)
            return output;
        }


        public void Initialize()
        {

        }

        public void Terminate()
        {

        }




    }
}
