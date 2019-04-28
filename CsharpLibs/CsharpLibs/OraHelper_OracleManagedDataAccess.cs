using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DEVGIS.CsharpLibs
{
    public static class OraHelper_OracleManagedDataAccess
    {
        private static string oraString = string.Empty;
        static OraHelper_OracleManagedDataAccess()
        {
            oraString = System.Configuration.ConfigurationManager.AppSettings["OracleStr"].ToString();
        }

        /// <summary>
        /// 查询返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, OracleParameter[] parameters = null)
        {
            using (OracleConnection conn = new OracleConnection(oraString))
            {
                try
                {
                    DataTable dt = new DataTable();
                    OracleCommand oraCmd = new OracleCommand();
                    oraCmd.CommandText = sql;
                    if (parameters != null && parameters.Length > 0)
                    {
                        oraCmd.Parameters.AddRange(parameters);
                    }
                    oraCmd.Connection = conn;
                    OracleDataAdapter da = new OracleDataAdapter(oraCmd);
                    da.Fill(dt);
                    return dt;

                }
                catch (Exception ex)
                {
                    Loger.WriteLog(ex);
                    return null;
                }
            }
               
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static bool ExecSql(string sql, OracleParameter[] parameters)
        {
            using (OracleConnection conn = new OracleConnection(oraString))
            {
                try
                {
                    conn.Open();
                    OracleCommand oraCmd = new OracleCommand();
                    oraCmd.CommandText = sql;
                    if (parameters != null && parameters.Length > 0)
                    {
                        oraCmd.Parameters.AddRange(parameters);
                    }

                    oraCmd.Connection = conn;
                    if (oraCmd.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Loger.WriteLog(ex);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 以事务方式执行SQL语句
        /// </summary>
        /// <param name="listsqls">SQL语句list</param>
        /// <param name="listparameters">参数列表list</param>
        /// <returns>成功返回true 否则返回false</returns>
        public static bool ExecSqlByTran(List<string> listsqls, List<OracleParameter[]> listparameters)
        {
            if (listsqls == null || listparameters == null || listsqls.Count <= 0 || listparameters.Count <= 0 || listsqls.Count != listparameters.Count)
            {
                return false;
            }
            else
            {
                using (OracleConnection conn = new OracleConnection(oraString))
                {

                    conn.Open();
                    OracleTransaction tran = conn.BeginTransaction();
                    try
                    {
                        for (int i = 0; i < listsqls.Count; i++)
                        {
                            OracleCommand oraCmd = new OracleCommand();
                            oraCmd.CommandText = listsqls[i];
                            oraCmd.Parameters.AddRange(listparameters[i]);
                            oraCmd.Connection = conn;
                            oraCmd.Transaction = tran;
                            oraCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        conn.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        conn.Close();
                        Loger.WriteLog(ex);
                        return false;
                    }
                }

            }
        }
    }
 }
