using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
namespace MapApp.Commons
{
    /// <summary>
    /// ���ݷ��ʳ��������
    /// Copyright (C) Maticsoft
    /// </summary>
    public sealed class DbHelperSQL
    {
        public static String connectionString;  
        static DbHelperSQL()
        {
            connectionString = System.Configuration.ConfigurationManager.AppSettings["GISDBConnectionString"].ToString();  
        }

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// ִ��SQL��� ����SqlDataReader
        /// </summary>
        /// <param name="strSQL">SQL���</param>
        /// <returns>����SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string strSQL)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }   

        }

        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">����SQL���</param>		
        public static void ExecuteSqlTran(List<SqlCommand> ListSqlCommand)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction sqlTran = conn.BeginTransaction();
                //OracleTransaction tx=conn.BeginTransaction();	
                foreach (SqlCommand oc in ListSqlCommand)
                {
                    try
                    {
                        oc.Connection = conn;
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
            }
        }

    }

}
