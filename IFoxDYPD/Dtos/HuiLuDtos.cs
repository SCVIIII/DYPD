using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Dtos
{
    public class HuiLuDtos
    {
        /// <summary>
        /// 存放变电模块对应100A及以上壳架塑壳、框架断路器的属性类
        /// </summary>
        public class CB_CLASS
        {
            public int IR1 { get; set; }
            public int RATING { get; set; }
            public int IN_40 { get; set; }
            public int HEIGHT { get; set; }
            public string IR2 { get; set; }
            public string IR3 { get; set; }
            public string CT { get; set; }
            public string CB_INFO1 { get; set; }
            public string CB_INFO2 { get; set; }
            public string CB_INFO3 { get; set; }

        } //end of class CableData
    }
}
