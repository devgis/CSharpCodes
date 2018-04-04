using System;
using System.Data;
using System.Data.OracleClient;
using System.Collections;
using System.Reflection;
using System.Configuration;

namespace YYGH.DAL
{
    //*********************************************************************
    //
    // The OracleHelper class is intended to encapsulate high performance, scalable best practices for 
    // common uses of OracleClient.
    //
    //*********************************************************************

    public sealed class OracleHelper
    {
        //*********************************************************************
        //
        // Since this class provides only static methods, make the default constructor private to prevent 
        // instances from being created with "new OracleHelper()".
        //
        //*********************************************************************

        private OracleHelper() { }

        //*********************************************************************
        //
        // This method is used to attach array of OracleParameters to a OracleCommand.
        // 
        // This method will assign a value of DbNull to any parameter with a direction of
        // InputOutput and a value of null.  
        // 
        // This behavior will prevent default values from being used, but
        // this will be the less common case than an intended pure output parameter (derived as InputOutput)
        // where the user provided no input value.
        // 
        // param name="command" The command to which the parameters will be added
        // param name="commandParameters" an array of OracleParameters tho be added to command
        //
        //*********************************************************************
        public static string connStr()
        {
            string connStr = new COMM.conn().GetConnStringNoDec();
            return connStr;
        }

        private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
            foreach (OracleParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        //*********************************************************************
        //
        // This method assigns an array of values to an array of OracleParameters.
        // 
        // param name="commandParameters" array of OracleParameters to be assigned values
        // param name="parameterValues" array of objects holding the values to be assigned
        //
        //*********************************************************************

        private static void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            //iterate through the OracleParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        //*********************************************************************
        //
        // This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        // to the provided command.
        // 
        // param name="command" the OracleCommand to be prepared
        // param name="connection" a valid OracleConnection, on which to execute this command
        // param name="transaction" a valid OracleTransaction, or 'null'
        // param name="commandType" the CommandType (stored procedure, text, etc.)
        // param name="commandText" the stored procedure name or T-Oracle command
        // param name="commandParameters" an array of OracleParameters to be associated with the command or 'null' if no parameters are required
        //
        //*********************************************************************

        private static void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or Oracle statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }

        //*********************************************************************
        //
        // Execute a OracleCommand (that returns no resultset) against the database specified in the connection string 
        // using the provided parameters.
        //
        // e.g.:  
        //  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        //
        // param name="connectionString" a valid connection string for a OracleConnection
        // param name="commandType" the CommandType (stored procedure, text, etc.)
        // param name="commandText" the stored procedure name or T-Oracle command
        // param name="commandParameters" an array of OracleParamters used to execute the command
        // returns an int representing the number of rows affected by the command
        //
        //*********************************************************************

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create & open a OracleConnection, and dispose of it after we are done.
            using (OracleConnection cn = new OracleConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        }

        //*********************************************************************
        //
        // Execute a stored procedure via a OracleCommand (that returns no resultset) against the database specified in 
        // the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        // stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        // 
        // This method provides no access to output parameters or the stored procedure's return value parameter.
        // 
        // e.g.:  
        //  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        //
        // param name="connectionString" a valid connection string for a OracleConnection
        // param name="spName" the name of the stored prcedure
        // param name="parameterValues" an array of objects to be assigned as the input values of the stored procedure
        // returns an int representing the number of rows affected by the command
        //
        //*********************************************************************

        /// <summary>
        /// 可执行四个基本操作。可无参数操作！
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="spName">储存过程名称</param>
        /// <param name="parameterValues">可变对象参数组</param>
        /// <returns>返回影响数据表中的行数。</returns>

        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of OracleParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        //*********************************************************************
        //
        // Execute a OracleCommand (that returns no resultset) against the specified OracleConnection 
        // using the provided parameters.
        // 
        // e.g.:  
        //  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        // 
        // param name="connection" a valid OracleConnection 
        // param name="commandType" the CommandType (stored procedure, text, etc.) 
        // param name="commandText" the stored procedure name or T-Oracle command 
        // param name="commandParameters" an array of OracleParamters used to execute the command 
        // returns an int representing the number of rows affected by the command
        //
        //*********************************************************************

        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters);

            //finally, execute the command.
            int retval = cmd.ExecuteNonQuery();

            // detach the OracleParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;
        }

        //*********************************************************************
        //
        // Execute a OracleCommand (that returns a resultset) against the database specified in the connection string 
        // using the provided parameters.
        // 
        // e.g.:  
        //  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        // 
        // param name="connectionString" a valid connection string for a OracleConnection 
        // param name="commandType" the CommandType (stored procedure, text, etc.) 
        // param name="commandText" the stored procedure name or T-Oracle command 
        // param name="commandParameters" an array of OracleParamters used to execute the command 
        // returns a dataset containing the resultset generated by the command
        //
        //*********************************************************************

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create & open a OracleConnection, and dispose of it after we are done.
            using (OracleConnection cn = new OracleConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteDataset(cn, commandType, commandText, commandParameters);
            }
        }

        //*********************************************************************
        //
        // Execute a stored procedure via a OracleCommand (that returns a resultset) against the database specified in 
        // the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        // stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        // 
        // This method provides no access to output parameters or the stored procedure's return value parameter.
        // 
        // e.g.:  
        //  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        // 
        // param name="connectionString" a valid connection string for a OracleConnection
        // param name="spName" the name of the stored procedure
        // param name="parameterValues" an array of objects to be assigned as the input values of the stored procedure
        // returns a dataset containing the resultset generated by the command
        //
        //*********************************************************************

        /// <summary>
        /// 可执行查询、修改等操作。可无参数操作！！
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="spName">储存过程名称</param>
        /// <param name="parameterValues">可变对象参数组</param>
        /// <returns>返回DataSet数据集</returns>
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of OracleParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        //*********************************************************************
        //
        // Execute a OracleCommand (that returns a resultset) against the specified OracleConnection 
        // using the provided parameters.
        // 
        // e.g.:  
        //  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        //
        // param name="connection" a valid OracleConnection
        // param name="commandType" the CommandType (stored procedure, text, etc.)
        // param name="commandText" the stored procedure name or T-Oracle command
        // param name="commandParameters" an array of OracleParamters used to execute the command
        // returns a dataset containing the resultset generated by the command
        //
        //*********************************************************************

        public static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            da.Fill(ds);

            // detach the OracleParameters from the command object, so they can be used again.			
            cmd.Parameters.Clear();

            //return the dataset
            return ds;
        }

        //*********************************************************************
        //
        // Execute a OracleCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        // using the provided parameters.
        // 
        // e.g.:  
        //  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new OracleParameter("@prodid", 24));
        // 
        // param name="connectionString" a valid connection string for a OracleConnection 
        // param name="commandType" the CommandType (stored procedure, text, etc.) 
        // param name="commandText" the stored procedure name or T-Oracle command 
        // param name="commandParameters" an array of OracleParamters used to execute the command 
        // returns an object containing the value in the 1x1 resultset generated by the command
        //
        //*********************************************************************

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create & open a OracleConnection, and dispose of it after we are done.
            using (OracleConnection cn = new OracleConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteScalar(cn, commandType, commandText, commandParameters);
            }
        }

        //*********************************************************************
        //
        // Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the database specified in 
        // the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        // stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        // 
        // This method provides no access to output parameters or the stored procedure's return value parameter.
        // 
        // e.g.:  
        //  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        // 
        // param name="connectionString" a valid connection string for a OracleConnection 
        // param name="spName" the name of the stored procedure 
        // param name="parameterValues" an array of objects to be assigned as the input values of the stored procedure 
        // returns an object containing the value in the 1x1 resultset generated by the command
        //
        //*********************************************************************

        /// <summary>
        /// 可用于事务操作。操作成功后返回操作第一个数据的数据类型空值。可无参数操作！
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="spName">储存过程名称</param>
        /// <param name="parameterValues">可变对象参数组</param>
        /// <returns>返回执行结果的第一行第一列的值</returns>
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of OracleParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        //*********************************************************************
        //
        // Execute a OracleCommand (that returns a 1x1 resultset) against the specified OracleConnection 
        // using the provided parameters.
        // 
        // e.g.:  
        //  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new OracleParameter("@prodid", 24));
        // 
        // param name="connection" a valid OracleConnection 
        // param name="commandType" the CommandType (stored procedure, text, etc.) 
        // param name="commandText" the stored procedure name or T-Oracle command 
        // param name="commandParameters" an array of OracleParamters used to execute the command 
        // returns an object containing the value in the 1x1 resultset generated by the command
        //
        //*********************************************************************

        public static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters);

            //execute the command & return the results
            object retval = cmd.ExecuteScalar();
            // detach the OracleParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();
            return retval;

        }
    }

    //*********************************************************************
    //
    // OracleHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
    // ability to discover parameters for stored procedures at run-time.
    //
    //*********************************************************************

    public sealed class OracleHelperParameterCache
    {
        //*********************************************************************
        //
        // Since this class provides only static methods, make the default constructor private to prevent 
        // instances from being created with "new OracleHelperParameterCache()".
        //
        //*********************************************************************

        private OracleHelperParameterCache() { }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        //*********************************************************************
        //
        // resolve at run time the appropriate set of OracleParameters for a stored procedure
        // 
        // param name="connectionString" a valid connection string for a OracleConnection 
        // param name="spName" the name of the stored procedure 
        // param name="includeReturnValueParameter" whether or not to include their return value parameter 
        //
        //*********************************************************************

        private static OracleParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            using (OracleConnection cn = new OracleConnection(connectionString))
            using (OracleCommand cmd = new OracleCommand(spName, cn))
            {
                cn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                OracleCommandBuilder.DeriveParameters(cmd);

                if (!includeReturnValueParameter)
                {
                    //cmd.Parameters.RemoveAt(0);
                }

                OracleParameter[] discoveredParameters = new OracleParameter[cmd.Parameters.Count]; ;

                cmd.Parameters.CopyTo(discoveredParameters, 0);

                return discoveredParameters;
            }
        }

        private static OracleParameter[] CloneParameters(OracleParameter[] originalParameters)
        {
            //deep copy of cached OracleParameter array
            OracleParameter[] clonedParameters = new OracleParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (OracleParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        //*********************************************************************
        //
        // add parameter array to the cache
        //
        // param name="connectionString" a valid connection string for a OracleConnection 
        // param name="commandText" the stored procedure name or T-Oracle command 
        // param name="commandParameters" an array of OracleParamters to be cached 
        //
        //*********************************************************************

        public static void CacheParameterSet(string connectionString, string commandText, params OracleParameter[] commandParameters)
        {
            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        //*********************************************************************
        //
        // Retrieve a parameter array from the cache
        // 
        // param name="connectionString" a valid connection string for a OracleConnection 
        // param name="commandText" the stored procedure name or T-Oracle command 
        // returns an array of OracleParamters
        //
        //*********************************************************************

        public static OracleParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string hashKey = connectionString + ":" + commandText;

            OracleParameter[] cachedParameters = (OracleParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        //*********************************************************************
        //
        // Retrieves the set of OracleParameters appropriate for the stored procedure
        // 
        // This method will query the database for this information, and then store it in a cache for future requests.
        // 
        // param name="connectionString" a valid connection string for a OracleConnection 
        // param name="spName" the name of the stored procedure 
        // returns an array of OracleParameters
        //
        //*********************************************************************

        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        //*********************************************************************
        //
        // Retrieves the set of OracleParameters appropriate for the stored procedure
        // 
        // This method will query the database for this information, and then store it in a cache for future requests.
        // 
        // param name="connectionString" a valid connection string for a OracleConnection 
        // param name="spName" the name of the stored procedure 
        // param name="includeReturnValueParameter" a bool value indicating whether the return value parameter should be included in the results 
        // returns an array of OracleParameters
        //
        //*********************************************************************

        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            OracleParameter[] cachedParameters;

            cachedParameters = (OracleParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                cachedParameters = (OracleParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
            }

            return CloneParameters(cachedParameters);
        }

    }
}
