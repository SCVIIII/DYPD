using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using IFoxDYPD.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Events
{
    public class InsertDYPDBlks
    {
        ///// <summary>
        ///// 函数功能：插入功能柜
        ///// </summary>
        ///// <param name="ptStart_TA"></param>
        ///// <returns></returns>
        //private List<Point3d> insert_DYPD_MULTI(Point3d ptStart_TA)
        //{
        //    //确定TB变压器的插入点
        //    Point3d ptStart_TB = ptStart_TA.PolarPoint(Math.PI / 2, -50000);
        //    //新建返回值列表，用于存放TA TB馈线柜的插入点
        //    List<Point3d> ptEND = new List<Point3d>();

        //    //TA和TB的柜信息列表
        //    (List<DYPD_MULTI>, List<DYPD_MULTI>) list_Return = get_lists_multi();
        //    List<DYPD_MULTI> list_DYPT_MULTI_TA = list_Return.Item1;
        //    List<DYPD_MULTI> list_DYPT_MULTI_TB = list_Return.Item2;

        //    //根据List<DYPD_MULTI>和插入点,插入功能柜
        //    if (list_DYPT_MULTI_TA.Count > 0)
        //    {
        //        Point3d ptEnd_TA = insert_MULTI_BLKS(ptStart_TA, list_DYPT_MULTI_TA, checkedListBox1.GetItemChecked(0));
        //        ptEND.Add(ptEnd_TA);
        //    }

        //    //返回值
        //    return ptEND;

        //} //end of private void insert_DYPD_MULTI(Point3d ptStart_TA)


    }

    
}
