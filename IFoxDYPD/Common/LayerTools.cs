using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using IFoxCAD.Cad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Common
{
    public static class LayerTools
    {
        public static void IfHasLayer(DBTrans tr, string LayerName,short colorindex)
        {
            if (!tr.LayerTable.Has(LayerName))
            {
                //要执行的操作
                tr.LayerTable.Add(LayerName, it =>
                {
                    it.Color = Color.FromColorIndex(ColorMethod.ByColor, colorindex);
                    it.LineWeight = LineWeight.ByLineWeightDefault;
                    it.IsPlottable = true;
                });
            }
        }
    }
}
