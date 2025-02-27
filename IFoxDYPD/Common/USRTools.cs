using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using static Autodesk.AutoCAD.LayerManager.LayerFilter;
using MiniExcelLibs;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
using Microsoft.Win32;
using IFoxDYPD.Dtos;


namespace IFoxDYPD.Common
{

    public static class USRTools
    {
        /// <summary>
        /// 获取用户插入点,需在using trans中使用
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Point3d get_POINT3D(Document doc)
        {
            //获取用户输入点
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("")
            {
                Message = "\n选择低压柜的插入点: "
            };
            pPtRes = doc.Editor.GetPoint(pPtOpts);
            Point3d ptReturn = pPtRes.Value;
            return ptReturn;
        }




        /// <summary>
        /// 功能函数:导入计算书EXCEL文件，获取变压器数量、馈电柜数量
        /// </summary>
        /// <param name="db">需要将db传入，用于打开DWG文件所在的文件夹.</param>
        /// <param name="Excelinfos">主程序中定义的变量，在此函数中修改变量，进而在WPF中显示低压柜磁贴.</param>
        /// <returns></returns>
        public static List<ExcelJiSuanshuHuiLuDto> ImportDataFromExcel(this Database db, ExcelDto Excelinfos)
        {
            
            //返回值
            var rows_Form = new List<ExcelJiSuanshuHuiLuDto>();  //DatagridView表格的数据源
            try
            {
                //开始导入计算书,打开地址选择的对话框
                OpenFileDialog openfiledialog = new OpenFileDialog
                {
                    //InitialDirectory = Path.GetDirectoryName(db.OriginalFileName),  //默认位置,为当前dwg的所在地址
                    InitialDirectory = Path.GetDirectoryName(@"C:\Users\sunzm\Desktop\新建文件夹\"),  //默认位置,为当前dwg的所在地址

                    //C:\Users\sunzm\Desktop
                    //InitialDirectory = @"C:\Users\Mr.Sun-Work\Desktop\新建文件夹", //测试用的默认位置
                    Title = "选择计算书",  //窗体标题
                    Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*", //过滤条件
                    Multiselect = false //是否多选
                };

                // 直接设置要打开的文件
                openfiledialog.FileName = "海南二号食堂负荷计算书";

                //如果文件选择框正常生成,则开始此函数
                if (openfiledialog.ShowDialog() == true)
                {
                    string fileName = openfiledialog.FileName;  //选择计算书
                    using (FileStream stream_Jisuanshu = File.OpenRead(fileName))  //根据计算书开始数据处理
                                                                                   //using (FileStream stream_Jisuanshu = File.OpenRead(@"C:\Users\Mr.Sun-Work\Desktop\新建文件夹\商业1#计算书.xlsx"))  //根据计算书开始数据处理
                    {

                        List<string> sheetNames = MiniExcel.GetSheetNames(fileName);   //查询所有 Sheet 名称
                        
                        var rows_Jinxian = new List<ExcelJiSuanshuHuiLuDto>();  //存放关于进线补偿柜的辅助信息
                        foreach (var sheetName in sheetNames)  //对于表内有多个表格时
                        {
                            //名为sheetName的页读取到的全部内容
                            var rows_Jisuanshu1 = stream_Jisuanshu.Query<ExcelJiSuanshuHuiLuDto>(sheetName: sheetName);
                            //对名为sheetName的页 筛选后的内容  using System.Linq;
                            var rows_Jisuanshu2 = (from d in rows_Jisuanshu1
                                                   where
                                                       (!string.IsNullOrWhiteSpace(d.IdHuilu))
                                                       &&
                                                       ((Char.IsLetter(d.IdHuilu[0]) || Char.IsDigit(d.IdHuilu[0])))
                                                       &&
                                                       (int)d.IdHuilu[0] <= 127

                                                   orderby d.IdHuilu ascending
                                                   orderby d.IdHuilu.Length ascending
                                                   select d).ToList();

                            //当前list元素数量>0时进行数据处理
                            if (rows_Jisuanshu2.Count > 0)
                            {

                                foreach (var n in rows_Jisuanshu2)
                                {
                                    n.Info_GuiHao = n.IdHuilu.Substring(0, n.IdHuilu.Length - 1); //设置柜号
                                    n.Info_SheetName = sheetName;  //设置所在页面的名称
                                } //end of foreach

                                rows_Form.AddRange(rows_Jisuanshu2);   //将有效数据存入,供表格输出
                                rows_Jinxian.Add(rows_Jisuanshu2[0]);  //将当前页的第一行存入
                                Excelinfos.SheetNames.Add(sheetName);  //显示Excel的sheet名称
                                Excelinfos.Num_transformer++;          //变压器数量
                            } //end of if (rows_Jisuanshu2.Count > 0)
                        } //end of foreach

                        

                        if ((Excelinfos.Num_transformer > 0) && Excelinfos.Num_transformer < 3)  //单个EXCEL文件内最多设有两台变压器
                        {
                            //将数据表格依次按回路编号排序 
                            rows_Form = (from d in rows_Form
                                         orderby d.IdHuilu
                                         orderby d.IdHuilu.Length
                                         orderby d.IdHuilu.Substring(0, 2)
                                         select d).ToList();

                            //循环函数，将每台变压器的馈电柜信息存入Dto
                            for (int i = 0; i < Excelinfos.Num_transformer; i++)
                            {
                                //从checkedListBox1获取排序好的sheetname
                                var list_Guihao_i = (from d in rows_Form
                                              where d.Info_SheetName == Excelinfos.SheetNames[i]
                                              orderby d.Info_GuiHao ascending
                                              orderby d.IdHuilu.Length ascending
                                              select d.Info_GuiHao).Distinct().ToList();

                                //筛选sheetName对应的第一个馈电柜柜号
                                string FirstGuiHao = list_Guihao_i.First();
                                string pre = GetCommonPrefix(list_Guihao_i);
                                int nth = 0;
                                if ((pre.Length < FirstGuiHao.Length)&& int.TryParse(FirstGuiHao[pre.Length].ToString(),out nth) )
                                {
                                    
                                }
                                    
                                switch (i)
                                {
                                    case 0: 
                                        Excelinfos.First_GuiHao_TA = FirstGuiHao;
                                        Excelinfos.GuiHao_TA = list_Guihao_i;
                                        Excelinfos.Num_KuiDianGui_TA= Excelinfos.GuiHao_TA.Count();
                                        Excelinfos.Pre_TA = pre;
                                        Excelinfos.Nth_FirstKuiChuGui_TA = nth;
                                        break;

                                    case 1: 
                                        Excelinfos.First_GuiHao_TB = FirstGuiHao;
                                        Excelinfos.GuiHao_TB = list_Guihao_i;
                                        Excelinfos.Num_KuiDianGui_TB = Excelinfos.GuiHao_TB.Count();
                                        Excelinfos.Pre_TB = pre;
                                        Excelinfos.Nth_FirstKuiChuGui_TB = nth;
                                        break;
                                }
                            } //end of for for (int i = 0; i < Excelinfos.Num_transformer; i++)

                           

                            if (true)
                            {

                            }

                            //    //添加限定程序,仅当有合法的柜号输入时,继续程序
                            //    if (list_GuiHao.Count > 0)
                            //    {
                            //        //测试程序：从rows_Form中提取柜号

                            //        //初始化TA,TB对应的功能柜数量
                            //        int NUM_GUIHAO_TA = 0;
                            //        int NUM_GUIHAO_TB = 0;

                            //        //先将TA的功能柜号显示在textBox_num_TA中
                            //        if (int.TryParse(list_GuiHao[0].Substring(list_GuiHao[0].Length - 1, 1), out NUM_GUIHAO_TA))
                            //        {

                            //            //记录功能柜（进线、联络、补偿柜）数量
                            //            //截断后取得的NUM_GUIHAO_TA为馈电柜编号,因此减1后,即为功能柜数量
                            //            NUM_GUIHAO_TA--;
                            //            //功能柜有效数量为1~6
                            //            if (1 <= NUM_GUIHAO_TA && NUM_GUIHAO_TA <= 6)
                            //            {
                            //                //功能柜数量
                            //                textBox_num_TA.Text = (NUM_GUIHAO_TA).ToString();
                            //                //TA低压柜前缀
                            //                textBox_PRE_TA.Text = list_GuiHao[0].Substring(0, list_GuiHao[0].Length - 1);
                            //            } //end of if (1 <= NUM_GUIHAO_TA && NUM_GUIHAO_TA <= 6)
                            //            else
                            //            {
                            //                //MessageBox.Show("TA功能柜数量不在1~6之间，请检查输入");
                            //            } //end of else (1 <= NUM_GUIHAO_TA && NUM_GUIHAO_TA <= 6)

                            //        } //end of if (int.TryParse(list_GuiHao[0].Substring(list_GuiHao[0].Length - 1, 1), out NUM_GUIHAO_TA))



                            //        //当变压器数量为2时，加入TB的变量
                            //        //num_SheetNames为经过过滤后的变压器数量
                            //        if (num_SheetNames == 2 && list_GuiHao.Count == 2)
                            //        {
                            //            if (int.TryParse(list_GuiHao[1].Substring(list_GuiHao[1].Length - 1, 1), out NUM_GUIHAO_TB))
                            //            {

                            //                //记录功能柜（进线、联络、补偿柜）数量
                            //                //截断后取得的NUM_GUIHAO_TB为馈电柜编号,因此减1后,即为功能柜数量
                            //                NUM_GUIHAO_TB--;
                            //                //功能柜有效数量为1~6
                            //                if (1 <= NUM_GUIHAO_TB && NUM_GUIHAO_TB <= 6)
                            //                {
                            //                    //功能柜数量
                            //                    textBox_num_TB.Text = (NUM_GUIHAO_TB).ToString();
                            //                    //TB低压柜前缀
                            //                    textBox_PRE_TB.Text = list_GuiHao[1].Substring(0, list_GuiHao[1].Length - 1);
                            //                }
                            //            }
                            //        } //end of if (num_SheetNames == 2 && list_GuiHao.Count == 2)



                            //        //测试结束
                            //    } //end of if(list_GuiHao.Count>0)
                            //} //end of if ((num_SheetNames > 0) && num_SheetNames < 3)


                            ////当变压器数量有误时,弹窗提示
                            //else
                            //{
                            //    //MessageBox.Show("请检查计算书的Sheet数量是否正确");
                            //} //end of else ((num_SheetNames > 0) && num_SheetNames < 3)

                        } //end of using
                    } //end of if
                }

                
            }
            catch (Exception ex) 
            { 

            }
            //end of public void import_Excel()
            return rows_Form;
        }

        /// <summary>
        /// 为低压配电程序,由外部CAD模板导入标准图块
        /// 返回值Bool,导入成功时返回true
        /// </summary>
        /// <param name="destDb"></param>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        public static bool ImportBlocksFromDwg_DYPD(Database destDb)
        {
            //返回值
            bool result = false;
            string sourceFileName = @"D:\Mycode\BlkDYPD\BlkDYPD_t3.dwg";

            //string sourceFileName= @"D:\Mycode\BlkDYPD\BlkDYPD.dwg";
            //创建一个新的数据库对象，作为源数据库，以读入外部文件中的对象
            Database sourceDb = new Database(false, true);
            try
            {
                //把DWG文件读入到一个临时的数据库中
                sourceDb.ReadDwgFile(sourceFileName, System.IO.FileShare.Read, true, null);
                //创建一个变量用来存储块的ObjectId列表
                ObjectIdCollection blockIds = new ObjectIdCollection();
                //获取源数据库的事务处理管理器
                Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = sourceDb.TransactionManager;
                //在源数据库中开始事务处理
                using (Transaction myT = tm.StartTransaction())
                {
                    //打开源数据库中的块表
                    BlockTable bt = (BlockTable)tm.GetObject(sourceDb.BlockTableId, OpenMode.ForRead, false);
                    //遍历每个块
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tm.GetObject(btrId, OpenMode.ForRead, false);
                        //只加入命名块和非布局块到复制列表中
                        if (!btr.IsAnonymous && !btr.IsLayout)
                        {
                            blockIds.Add(btrId);
                        }
                        btr.Dispose();
                    }
                    bt.Dispose();
                }
                //定义一个IdMapping对象
                IdMapping mapping = new IdMapping();
                //从源数据库向目标数据库复制块表记录
                sourceDb.WblockCloneObjects(blockIds, destDb.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);
                result = true;
            }
            catch (Exception ex)
            {

                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("未找到图库文件：BlkDYPD.dwg" + "\n错误原因:   " + ex.ToString());
                result = false;
            }
            //操作完成，销毁源数据库
            sourceDb.Dispose();
            return result;
        } //end of public static bool ImportBlocksFromDwg_DYPD(Database destDb)


        public static string GetCommonPrefix(List<string> strings)
        {
            if (strings == null || strings.Count == 0)
                return string.Empty;

            // 找到最短字符串的长度
            int minLength = strings.Min(s => s.Length);

            string prefix = string.Empty;

            for (int i = 0; i < minLength; i++)
            {
                char currentChar = strings[0][i]; // 取第一个字符串的当前字符

                // 检查所有字符串的当前字符是否相同
                foreach (var str in strings)
                {
                    if (str[i] != currentChar)
                    {
                        return prefix; // 一旦发现不相同，返回当前前缀
                    }
                }

                // 如果相同，将字符添加到前缀
                prefix += currentChar;
            }

            return prefix; // 返回找到的前缀
        }
    }

} //end of public partial class Form1 : Form



