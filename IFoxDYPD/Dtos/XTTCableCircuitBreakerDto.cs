using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFoxDYPD.Dtos
{
    public class XTTCableCircuitBreakerDto
    {

        //额定电流
        public int In { get; set; }
        //电缆截面
        public double CableCSA { get; set; }
        public string Cable { get; set; }
        public string CircuitBreaker { get; set; }
        //保护管径
        public int SC { get; set; }
        public string info1 { get; set; }
        public string info2 { get; set; }
        public string info3 { get; set; }
        public string info4 { get; set; }

        public static List<XTTCableCircuitBreakerDto> CreateXTTCableSwitchDto()
        {
            List<XTTCableCircuitBreakerDto> xTTCableSwitchDtos = new List<XTTCableCircuitBreakerDto>();
            xTTCableSwitchDtos.Add(new XTTCableCircuitBreakerDto { In = 10, CableCSA = 2.5, Cable = "2.5+E2.5", SC = 20 });
            xTTCableSwitchDtos.Add(new XTTCableCircuitBreakerDto { In = 16, CableCSA = 2.5, Cable = "2.5+E2.5", SC = 20 });
            xTTCableSwitchDtos.Add(new XTTCableCircuitBreakerDto { In = 20, CableCSA = 4, Cable = "4+E4", SC = 20 });
            xTTCableSwitchDtos.Add(new XTTCableCircuitBreakerDto { In = 25, CableCSA = 6, Cable = "6+E6", SC = 25 });
            xTTCableSwitchDtos.Add(new XTTCableCircuitBreakerDto { In = 32, CableCSA = 10, Cable = "10+E10", SC = 32 });
            xTTCableSwitchDtos.Add(new XTTCableCircuitBreakerDto { In = 40, CableCSA = 16, Cable = "16+E16", SC = 40 });
            xTTCableSwitchDtos.Add(new XTTCableCircuitBreakerDto { In = 50, CableCSA = 16, Cable = "16+E16", SC = 40 });
            return xTTCableSwitchDtos;
        }


    }
    



    public class XTTCircuitBreakerDto
    {

        //额定电流
        public int In { get; set; }
        //断路器
        public string CircuitBreaker { get; set; }
        public string info1 { get; set; }
        public string info2 { get; set; }
        public string info3 { get; set; }
        public string info4 { get; set; }

        


        /// <summary>
        /// 良信NDB1LE系列断路器，不包括后缀、三相单相
        /// </summary>
        /// <returns></returns>
        public List<XTTCircuitBreakerDto> CreateXTT_LazzenSwitchDto_NDB1LE()
        {
            List<XTTCircuitBreakerDto> xTTSwitchDtos = new List<XTTCircuitBreakerDto>();
            xTTSwitchDtos.Add(new XTTCircuitBreakerDto { In = 10, CircuitBreaker = "NDB1LE-" });
            xTTSwitchDtos.Add(new XTTCircuitBreakerDto { In = 16, CircuitBreaker = "NDB1LE-" });
            xTTSwitchDtos.Add(new XTTCircuitBreakerDto { In = 20, CircuitBreaker = "NDB1LE-" });
            xTTSwitchDtos.Add(new XTTCircuitBreakerDto { In = 25, CircuitBreaker = "NDB1LE-" });
            xTTSwitchDtos.Add(new XTTCircuitBreakerDto { In = 32, CircuitBreaker = "NDB1LE-" });
            xTTSwitchDtos.Add(new XTTCircuitBreakerDto { In = 40, CircuitBreaker = "NDB1LE-" });
            xTTSwitchDtos.Add(new XTTCircuitBreakerDto { In = 50, CircuitBreaker = "NDB1LE-" });
            xTTSwitchDtos.Add(new XTTCircuitBreakerDto { In = 63, CircuitBreaker = "NDB1LE-" });
            return xTTSwitchDtos;
        }

        


    }
}
