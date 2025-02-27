using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Dtos
{
    /// <summary>
    /// 存放风机三段式保护的规格
    /// </summary>
    public class XTTACDtos
    {

        private double _pe;
        /// <summary>
        /// 风机功率
        /// </summary>
        public double Pe
        {
            get => _pe;
            set => _pe = Math.Round(value, 1); // 保留一位小数
        }

        private string _pdxCB_Shell;
        /// <summary>
        /// 上级断路器壳架
        /// </summary>
        public string PDXCB_Shell
        {
            get { return _pdxCB_Shell; }
            set { _pdxCB_Shell = value; }
        }
        private double _pdxIn;
        /// <summary>
        /// 上级断路器的断路器额定电流
        /// </summary>
        public double PDXIn
        {
            get { return _pdxIn; }
            set { _pdxIn = value; }
        }






    }
}

