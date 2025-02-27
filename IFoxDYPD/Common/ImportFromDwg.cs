using Autodesk.AutoCAD.DatabaseServices;
using IFoxCAD.Cad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Common
{
    public static class ImportFromDwg
    {
        


        /// <summary>
        /// 为低压配电程序,由外部CAD模板导入标准图块
        /// 返回值Bool,导入成功时返回true
        /// </summary>
        /// <param name="destDb"></param>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        public static bool ImportBlocksFromDwg_DYPD(DBTrans tr)
        {
            //返回值
            bool result = false;
            string sourceFileName = @"D:\Mycode\BlkDYPD\BlkDYPD_t3.dwg";

            //string sourceFileName= @"D:\Mycode\BlkDYPD\BlkDYPD.dwg";
            //创建一个新的数据库对象，作为源数据库，以读入外部文件中的对象
            Database sourceDb = Env.Database;
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
                //sourceDb.WblockCloneObjects(blockIds, destDb.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);
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
    }
}
