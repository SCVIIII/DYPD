using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;
using MySql.Data.MySqlClient;


namespace IFoxDYPD.SQL
{
    public class MySQLConnector
    {
        //连接到MySql数据库
        public MySqlConnection ConnMySQL()
        {

            try
            {
                string connectionString = "server=rm-bp16iybc06ihhzl713o.mysql.rds.aliyuncs.com;user=root;password=Szm123456;port=3306";
                MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                return conn;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            
        }

        //关闭MySql数据库
        public void  CloseMySQL(MySqlConnection conn)
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}
