using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace Devgis.Common
{
    public class OleHelper
    {
        private OleDbConnection StyleConnection;

        #region 构造方法
        private OleHelper()
        {
            #region 初始化连接信息
            string DBPath=Path.Combine(Application.StartupPath,"data.mdb");
             string strConStr= "Provider=Microsoft.Jet.OLEDB.4.0;Data source=" + DBPath;
             StyleConnection = new OleDbConnection(strConStr);
            #endregion
        }
        #endregion

        #region 单例
        private static OleHelper _instance = null;

        /// <summary>
        /// PGHelper的实例
        /// </summary>
        public static OleHelper Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new OleHelper();
                }
                return _instance;
            }
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 从型号基础库获取数据
        /// </summary>
        /// <param name="SQL">查询的SQL语句</param>
        /// <returns>查询的结果</returns>
        public DataTable GetDataTable(String SQL)
        {
            OleDbCommand cmd = new OleDbCommand(SQL, StyleConnection);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 执行单个SQL
        /// </summary>
        /// <param name="Sql">需要执行的SQL</param>
        /// <returns>执行结果</returns>
        public bool ExecuteSql(String Sql)
        {
            StyleConnection.Open();
            try
            {
                OleDbCommand oc = new OleDbCommand(Sql, StyleConnection);
                if (oc.ExecuteNonQuery() > 0)
                {
                    return true;
                }
            }
            catch
            {
            }
            finally
            {
                StyleConnection.Close();
            }
            return false;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="ListSql">SQL语句集合</param>
        /// <returns>执行结果</returns>
        public bool ExecuteSqlTran(List<String> ListSql)
        {
            StyleConnection.Open();
            OleDbTransaction sqlTran = StyleConnection.BeginTransaction();
            try
            {
                //OracleTransaction tx=conn.BeginTransaction();	
                foreach (String sql in ListSql)
                {
                    try
                    {
                        OleDbCommand oc = new OleDbCommand(sql, StyleConnection);
                        oc.Transaction = sqlTran;
                        oc.ExecuteNonQuery();
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        sqlTran.Rollback();
                        throw new Exception(E.Message);
                    }

                }
                sqlTran.Commit();
                return true;
            }
            catch
            {
                sqlTran.Rollback();
            }
            finally
            {
                StyleConnection.Close();
            }
            return false;
        }
        #endregion

    }
}
