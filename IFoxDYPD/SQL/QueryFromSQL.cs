using IFoxDYPD.Dtos;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IFoxDYPD.Dtos.HuiLuDtos;

namespace IFoxDYPD.SQL
{
    public class QueryFromSQL
    {
        //读取MYSQL表并提取铜排截面
        /// <summary>
        /// 查询铜排规格：读取MYSQL表并转换为CB_CLASS类
        /// </summary>
        /// <param name="args"></param>
        public  string QueryCooperBAR(MySqlConnection conn, int capacity)
        {
            //数据库连接
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            //返回值
            string COOPER_BAR = "";

            //数据库查询语句
            string query = "SELECT EB45_COOPER_BAR FROM base.DYPD_INCOMING_CABINET WHERE EB31_TRANSFORMER= " + capacity;

            //数据库查询
            using (var command = new MySqlCommand(query, conn))
            {
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        COOPER_BAR = reader["EB45_COOPER_BAR"].ToString();
                    }
                } //end of using (var reader = command.ExecuteReader())
            } //end of using (var command = new MySqlCommand(query, conn))

            return COOPER_BAR;
        } //end of public static void sql_CBClass(MySqlConnection conn)

        /// <summary>
        /// 查询进线柜信息：读取MYSQL表并转换为DYPD_INCOMING_CABINET类
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public  List<DYPD_INCOMING_CABINET> QueryIncomingCabinet(MySqlConnection conn, int capacity)
        {
            //数据库连接
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }


            //数据库查询语句
            string query = "SELECT * FROM base.DYPD_INCOMING_CABINET WHERE EB31_TRANSFORMER= " + capacity; // 替换为你的表名

            //数据库查询
            using (var command = new MySqlCommand(query, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    // 读取数据
                    List<DYPD_INCOMING_CABINET> results = new List<DYPD_INCOMING_CABINET>();

                    while (reader.Read())
                    {
                        DYPD_INCOMING_CABINET myObj = new DYPD_INCOMING_CABINET();
                        myObj.EB31_TRANSFORMER = (reader["EB31_TRANSFORMER"]).ToString() + "kVA";
                        myObj.EB12_PURPOSE = reader["EB12_PURPOSE"].ToString();
                        myObj.EB13_SIZE = reader["EB13_SIZE"].ToString();
                        myObj.EB14_TYPE = reader["EB14_TYPE"].ToString();
                        myObj.EB17_IR2 = reader["EB17_IR2"].ToString();
                        myObj.EB18_BREAKER = reader["EB18_BREAKER"].ToString();
                        myObj.EB19_In = reader["EB19_In"].ToString();
                        myObj.EB20_Ir1 = reader["EB20_Ir1"].ToString();
                        myObj.EB21_Ir3 = reader["EB21_Ir3"].ToString();
                        myObj.EB22_CT = reader["EB22_CT"].ToString();
                        myObj.EB23_INFO1 = reader["EB23_INFO1"].ToString();
                        myObj.EB32_VOLTAGE = reader["EB32_VOLTAGE"].ToString();
                        myObj.EB33_Ud = reader["EB33_Ud"].ToString();
                        myObj.EB34_IP = reader["EB34_IP"].ToString();
                        myObj.EB35_SIZE = reader["EB35_SIZE"].ToString();
                        myObj.EB36_INFO2 = reader["EB36_INFO2"].ToString();
                        myObj.EB41_IMCOMING_CABLE = reader["EB41_IMCOMING_CABLE"].ToString();
                        myObj.EB42_BUSBAR = reader["EB42_BUSBAR"].ToString();
                        myObj.EB43_BUSBAR_INFO = reader["EB43_BUSBAR_INFO"].ToString();
                        myObj.EB44_BUSBAR_PE = reader["EB44_BUSBAR_PE"].ToString();
                        results.Add(myObj);
                    }

                    //测试用:对结果进行显示
                    foreach (var obj in results)
                    {
                        //MessageBox.Show($"IR1: {obj.IR1}, HEIGHT: {obj.HEIGHT}, CT: {obj.CT}");
                    }

                    return results;
                } //end of using (var reader = command.ExecuteReader())
            } //end of using (var command = new MySqlCommand(query, conn))
        } //end of public static List<DYPD_INCOMING_CABINET> query_DYPD_INCOMING_CABINET(MySqlConnection conn, int capacity)

        //读取MYSQSL中的电容柜表并转换为CLASS
        public  List<MTVSDto> QueryMTVS(MySqlConnection conn, int capacity)
        {
            //数据库连接
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }


            //数据库查询语句
            string query = "SELECT * FROM base.DYPD_MVTS WHERE EB31_CAPACITY= " + capacity; // 替换为你的表名

            //数据库查询
            using (var command = new MySqlCommand(query, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    // 读取数据
                    List<MTVSDto> results = new List<MTVSDto>();

                    while (reader.Read())
                    {
                        MTVSDto myObj = new MTVSDto();
                        myObj.EB33_CAPACITY_1P = (reader["EB33_CAPACITY_1P"]).ToString() + "kvar";
                        myObj.EB12_PURPOSE = reader["EB12_PURPOSE"].ToString();
                        myObj.EB13_SIZE = reader["EB13_SIZE"].ToString();
                        myObj.EB14_TYPE = reader["EB14_TYPE"].ToString();
                        myObj.EB21_BUSBAR = "低压母线规格（待补充）";
                        myObj.EB31_CAPACITY = "总无功功率补偿有效输出为" + reader["EB31_CAPACITY"].ToString() + "kvar";
                        myObj.EB32_CAPACITY_3P = "三相无功功率补偿有效输出为" + reader["EB32_CAPACITY_3P"].ToString() + "kvar";
                        myObj.EB33_CAPACITY_1P = "单相无功功率补偿有效输出为" + reader["EB33_CAPACITY_1P"].ToString() + "kvar";
                        myObj.EB34_FUSE = reader["EB34_FUSE"].ToString();
                        myObj.EB35_CT = reader["EB35_CT"].ToString();
                        myObj.EB36_INFO1 = reader["EB36_INFO1"].ToString();
                        myObj.EB37_INFO2 = reader["EB37_INFO2"].ToString();
                        myObj.EB38_INFO3 = reader["EB38_INFO3"].ToString();
                        myObj.EB41_INFOA = reader["EB41_INFOA"].ToString();
                        myObj.EB42_INFOB = reader["EB42_INFOB"].ToString();
                        myObj.EB43_INFOC = reader["EB43_INFOC"].ToString();
                        myObj.EB44_INFOD = reader["EB44_INFOD"].ToString();
                        results.Add(myObj);
                    }

                    //测试用:对结果进行显示
                    foreach (var obj in results)
                    {
                        //MessageBox.Show($"IR1: {obj.IR1}, HEIGHT: {obj.HEIGHT}, CT: {obj.CT}");
                    }

                    return results;
                } //end of using (var reader = command.ExecuteReader())
            } //end of using (var command = new MySqlCommand(query, conn))
        } //end of public static List<DYPD_MTVS> query_DYPD_MTVS(MySqlConnection conn, int capacity)


        /// <summary>
        /// 读取MYSQL表并转换为CB_CLASS类
        /// </summary>
        /// <param name="args"></param>
        public  List<CB_CLASS> query_CBClass(MySqlConnection conn, int IR1)
        {
            //数据库连接
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            //数据库查询语句
            string query = "SELECT * FROM base.SCHNEIDER WHERE IR1= " + IR1; // 替换为你的表名

            //数据库查询
            using (var command = new MySqlCommand(query, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    // 读取数据
                    List<CB_CLASS> results = new List<CB_CLASS>();

                    while (reader.Read())
                    {
                        CB_CLASS myObj = new CB_CLASS();
                        myObj.IR1 = Convert.ToInt32(reader["IR1"]);
                        myObj.RATING = Convert.ToInt32(reader["RATING"]);
                        myObj.IN_40 = Convert.ToInt32(reader["IN_40"]);
                        myObj.HEIGHT = Convert.ToInt32(reader["HEIGHT"]);
                        myObj.IR2 = reader["IR2"].ToString();
                        myObj.IR3 = reader["IR3"].ToString();
                        myObj.CT = reader["CT"].ToString();
                        myObj.CB_INFO1 = reader["CB_INFO1"].ToString();
                        myObj.CB_INFO2 = reader["CB_INFO2"].ToString();
                        myObj.CB_INFO3 = reader["CB_INFO3"].ToString();
                        results.Add(myObj);
                    }

                    //测试用:对结果进行显示
                    foreach (var obj in results)
                    {
                        //MessageBox.Show($"IR1: {obj.IR1}, HEIGHT: {obj.HEIGHT}, CT: {obj.CT}");
                    }

                    return results;
                } //end of using (var reader = command.ExecuteReader())
            } //end of using (var command = new MySqlCommand(query, conn))
        } //end of public static void sql_CBClass(MySqlConnection conn)


    }

    

}
