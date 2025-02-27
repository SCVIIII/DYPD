using IFoxCAD.Cad;
using IFoxDYPD.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFoxDYPD.Common
{
    public static class HuiIsValidTools
    {


        /// <summary>
        /// 校验命名方式是否合规:1ALG:m1/0.5kW
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="huilu"></param>
        /// <param name="pe"></param>
        /// <returns></returns>
        public static bool IsValidFormat(string input, out XTTHuiLuIsValidDto xttHuiLuIsValid)
        {
            //初始化返回值的class
            xttHuiLuIsValid = new XTTHuiLuIsValidDto();

            // 定义正则表达式
            // 照明系统
            // 形式1  箱名:回路/Pe
            // 形式2  箱名:回路
            // 形式4  箱名:回路:子配电箱名/Pe
            string pattern_Pe = @"^(?<name>[^:]+):(?<huilu>[^/]+)/(?<pe>[\d.]+)kW$";
            string pattern_NoPe = @"^(?<name>[^:]+):(?<huilu>[^/]+)$";
            string pattern_PDX = @"^(?<name>[^:]+):(?<huilu>[^/]+):(?<pdx_name>[^/]+)/(?<pe>[\d.]+)kW$";

            // 匹配字符串
            Match match_Pe = Regex.Match(input, pattern_Pe, RegexOptions.IgnoreCase);
            Match match_NoPe = Regex.Match(input, pattern_NoPe, RegexOptions.IgnoreCase);
            Match match_PDX = Regex.Match(input, pattern_PDX, RegexOptions.IgnoreCase);

            // 验证匹配结果
            //平面有标注Pe时,以平面为准
            if (match_PDX.Success)
            {
                xttHuiLuIsValid.Name = match_PDX.Groups["name"].Value;
                xttHuiLuIsValid.CircuitNumber = match_PDX.Groups["huilu"].Value;
                xttHuiLuIsValid.PDXName = match_PDX.Groups["pdx_name"].Value;
                // 尝试解析数字
                if (double.TryParse(match_Pe.Groups["pe"].Value, out double pe))
                {
                    xttHuiLuIsValid.Pe = pe;
                    xttHuiLuIsValid.IsValid = true;
                }
                return true;
            }
            else if (match_Pe.Success)
            {
                xttHuiLuIsValid.Name = match_Pe.Groups["name"].Value;
                xttHuiLuIsValid.CircuitNumber = match_Pe.Groups["huilu"].Value;
                // 尝试解析数字
                if (double.TryParse(match_Pe.Groups["pe"].Value, out double pe))
                {
                    xttHuiLuIsValid.Pe = pe;
                    xttHuiLuIsValid.IsValid = true;
                }
                return true;
            }
            //平面未标注时,仅设置箱名、回路,功率由后续WinForm中的函数设置默认值
            else if (match_NoPe.Success)
            {
                xttHuiLuIsValid.Name = match_Pe.Groups["name"].Value;
                xttHuiLuIsValid.CircuitNumber = match_Pe.Groups["huilu"].Value;
                return true;
            }
            //两种match都不满足时,返回false
            return false;
        }

        #region 含~的回路处理


        public static void Method_CalHuiLus(string input)
        {
            input = "c1~c11";

            // 正则表达式模式，匹配数字
            string pattern = @"([a-zA-Z]+)(\d+)~\1(\d+)";

            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                // 提取前缀和数字
                string pre = match.Groups[1].Value;  // 提取前缀
                string start = match.Groups[2].Value; // 提取起始数字
                string end = match.Groups[3].Value;   // 提取结束数字

                // 校验是否为有效整数
                if (int.TryParse(start, out int startNumber) && int.TryParse(end, out int endNumber))
                {
                    Console.WriteLine($"前缀: {pre}, 起始数字: {startNumber}, 结束数字: {endNumber}");
                }
                else
                {
                    Console.WriteLine("提取的数字不是有效的整数");
                }
            }
            else
            {
                Console.WriteLine("未找到匹配的数字范围");
            }
        }


        #endregion

        /// <summary>
        /// 判断字符串中是否含有英文的冒号":"
        /// 且内容为字符串+冒号+字符串的形式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IfStrContainsColon(string input)
        {
            // 正则表达式：^(.+):(.+)$
            string pattern = @"^(.+):(.+)$";
            return Regex.IsMatch(input, pattern);
        }

        //测试用的回路筛选函数
        public static List<XTTHuiLuIsValidDto> IsVaildHuiluInput(string input)
        {
            //新建返回值
            var listReturn = new List<XTTHuiLuIsValidDto>();

            //正则表达式判断是否有":",且有2或3个冒号
            //两个冒号(常规配电回路)
            string pattern_1colons = @"^(.+):(.+)$";
            //三个冒号(带子配电箱的情况)
            string pattern_2colons = @"^(.+):(.+):(.+)$";
            Match match_1colons = Regex.Match(input, pattern_1colons);
            Match match_2colons = Regex.Match(input, pattern_2colons);

            //只含有一个冒号时
            if (match_1colons.Success && !match_2colons.Success)
            {
                //尝试分割字符串
                string[] parts = input.Split(':');
                //异常时反馈false
                if (parts.Length != 2) { }
                string prefix = parts[0];  // ':'前的部分(所属配电箱名称)
                string suffix = parts[1];  // ':'后的部分(回路编号+功率)
                //形式3 包含"~"或","的回路
                if (suffix.Contains("~") || suffix.Contains(","))
                {
                    //提取Pe
                    //提取数字后面的"\\数字kW"
                    string pePattern = @"/(\d+(\.\d+)?)kW";
                    Match peMatch = Regex.Match(input, pePattern);
                    string PeStr = peMatch.Success ? peMatch.Groups[1].Value : string.Empty;

                    // 提取前缀
                    string prefixPattern = @"([a-zA-Z]+)";
                    Match prefixMatch = Regex.Match(suffix, prefixPattern);

                    if (prefixMatch.Success)
                    {
                        //提取回路编号的前缀字母
                        string prefix_DuoHuilus = prefixMatch.Groups[1].Value;
                        string[] ranges = suffix.Split(',');

                        foreach (string range in ranges)
                        {
                            // 正则表达式模式，匹配范围或单个数字
                            string pattern = $@"{prefix_DuoHuilus}(\d+)(?:~{prefix_DuoHuilus}(\d+))?";
                            Match match = Regex.Match(range, pattern);

                            if (match.Success)
                            {
                                // 如果是范围
                                if (match.Groups[2].Success)
                                {
                                    int start = int.Parse(match.Groups[1].Value);
                                    int end = int.Parse(match.Groups[2].Value);

                                    for (int i = start; i <= end; i++)
                                    {
                                        //初始化返回值的class
                                        var xttHuiLuIsValid = new XTTHuiLuIsValidDto();
                                        xttHuiLuIsValid.Name = prefix;
                                        xttHuiLuIsValid.CircuitNumber = prefix_DuoHuilus + i;
                                        if (double.TryParse(PeStr, out double pe))
                                        {
                                            xttHuiLuIsValid.Pe = pe;
                                        }
                                        listReturn.Add(xttHuiLuIsValid);
                                    }
                                }
                                // 如果是单个数字
                                else
                                {
                                    //获取单个数字对应的回路号
                                    int number = int.Parse(match.Groups[1].Value);
                                    //初始化返回值的class
                                    var xttHuiLuIsValid = new XTTHuiLuIsValidDto();
                                    xttHuiLuIsValid.Name = prefix;
                                    xttHuiLuIsValid.CircuitNumber = prefix_DuoHuilus + number;
                                    if (double.TryParse(PeStr, out double pe))
                                    {
                                        xttHuiLuIsValid.Pe = pe;
                                    }
                                    listReturn.Add(xttHuiLuIsValid);
                                }
                            }
                        }
                    }
                    else
                    {

                    }


                    //插入结束
                }
                else
                {
                    //
                    //初始化返回值的class
                    var xttHuiLuIsValid = new XTTHuiLuIsValidDto();

                    // 定义照明系统的正则表达式
                    // 形式1  箱名:回路/Pe
                    // 形式2  箱名:回路
                    string pattern_Pe = @"^(?<name>[^:]+):(?<huilu>[^/]+)/(?<pe>[\d.]+)kW$";
                    string pattern_NoPe = @"^(?<name>[^:]+):(?<huilu>[^/]+)$";

                    // 匹配字符串
                    Match match_Pe = Regex.Match(input, pattern_Pe, RegexOptions.IgnoreCase);
                    Match match_NoPe = Regex.Match(input, pattern_NoPe, RegexOptions.IgnoreCase);

                    //验证匹配结果
                    //平面有标注Pe时,以平面为准
                    if (match_Pe.Success)
                    {
                        xttHuiLuIsValid.Name = match_Pe.Groups["name"].Value;
                        xttHuiLuIsValid.CircuitNumber = match_Pe.Groups["huilu"].Value;
                        // 尝试解析数字
                        if (double.TryParse(match_Pe.Groups["pe"].Value, out double pe))
                        {
                            xttHuiLuIsValid.Pe = pe;
                            xttHuiLuIsValid.IsValid = true;
                        }
                        //将当前的回路信息添加到返回值中
                        listReturn.Add(xttHuiLuIsValid);
                    }
                    //平面未标注时,仅设置箱名、回路,功率由后续WinForm中的函数设置默认值
                    else if (match_NoPe.Success)
                    {
                        xttHuiLuIsValid.Name = match_NoPe.Groups["name"].Value;
                        xttHuiLuIsValid.CircuitNumber = match_NoPe.Groups["huilu"].Value;
                        //待补充:默认的回路功率
                        //xttHuiLuIsValid.IsValid = true;
                        //将当前的回路信息添加到返回值中
                        listReturn.Add(xttHuiLuIsValid);
                    }
                    //
                }


            } //end of if(match_2colons.Success)

            //含有两个冒号时
            //编号形式4:箱名 + 回路 + 子配电箱 + 功率
            else if (match_2colons.Success)
            {
                //初始化返回值的class
                var xttHuiLuIsValid = new XTTHuiLuIsValidDto();

                //定义正则表达式
                //照明系统
                //形式4  箱名:回路:子配电箱名/Pe
                string pattern_PDX = @"^(?<name>[^:]+):(?<huilu>[^/]+):(?<pdx_name>[^/]+)/(?<pe>[\d.]+)kW$";

                // 匹配字符串
                Match match_PDX = Regex.Match(input, pattern_PDX, RegexOptions.IgnoreCase);

                //验证匹配结果
                //平面有标注Pe时,以平面为准
                if (match_PDX.Success)
                {
                    xttHuiLuIsValid.Name = match_PDX.Groups["name"].Value;
                    xttHuiLuIsValid.CircuitNumber = match_PDX.Groups["huilu"].Value;
                    xttHuiLuIsValid.PDXName = match_PDX.Groups["pdx_name"].Value;
                    // 尝试解析数字
                    if (double.TryParse(match_PDX.Groups["pe"].Value, out double pe))
                    {
                        xttHuiLuIsValid.Pe = pe;
                        xttHuiLuIsValid.IsValid = true;
                        listReturn.Add(xttHuiLuIsValid);
                    }
                }
                
            } //end of else if(match_3colons.Success)
            //异常
            else
            {
                MessageBox.Show($"回路编号有误:{input}");
            }



            return listReturn;
        } //end of public static List<XTTHuiLuIsValidDto> IsVaildHuiluInput(string input)


    }


}
