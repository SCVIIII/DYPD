using Autodesk.AutoCAD.DatabaseServices;
using IFoxCAD.Cad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Common
{
    public static class IFoxARXTools
    {
        public static string IFoxGetAttributeInBlockReference(this ObjectId blockReferenceId, DBTrans tr, string attributeName)
        {
            //新建返回值
            string attributeValue = string.Empty;
            // 获取块参照
            var bref = tr.GetObject<BlockReference>(blockReferenceId);
            // 遍历块参照的属性
            foreach (ObjectId attId in bref.AttributeCollection)
            {
                // 获取块参照属性对象
                AttributeReference attRef = tr.GetObject<AttributeReference>(attId);
                //判断属性名是否为指定的属性名
                if (attRef.Tag.ToUpper() == attributeName.ToUpper())
                {
                    attributeValue = attRef.TextString;//获取属性值
                    break;
                }
            }
            //返回值
            return attributeValue;
        }
    }
}
