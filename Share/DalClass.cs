using System.Data;
using System.Data.OleDb;

namespace DEVGIS.CsharpLibs
{
    public class Services
    {
        public DataTable Query(string where = "")
        {
            string sql = "select * from services where (DELETED<>1 or DELETED is null)";
            if (!string.IsNullOrEmpty(where))
            {
                sql += " and " + where;
            }
            return OledbHelper.GetDataTable(sql);
        }

        public bool Add(Service service)
        {
            string sql = "insert into SERVICES(ID, NAME, URL, REMARKS)VALUES(?,?,?,?)";
            OleDbParameter[] parms = new OleDbParameter[] {
                new OleDbParameter("ID",OleDbType.VarChar),
                new OleDbParameter("NAME",OleDbType.VarChar),
                 new OleDbParameter("PARAMS",OleDbType.VarChar),
                new OleDbParameter("URL",OleDbType.VarChar)
            };
            parms[0].Value = service.ID;
            parms[1].Value = service.NAME;
            parms[2].Value = service.URL;
            parms[3].Value = service.PARAMS;
            return OledbHelper.ExecSql(sql, parms);
        }

        public bool Edit(Service service)
        {
            string sql = "update SERVICES set NAME=?, URL=?, ContentType=?, PARAMS=? where ID=?";
            OleDbParameter[] parms = new OleDbParameter[] {
                new OleDbParameter("ID",OleDbType.VarChar),
                new OleDbParameter("NAME",OleDbType.VarChar),
                 new OleDbParameter("PARAMS",OleDbType.VarChar),
                new OleDbParameter("URL",OleDbType.VarChar)
            };

            parms[0].Value = service.ID;
            parms[1].Value = service.NAME;
            parms[2].Value = service.URL;
            parms[3].Value = service.PARAMS;
            return OledbHelper.ExecSql(sql, parms);
        }

        public bool Delete(string id)
        {
            string sql = "update SERVICES set DELETED=1 where ID=?";
            OleDbParameter[] parms = new OleDbParameter[] {
                new OleDbParameter("ID",OleDbType.VarChar)
            };

            parms[0].Value = id;
            return OledbHelper.ExecSql(sql, parms);
        }
    }

    public class Service
    {
        public string ID
        {
            get;
            set;
        }

        public string NAME
        {
            get;
            set;
        }

        public string URL
        {
            get;
            set;
        }

        public string PARAMS
        {
            get;
            set;
        }

        public string UserPassword
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Method
        {
            get;
            set;
        }

        public string Encoding
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }
    }
}
