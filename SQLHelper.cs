using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

namespace Bddd.Common
{
    public class SQLHelper
    {
        private SqlConnection StyleConnection;
        string strConStr=string.Empty;

        #region 构造方法
        private SQLHelper()
        {
            #region 初始化连接信息
            strConStr = System.Configuration.ConfigurationManager.AppSettings["SQLServer"].ToString();
            StyleConnection = new SqlConnection(strConStr);
            #endregion
        }
        #endregion

        #region 单例
        private static SQLHelper _instance = null;

        /// <summary>
        /// PGHelper的实例
        /// </summary>
        public static SQLHelper Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SQLHelper();
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
        public DataTable GetDataTable(String SQL, SqlParameter[] parameters = null)
        {
            using (var con = new SqlConnection(strConStr))
            {
                SqlCommand cmd = new SqlCommand(SQL, con);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool ExecSql(string sql, SqlParameter[] parameters=null)
        {
            using (var SqlConn = new SqlConnection(strConStr))
            {
                try
                {
                    SqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandText = sql;
                    if (parameters != null)
                    {
                        sqlCmd.Parameters.AddRange(parameters);
                    }
                    sqlCmd.Connection = SqlConn;
                    return sqlCmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    SqlConn.Close();
                }
            }
        }

        public bool ExecSqlByTran(List<string> listsqls, List<SqlParameter[]> listparameters)
        {
            if (listsqls == null || listparameters == null || listsqls.Count <= 0 || listparameters.Count <= 0 || listsqls.Count != listparameters.Count)
            {
                return false;
            }
            else
            {
                using (var SqlConn = new SqlConnection(strConStr))
                {
                    SqlConn.Open();
                    SqlTransaction tran = SqlConn.BeginTransaction();
                    try
                    {
                        for (int i = 0; i < listsqls.Count; i++)
                        {
                            SqlCommand sqlCmd = new SqlCommand();
                            sqlCmd.CommandText = listsqls[i];
                            if (listparameters!=null&&listparameters[i] != null)
                            {
                                sqlCmd.Parameters.AddRange(listparameters[i]);
                            }
                            sqlCmd.Connection = SqlConn;
                            sqlCmd.Transaction = tran;
                            sqlCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return false;
                    }
                    finally
                    {
                        SqlConn.Close();
                    }
                }
            }
        }
        #endregion

    }
}
