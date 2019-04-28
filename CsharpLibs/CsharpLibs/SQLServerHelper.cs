using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace DEVGIS.CsharpLibs
{
    public static class SQLServerHelper
    {
        private static SqlConnection conn;
        static SQLServerHelper()
        {
            if (conn == null)
            {
                conn = new SqlConnection();
                conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["sqlstr"];
            }
        }
        
        /// <summary>
        /// ��ѯ����DataSet
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="parameters">�����б�</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = sql;
                if (parameters != null && parameters.Length > 0)
                {
                    sqlCmd.Parameters.AddRange(parameters);
                }
                sqlCmd.Connection = conn;
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                da.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// ��ѯ����DataTable
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="parameters">�����б�</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                DataSet ds = GetDataSet(sql, parameters);
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// ִ��SQL���
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <param name="parameters">�����б�</param>
        /// <returns></returns>
        public static bool ExecSql(string sql, SqlParameter[] parameters=null)
        {
            try
            {
                conn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = sql;
                if (parameters != null && parameters.Length>0)
                {
                    sqlCmd.Parameters.AddRange(parameters);
                }

                sqlCmd.Connection = conn;
                if (sqlCmd.ExecuteNonQuery() > 0)
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
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// ������ʽִ��SQL���
        /// </summary>
        /// <param name="listsqls">SQL���list</param>
        /// <param name="listparameters">�����б�list</param>
        /// <returns>�ɹ�����true ���򷵻�false</returns>
        public static bool ExecSqlByTran(List<string> listsqls, List<SqlParameter[]> listparameters)
        {
            if (listsqls == null || listparameters == null || listsqls.Count <= 0 || listparameters.Count <= 0 || listsqls.Count != listparameters.Count)
            {
                return false;
            }
            else
            {

                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    for (int i = 0; i < listsqls.Count; i++)
                    {
                        SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandText = listsqls[i];
                        sqlCmd.Parameters.AddRange(listparameters[i]);
                        sqlCmd.Connection = conn;
                        sqlCmd.Transaction = tran;
                        sqlCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                    conn.Close();
                    return true;
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    conn.Close();
                    return false;
                }
                
            }
        }

    }
}
